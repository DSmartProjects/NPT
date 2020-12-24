using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoKallMCCST.ViewModel;

namespace VideoKallMCCST.Model
{
   public class SeatBackStethoscopeTestResult : NotifyPropertyChanged
    {       
        private int _seatBackStethoscopeTestResultId;
        public int SeatBackStethoscopeTestResultId
        {
            get
            {
                return _seatBackStethoscopeTestResultId;
            }
            set
            {
                _seatBackStethoscopeTestResultId = value;
                OnPropertyChanged("SeatBackStethoscopeTestResultId");
            }
        }
        private int _patientId;

        public int PatientId
        {
            get
            {
                return _patientId;
            }
            set
            {
                _patientId = value;
                OnPropertyChanged("PatientId");
            }
        }
        private int? _chairId;
        public int? ChairId
        {
            get
            {
                return _chairId;
            }
            set
            {
                _chairId = value;
                OnPropertyChanged("ChairId");
            }
        }
        private Guid? _sessionId;
        public Guid? SessionId
        {
            get
            {
                return _sessionId;
            }
            set
            {
                _sessionId = value;
                OnPropertyChanged("SessionId");
            }
        }
        private int? _stethoscopeId;
        public int? StethoscopeId {
            get
            {
                return _stethoscopeId;
            }
            set
            {
                _stethoscopeId = value;
                OnPropertyChanged("StethoscopeId");
            }
        }
        private byte[] _recording;
        public byte[] Recording {
            get
            {
                return _recording;
            }
            set
            {
                _recording = value;
                OnPropertyChanged("Recording");
            }
        }
        private int? _createdBy;
        public int? CreatedBy
        {
            get
            {
                return _createdBy;
            }
            set
            {
                _createdBy = value;
                OnPropertyChanged("CreatedBy");
            }
        }

        private DateTime? _createdDate;
        public DateTime? CreatedDate
        {
            get
            {
                return _createdDate;
            }
            set
            {
                _createdDate = value;
                OnPropertyChanged("CreatedDate");
            }
        }

        private Patient _patient = null;
        public virtual Patient Patient { get { return _patient; } set { _patient = value; OnPropertyChanged("Patient"); } }
    }
}
