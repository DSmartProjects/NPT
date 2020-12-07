using Microsoft.Samples.SimpleCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoKallMCCST.Communication;
using VideoKallMCCST.Model;
using VideoKallMCCST.View;
using Windows.Media.MediaProperties;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VideoKallMCCST.ViewModel
{
    public class VideoCallViewModel:NotifyPropertyChanged
    {
        CaptureDevice device = null;
        bool? roleIsActive = null;
        int isTerminator = 0;
        bool activated = false;
        bool isMuted = false;
        IncomingConnectionEventArgs incommingCall = null;


        private Patient _patientDetails = null;
        public Patient PatientDetails { get { return _patientDetails; } set { _patientDetails = value; OnPropertyChanged("PatientDetails"); } }


        private Visibility _remoteVideoVisiblity = Visibility.Collapsed;
        public Visibility RemoteVideoVisiblity { get { return _remoteVideoVisiblity; } set { _remoteVideoVisiblity = value; OnPropertyChanged("RemoteVideoVisiblity"); } }

        private Visibility _previewVideoVisiblity = Visibility.Collapsed;
        public Visibility PreviewVideoVisiblity { get { return _previewVideoVisiblity; } set { _previewVideoVisiblity = value; OnPropertyChanged("PreviewVideoVisiblity"); } }

        private Visibility _incomingCallRingVisibility = Visibility.Collapsed;
        public Visibility IncomingCallRingVisibility { get { return _incomingCallRingVisibility; } set { _incomingCallRingVisibility = value; OnPropertyChanged("IncomingCallRingVisibility"); } }

        private Visibility _muteVisibility = Visibility.Collapsed;
        public Visibility MuteVisiblity { get { return _muteVisibility; } set { _muteVisibility = value; OnPropertyChanged("MuteVisiblity"); } }

        private Visibility _endConsult = Visibility.Collapsed;
        public Visibility EndConsultVisibility { get { return _endConsult; } set { _endConsult = value; OnPropertyChanged("EndConsultVisibility"); } }

        private Visibility _acceptVisiblity = Visibility.Collapsed;
        public Visibility AcceptVisiblity { get { return _acceptVisiblity; } set { _acceptVisiblity = value; OnPropertyChanged("AcceptVisiblity"); } }

        private Visibility _unMuteVisiblity = Visibility.Collapsed;
        public Visibility UnMuteVisiblity { get { return _unMuteVisiblity; } set { _unMuteVisiblity = value; OnPropertyChanged("UnMuteVisiblity"); } }

        private Visibility _videoLogoVisiblity = Visibility.Visible;
        public Visibility VideoLogoVisiblity { get { return _videoLogoVisiblity; } set { _videoLogoVisiblity = value; OnPropertyChanged("VideoLogoVisiblity"); } }

        //private CaptureElement _previewVideo = null;
        //public CaptureElement PreviewVideo { get { return _previewVideo; } set { _previewVideo = value; OnPropertyChanged("PreviewVideo"); } }

        //private MediaElement __remoteVideo = null;
        //public MediaElement RemoteVideo { get { return __remoteVideo; } set { __remoteVideo = value; OnPropertyChanged("RemoteVideo"); } }

        private bool _isEnableEndConsult = true;
        public bool IsEnableEndConsult { get { return _isEnableEndConsult; } set { _isEnableEndConsult = value; OnPropertyChanged("IsEnableEndConsult"); } }


        


        public VideoCallViewModel() {

        }
        public void DefaultVisibilities()
        {
            RemoteVideoVisiblity = Visibility.Collapsed;
            IncomingCallRingVisibility = Visibility.Collapsed;
            MuteVisiblity = Visibility.Collapsed;
            EndConsultVisibility = Visibility.Collapsed;
            AcceptVisiblity = Visibility.Collapsed;
            UnMuteVisiblity = Visibility.Collapsed;
            PreviewVideoVisiblity = Visibility.Collapsed;
            VideoLogoVisiblity = Visibility.Visible;
        }

        private async Task StartRecordToCustomSink()
        {
            MediaEncodingProfile mep = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Qvga);
            mep.Video.FrameRate.Numerator = 15;
            mep.Video.FrameRate.Denominator = 1;
            mep.Container = null;
            await device.StartRecordingAsync(mep);
        }


    }
}
