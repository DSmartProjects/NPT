using Microsoft.Samples.SimpleCommunication;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using VideoKallMCCST.Communication;
using VideoKallMCCST.Helpers;
using VideoKallMCCST.Model;
using VideoKallMCCST.ViewModel;
using Windows.Media.MediaProperties;
using Windows.System.Display;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace VideoKallMCCST.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VideoCall : Page
    {
        MainPage rootPage = MainPage.mainPage;
        CaptureDevice device = null;
        bool? roleIsActive = null;
        int isTerminator = 0;
        bool activated = false;
        bool isMuted = false;
        IncomingConnectionEventArgs incommingCall = null;
        public VideoCallViewModel _videoCallVM = null;
        static DispatcherTimer timer = null;
       
        public VideoCall()
        {
            this.InitializeComponent();
            rootPage.EnsureMediaExtensionManager();
            _videoCallVM = MainPage.VideoCallVM;
            this.DataContext = _videoCallVM;
            _videoCallVM.DefaultVisibilities();
        }

        private async void TimerCallbackCompleted(object sender, object e)
        {
            timer.Stop();
            IncomingCallRing.Stop();
            await EndCallAsync();
        }

        public async void CallDevice(CaptureDevice device)
        {
            PreviewVideo.Source = device.CaptureSource;
            await device.CaptureSource.StartPreviewAsync();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var cameraFound = await CaptureDevice.CheckForRecordingDeviceAsync();
            if (cameraFound)
            {
                device = new CaptureDevice();
                await InitializeAsync();
                device.IncomingConnectionArrived += Device_IncomingConnectionArrived;
                device.CaptureFailed += Device_CaptureFailed;
                RemoteVideo.MediaFailed += RemoteVideo_MediaFailed;
            }
            else
            {
                //rootPage.NotifyUser("A machine with a camera and a microphone is required to run this sample.", NotifyType.ErrorMessage);
            }
        }

        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (activated)
            {
                RemoteVideo.Stop();

                RemoteVideo.Source = null;
            }

            if (device != null)
            {
                await device.CleanUpAsync();
                device = null;
            }
        }
        const int ERROR_FILE_NOT_FOUND = 2;
        const int ERROR_ACCESS_DENIED = 5;
        const int ERROR_NO_APP_ASSOCIATED = 1155;
        private async Task InitializeAsync(CancellationToken cancel = default(CancellationToken))
        {
            try
            {
                await device.InitializeAsync();
                await StartRecordToCustomSink();
                EndConsult.IsEnabled = true;
                RemoteVideo.Source = null;
                // Each client starts out as passive
                roleIsActive = false;
                Interlocked.Exchange(ref isTerminator, 0);
            }

            catch (Win32Exception e)
            {
                if (e.NativeErrorCode == ERROR_FILE_NOT_FOUND ||
                    e.NativeErrorCode == ERROR_ACCESS_DENIED ||
                    e.NativeErrorCode == ERROR_NO_APP_ASSOCIATED)
                {
                    MainPage.mainPage.RightPanelHolder.Navigate(typeof(VideoCall));
                    MainPage.mainPage.pagePlaceHolder.Navigate(typeof(LogoPage));
                }
            }
            catch (Exception)
            {
                //rootPage.NotifyUser("Initialization error. Restart the sample to try again.", NotifyType.ErrorMessage);
            }

        }

        async void RemoteVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            try
            {
                MainPage.VideoCallVM.PatientDetails = null;
                await EndCallAsync();
            }
            catch (Exception)
            {
            }
        }


        async void Device_IncomingConnectionArrived(object sender, IncomingConnectionEventArgs e)
        {
            incommingCall = e;          
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, (() =>
            {
                timer = new DispatcherTimer();
                timer.Start();
                timer.Interval = TimeSpan.FromSeconds(Constants.MIN_Media_Duration);            
                timer.Tick += TimerCallbackCompleted;
                _videoCallVM.IncomingCallRingVisibility = Visibility.Visible;
                _videoCallVM.AcceptVisiblity = Visibility.Visible;
                IncomingCallRing.Play();
            }));           
        }


        async void Device_CaptureFailed(object sender, Windows.Media.Capture.MediaCaptureFailedEventArgs e)
        {

            try
            {
                MainPage.VideoCallVM.PatientDetails = null;
                await EndCallAsync();
            }
            catch (Exception)
            {
                //_videoCallVM.DefaultVisibilities();
                //this.Frame.Navigate(typeof(VideoCall));
            }

        }

        private async Task StartRecordToCustomSink()
        {
            MediaEncodingProfile mep = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Qvga);
            mep.Video.FrameRate.Numerator = 15;
            mep.Video.FrameRate.Denominator = 1;
            mep.Container = null;
            await device.StartRecordingAsync(mep);
        }
        private async Task EndCallAsync()
        {
            incommingCall = null;
              //await device.CleanUpAsync();
            // end the CallButton. session
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, (() =>
            {
                _videoCallVM.DefaultVisibilities();
                MainPage.VideoCallVM.PatientDetails = null;
                IncomingCallRing.Stop();
                RemoteVideo.Stop();
                RemoteVideo.Source = null;
                PreviewVideo.Source = null;
                PreviewVideo.Visibility = Visibility.Collapsed;
                device.mediaSink.Dispose();
            }));
            //this.Frame.Navigate(typeof(VideoCall));
            // Start waiting for a new CallButton.
            await InitializeAsync();
            MainPage.mainPage.RightPanelHolder.Navigate(typeof(VideoCall));
            MainPage.mainPage.pagePlaceHolder.Navigate(typeof(LogoPage));
        }
        private async Task PatientEndCallAsync()
        {
            //timer.Tick += TimerCallbackCompleted;       
            await device.CleanUpAsync();
            RemoteVideo.Stop();
            PreviewVideo.Source = null;
            PreviewVideo.IsTapEnabled = true;
            PreviewVideo.Visibility = Visibility.Collapsed;
            RemoteVideo.Visibility = Visibility.Collapsed;
            VideoLogo.Visibility = Visibility.Visible;
            // Start waiting for a new CallButton.
            await InitializeAsync();
            _videoCallVM.DefaultVisibilities();
            MainPage.mainPage.RightPanelHolder.Navigate(typeof(VideoCall));
            MainPage.mainPage.pagePlaceHolder.Navigate(typeof(LogoPage));
            // MainPage.mainPage.pagePlaceHolder.Navigate(typeof(VideoCall));
            //TestPanelExpander.TestPanelExp.Frame.Navigate(typeof(LogoPage));
        }


        private void Apchair1_Click(object sender, RoutedEventArgs e)
        {
            MainPage.mainPage.SeatReclineMsg?.Invoke(false);
        }

        private void Apchair2_Click(object sender, RoutedEventArgs e)
        {
            MainPage.mainPage.SeatReclineMsg?.Invoke(true);
        }

        private async void BtnSearchPatient_Click(object sender, RoutedEventArgs e)
        {
            SearchPatient searchPatient = new SearchPatient();
            await searchPatient.ShowAsync();
            //SearchPatientPOP.IsOpen = true;
        }
        private async void Accept_Click(object sender, RoutedEventArgs e)
        {
            await AcceptCall();
            btnSearchPatient.IsEnabled = true;
        }
        private async Task AcceptCall()
        {
            timer.Tick -= TimerCallbackCompleted;
            timer.Stop();
            timer.Tick += TimerCallbackCompleted;
            //timer.Tick -= (o, args) =>
            //{
            //};
            IncomingCallRing.Stop();
            CallDevice(device);
            _videoCallVM.RemoteVideoVisiblity = Visibility.Visible;
            _videoCallVM.PreviewVideoVisiblity = Visibility.Visible;
            _videoCallVM.MuteVisiblity = Visibility.Visible;
            _videoCallVM.EndConsultVisibility = Visibility.Visible;
            _videoCallVM.AcceptVisiblity = Visibility.Collapsed;
            _videoCallVM.IsEnableEndConsult = true;
            TxtSMCStatus.Content = string.Empty;
            TxtSMCStatus.Content = "In Use";
            if (incommingCall == null)
                return;
            incommingCall.Accept();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, (() =>
            {
                activated = true;
                var remoteAddress = incommingCall.RemoteUrl;

                Interlocked.Exchange(ref isTerminator, 0);

                if (!((bool)roleIsActive))
                {
                // Passive client
                RemoteVideo.Source = new Uri(remoteAddress);
                }
                remoteAddress = remoteAddress.Replace("stsp://", "");
            }));

        }


        async Task EndVideoCall()
        {
            try
            {
                incommingCall = null;
                if (Interlocked.CompareExchange(ref isTerminator, 1, 0) == 0)
                {
                    await EndCallAsync();
                }
            }
            catch (Exception)
            {
            }
        }

        // Create this variable at a global scope.Set it to null.
        private DisplayRequest appDisplayRequest = null;
        MediaElement mediaElement = null;
        private void RemoteVideo_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            mediaElement = sender as MediaElement;
            if (mediaElement != null && mediaElement.IsAudioOnly == false)
            {
                if (mediaElement.CurrentState == MediaElementState.Playing)
                {
                    if (appDisplayRequest == null)
                    {
                        // This call creates an instance of the DisplayRequest object. 
                        appDisplayRequest = new DisplayRequest();
                        appDisplayRequest.RequestActive();
                    }
                }
                else // CurrentState is Buffering, Closed, Opening, Paused, or Stopped. 
                {
                    if (appDisplayRequest != null)
                    {
                        // Deactivate the display request and set the var to null.
                        appDisplayRequest.RequestRelease();
                        appDisplayRequest = null;
                    }
                }
            }
        }

        private async void Reject_Click(object sender, RoutedEventArgs e)
        {
            await EndVideoCall();
        }

        private async void EndConsult_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null && Interlocked.CompareExchange(ref isTerminator, 1, 0) == 0)
            {
                TxtSMCStatus.Content = string.Empty;
                TxtSMCStatus.Content = "Not Ready";
                await EndCallAsync();
            }


        }

        private async void Mute_Click(object sender, RoutedEventArgs e)
        {
            if (isMuted == false)
            {
                RemoteVideo.IsMuted = true;
                isMuted = true;
                Mute.Visibility = Visibility.Collapsed;
                UnMute.Visibility = Visibility.Visible;
            }

        }

        private async void UnMute_Click(object sender, RoutedEventArgs e)
        {
            if (isMuted == true)
            {
                RemoteVideo.IsMuted = false;
                isMuted = false;
                Mute.Visibility = Visibility.Visible;
                UnMute.Visibility = Visibility.Collapsed;
            }

        }
        private async Task CleanupAsync()
        {
            try
            {
                PreviewVideo.Source = null;
                PreviewVideo.Visibility = Visibility.Collapsed;
                await device.CleanUpAsync();
            }
            catch (Exception)
            {
            }
            finally
            {
                PreviewVideo.Visibility = Visibility.Collapsed;
                PreviewVideo.Source = null;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            btnSearchPatient.IsEnabled = false;
            SMCConnecteionStatus();

            SMCCommChannel = new CommunicationChannel();
            SMCCommChannel.Initialize();

            Utility ut = new Utility();

            var result = Task.Run(async () => { return await ut.ReadIPaddress(); }).Result;
            SMCCommChannel.IPAddress = MainPage.mainPage.mainpagecontext.TxtIpAddress;// "192.168.0.33";
            SMCCommChannel.PortNo = MainPage.mainPage.mainpagecontext.TxtProtNo;// "9856";
                                                                                //SMCCommChannel.MessageReceived += SMCCommChannel_MessageReceived;
            MainPage.mainPage.mainpagecontext.IsSMCConnected = false;
            SMCCommChannel.Connect();
            SMCCommChannel.SendMessage(CommunicationCommands.MCCConnection);
            watchdog = new DispatcherTimer();
            watchdog.Tick += Watchdog_Tick;
            watchdog.Interval = new TimeSpan(0, 0, 1);
            watchdog.Start();
            //CommToDataAcq.MessageReceived += SMCCommChannel_MessageReceived;
            //CommToDataAcq.Initialize();
            //CommToDataAcq.Connect();
        }



        public void SMCConnecteionStatus()
        {
            string status = (string)TxtSMCStatus.Content;
            if (status == "In Use")
            {
                TxtSMCStatus.Content = "In Use";
                ColorChange();
            }
            else
            {
                TxtSMCStatus.Content = MainPage.mainPage.mainpagecontext.IsSMCConnected ? "Ready" : "Not Ready";
                ColorChange();
            }
        }

        private void Watchdog_Tick(object sender, object e)
        {
            watchdog.Stop();
            if (statuscheckinterval > 1 || MainPage.mainPage.mainpagecontext.IsSMCConnected)
                SMCConnecteionStatus();

            if (!MainPage.mainPage.mainpagecontext.IsSMCConnected && intervalcount >= 5)
            {
                intervalcount = 0;
                SMCCommChannel.IPAddress = MainPage.mainPage.mainpagecontext.TxtIpAddress;// "192.168.0.33";
                SMCCommChannel.PortNo = MainPage.mainPage.mainpagecontext.TxtProtNo; // "9856"
                SMCCommChannel.Connect();
                SMCCommChannel.SendMessage(CommunicationCommands.MCCConnection);
            }
            else if (MainPage.mainPage.mainpagecontext.IsSMCConnected && statuscheckinterval >= 5)
            {
                statuscheckinterval = 0;
                MainPage.mainPage.mainpagecontext.IsSMCConnected = false;
                SMCCommChannel.SendMessage(CommunicationCommands.MCCConnectionStatusCheckCmd);
            }
            else if (!MainPage.mainPage.mainpagecontext.IsSMCConnected && intervalcount % 2 == 0)
            {
                SMCCommChannel.SendMessage(CommunicationCommands.MCCConnectionStatusCheckCmd);
            }

            if (intervalcount > 6)
                intervalcount = 0;
            //  if(!isDataAcquitionappConnected && statuscheckinterval>2)
            //     CommToDataAcq.SendMessageToDataacquistionapp("ConnectionTest");

            intervalcount++;

            statuscheckinterval++;
            // mainpagecontext.UpdateStatus(mainpagecontext.IsSMCConnected);
            watchdog.Start();
        }

        public SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            return new SolidColorBrush(
                Color.FromArgb(
                    255,
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16)
                )
            );
        }
        public void ColorChange()
        {
            if (!MainPage.mainPage.mainpagecontext.IsSMCConnected)
                TxtSMCStatus.Background = GetColorFromHexa("#ED604A");
            else
                TxtSMCStatus.Background = GetColorFromHexa("#34CBA8");

            string status = (string)TxtSMCStatus.Content;
            if (status == "In Use")
            {
                TxtSMCStatus.Background = GetColorFromHexa("#FFC10D");
            }
        }
        public CommunicationChannel SMCCommChannel { get; private set; }

        int intervalcount = 0;
        int statuscheckinterval = 0;

        DispatcherTimer watchdog = null;


    }
}
