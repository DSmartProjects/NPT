using Microsoft.Samples.SimpleCommunication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using VideoKallMCCST.Communication;
using VideoKallMCCST.Model;
using VideoKallMCCST.ViewModel;
using VideoKallMCCST.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.MediaProperties;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace VideoKallMCCST.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VideoCallPage : Page
    {
        MainPage rootPage = MainPage.mainPage;
        CaptureDevice device = null;
        bool? roleIsActive = null;
        int isTerminator = 0;
        bool activated = false;
        bool isMuted = false;
        IncomingConnectionEventArgs incommingCall = null;
        public VideoCallViewModel _videoCallVM = null;
        bool isChairInclined = false;
        public VideoCallPage()
        {
            this.InitializeComponent();
            rootPage.EnsureMediaExtensionManager();
            this.DefaultVisibilities();
            this.SetChairPosition();
            _videoCallVM = MainPage.VideoCallVM;
            this.DataContext = _videoCallVM;
        }

        public void SetChairPosition()
        {
            if (isChairInclined == false)
            {
                isChairInclined = true;
                UprightChair.IsEnabled = false;
                LeaningChair.IsEnabled = true;
            }
        }

        public void DefaultVisibilities()
        {
            RemoteVideo.Visibility = Visibility.Collapsed;
            IncomingCallRing.Visibility = Visibility.Collapsed;
            Mute.Visibility = Visibility.Collapsed;
            EndConsult.Visibility = Visibility.Collapsed;
            Accept.Visibility = Visibility.Collapsed;
            UnMute.Visibility = Visibility.Collapsed;
            VideoLogo.Visibility = Visibility.Visible;
            //VideoVisibility.Visibility = Visibility.Collapsed;
            //Reject.Visibility = Visibility.Collapsed;

        }
        public async void CallDevice()
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
        private async Task InitializeAsync(CancellationToken cancel = default(CancellationToken))
        {

            try
            {
                await device.InitializeAsync();
                await StartRecordToCustomSink();
                EndConsult.IsEnabled = false;
                RemoteVideo.Source = null;
                // Each client starts out as passive
                roleIsActive = false;
                Interlocked.Exchange(ref isTerminator, 0);
            }
            catch (Exception)
            {
                //rootPage.NotifyUser("Initialization error. Restart the sample to try again.", NotifyType.ErrorMessage);
            }

        }

        async void RemoteVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (Interlocked.CompareExchange(ref isTerminator, 1, 0) == 0)
            {               
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, (async () =>
                {
                    await EndCallAsync();
                }));
            }
        }

        async void Device_IncomingConnectionArrived(object sender, IncomingConnectionEventArgs e)
        {
            incommingCall = e;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, (() =>
            {
                VideoLogo.Visibility = Visibility.Collapsed;
                IncomingCallRing.Visibility = Visibility.Visible;
                IncomingCallRing.Play();
                Accept.Visibility = Visibility.Visible;
                //Incall.Visibility = Visibility.Visible;
                // Reject.Visibility = Visibility.Visible;
            }));

        }


        async void Device_CaptureFailed(object sender, Windows.Media.Capture.MediaCaptureFailedEventArgs e)
        {

            if (Interlocked.CompareExchange(ref isTerminator, 1, 0) == 0)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, (async () =>
                {
                    await EndCallAsync();
                }));

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
           
            DefaultVisibilities();
            await device.CleanUpAsync();
            RemoteVideo.Stop();
            PreviewVideo.Source = device.CaptureSource;
            PreviewVideo.Visibility = Visibility.Collapsed;
            RemoteVideo.Visibility = Visibility.Collapsed;
            VideoLogo.Visibility = Visibility.Visible;
            // Start waiting for a new CallButton.
            await InitializeAsync();
            rootPage.pagePlaceHolder.Navigate(typeof(LogoPage));
            //TestPanelExpander.TestPanelExp.Frame.Navigate(typeof(LogoPage));
        }

        private void Apchair1_Click(object sender, RoutedEventArgs e)
        {
            if (isChairInclined == false)
            {
                isChairInclined = true;
                UprightChair.IsEnabled = false;
                LeaningChair.IsEnabled = true;
            }
        }

        private void Apchair2_Click(object sender, RoutedEventArgs e)
        {
            if (isChairInclined == true)
            {
                isChairInclined = false;
                LeaningChair.IsEnabled = false;
                UprightChair.IsEnabled = true;
            }
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
        }
        private async Task AcceptCall()
        {
            //LogoPage._logoPage.Frame.Navigate(typeof(TestPanelExpander));
            IncomingCallRing.Stop();
            CallDevice();
            RemoteVideo.Visibility = Visibility.Visible;
            PreviewVideo.Visibility = Visibility.Visible;
            //VideoVisibility.Visibility = Visibility.Visible;
            Mute.Visibility = Visibility.Visible;        
            EndConsult.Visibility = Visibility.Visible;
            //Incall.Visibility = Visibility.Collapsed;
            Accept.Visibility = Visibility.Collapsed;
            EndConsult.IsEnabled = true;
            Accept.Visibility = Visibility.Collapsed;
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
                return;
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
            { }
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
                // end the CallButton. session
                await EndCallAsync();
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, (() =>
                {

                }));

            }
            TxtSMCStatus.Content = string.Empty;
            TxtSMCStatus.Content = "Not Ready";
            DefaultVisibilities();
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
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
            string status =(string) TxtSMCStatus.Content;
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
