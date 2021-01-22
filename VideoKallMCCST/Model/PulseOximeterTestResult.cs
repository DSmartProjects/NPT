using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoKallMCCST.ViewModel;

namespace VideoKallMCCST.Model
{
    public class PulseOximeterTestResult : NotifyPropertyChanged
    {
        private int _pulseOximeterTestResultId;
        public int PulseOximeterTestResultId
        {
            get
            {
                return _pulseOximeterTestResultId;
            }
            set
            {
                _pulseOximeterTestResultId = value;
                OnPropertyChanged("PulseOximeterTestResultId");
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
        private int? _spO2;
        public int? SpO2
        {
            get
            {
                return _spO2;
            }
            set
            {
                _spO2 = value;
                OnPropertyChanged("SpO2");
            }
        }
        private int? _heartRate;
        public int? HeartRate
        {
            get
            {
                return _heartRate;
            }
            set
            {
                _heartRate = value;
                OnPropertyChanged("HeartRate");
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
