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
        public VideoCallPage()
        {
            this.InitializeComponent();
            rootPage.EnsureMediaExtensionManager();
            DefaultVisibilities();
            _videoCallVM = MainPage.VideoCallVM;          
            this.DataContext =_videoCallVM;            
        }    
        public void DefaultVisibilities()
        {
            RemoteVideo.Visibility = Visibility.Collapsed;
            IncomingCallRing.Visibility = Visibility.Visible;
            //VideoVisibility.Visibility = Visibility.Collapsed;
            Mute.Visibility = Visibility.Collapsed;
            EndConsult.Visibility = Visibility.Collapsed;
            Accept.Visibility = Visibility.Collapsed;
            MuteUn.Visibility = Visibility.Collapsed;
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
                // Reject.Visibility = Visibility.Visible;
                Accept.Visibility = Visibility.Visible;
                Incall.Visibility = Visibility.Visible;
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

            //DefaultVisibilities();
            await device.CleanUpAsync();


            RemoteVideo.Stop();

            // Start waiting for a new CallButton.
            await InitializeAsync();

            PreviewVideo.Source = null;

            TestPanelExpander.TestPanelExp.Frame.Navigate(typeof(LogoPage));
        }

        private void Apchair1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Apchair2_Click(object sender, RoutedEventArgs e)
        {

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
            Incall.Visibility = Visibility.Collapsed;
            Accept.Visibility = Visibility.Collapsed;
            EndConsult.IsEnabled = true;
            Accept.Visibility = Visibility.Collapsed;

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
            DefaultVisibilities();
        }

        private async void Mute_Click(object sender, RoutedEventArgs e)
        {
            if (isMuted == false)
            {
                RemoteVideo.IsMuted = true;
                isMuted = true;
                Mute.Visibility = Visibility.Collapsed;
                MuteUn.Visibility = Visibility.Visible;
            }

        }

        private async void UnMute_Click(object sender, RoutedEventArgs e)
        {
            if (isMuted == true)
            {
                RemoteVideo.IsMuted = false;
                isMuted = false;
                Mute.Visibility = Visibility.Visible;
                MuteUn.Visibility = Visibility.Collapsed;
            }

        }

        //private void BtnSearchPatientResult_Click(object sender, RoutedEventArgs e)
        //{
        //    //SearchPatientPOP.IsOpen = false;

        //    SearchPatient searchPatient = new SearchPatient();
           


        //}
    }
}
