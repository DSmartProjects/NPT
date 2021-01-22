using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoKallMCCST.ViewModel;

namespace VideoKallMCCST.Model
{
    public class ChestStethoscopeTestResult : NotifyPropertyChanged
    {      
        private int _chestStethoscopeTestResultId;
        public int ChestStethoscopeTestResultId
        {
            get
            {
                return _chestStethoscopeTestResultId;
            }
            set
            {
                _chestStethoscopeTestResultId = value;
                OnPropertyChanged("ChestStethoscopeTestResultId");
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
        private byte[] _recording;
        public byte[] Recording
        {
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
        private string _recording_Path;
        public string Recording_Path
        {
            get
            {
                return _recording_Path;
            }
            set
            {
                _recording_Path = value;
                OnPropertyChanged("Recording_Path");
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
