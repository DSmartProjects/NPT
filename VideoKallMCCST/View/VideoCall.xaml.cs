using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using VideoKallMCCST.Communication;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Samples.SimpleCommunication;
using Windows.Media.MediaProperties;
using Windows.UI.Core;
using VideoKallMCCST.ViewModel;
using Windows.UI.Popups;
using Windows.System.Display;
using Windows.UI.Xaml.Media;
using Windows.Media.Playback;
using Windows.UI.Xaml.Media.Imaging;

namespace VideoKallMCCST.View
{
    public sealed partial class VideoCall : Page
    {        
        MainPage rootPage = MainPage.mainPage;
        CaptureDevice device = null;
        bool? roleIsActive = null;
        int isTerminator = 0;
        bool activated = false;
        bool isMuted = true;

        IncomingConnectionEventArgs incommingCall = null;

        public VideoCall()
        {
            this.InitializeComponent();
            rootPage.EnsureMediaExtensionManager();
            DefaultVisibilities();
        }

        public void DefaultVisibilities()
        {
            RemoteVideo.Visibility = Visibility.Collapsed;
            IncomingCallRing.Visibility = Visibility.Visible;

            //VideoVisibility.Visibility = Visibility.Collapsed;
            Mute.Visibility = Visibility.Collapsed;
            EndConsult.Visibility = Visibility.Collapsed;
            Accept.Visibility = Visibility.Collapsed;
            Reject.Visibility = Visibility.Collapsed;

        }
        public async void CallDevice()
        {
            PreviewVideo.Source = device.CaptureSource;
            await device.CaptureSource.StartPreviewAsync();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
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
            //rootPage.NotifyUser("Initializing..", NotifyType.StatusMessage);

            try
            {
                await device.InitializeAsync();
                await StartRecordToCustomSink();
                EndConsult.IsEnabled = false;
                RemoteVideo.Source = null;
                // Each client starts out as passive
                roleIsActive = false;
                Interlocked.Exchange(ref isTerminator, 0);
                //rootPage.NotifyUser("Tap 'CallButton.' button to start CallButton.", NotifyType.StatusMessage);
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
                await EndCallAsync();
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, (() =>
                {

                }));

            }
        }

        async void Device_IncomingConnectionArrived(object sender, IncomingConnectionEventArgs e)
        {
            incommingCall = e;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, (() =>
            {
                IncomingCallRing.Play();
                Reject.Visibility = Visibility.Visible;
                Accept.Visibility = Visibility.Visible;
            }));

        }


        async void Device_CaptureFailed(object sender, Windows.Media.Capture.MediaCaptureFailedEventArgs e)
        {

            if (Interlocked.CompareExchange(ref isTerminator, 1, 0) == 0)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, (() =>
                {
                     EndCallAsync();
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

            // Start waiting for a new CallButton.
            await InitializeAsync();

            PreviewVideo.Source = null;

        }



        private async Task AcceptCall()
        {
            IncomingCallRing.Stop();
            CallDevice();
            RemoteVideo.Visibility = Visibility.Visible;
            PreviewVideo.Visibility = Visibility.Visible;
            //VideoVisibility.Visibility = Visibility.Visible;
            Mute.Visibility = Visibility.Visible;
            EndConsult.Visibility = Visibility.Visible;
            Reject.Visibility = Visibility.Collapsed;
            Accept.Visibility = Visibility.Collapsed;
            EndConsult.IsEnabled = true;

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



        /// <summary>
        /// end the video call
        /// </summary>
        /// <returns></returns>
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

        private async void Accept_Click(object sender, RoutedEventArgs e)
        {
            await AcceptCall();
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

        }
   

        private async void Mute_Click(object sender, RoutedEventArgs e)
        {
            if (isMuted==false)
            {
                RemoteVideo.IsMuted = false;
                isMuted = true;
                MuteImg.Source = new BitmapImage(new Uri("ms-appx:///Images/microphone.png"));

            }
            else if (isMuted==true)
            {
                RemoteVideo.IsMuted = true;
                isMuted = false;
                MuteImg.Source = new BitmapImage(new Uri("ms-appx:///Images/mute.png"));
            }
        }
    }
}
