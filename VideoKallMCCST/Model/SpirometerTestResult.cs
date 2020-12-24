using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoKallMCCST.ViewModel;

namespace VideoKallMCCST.Model
{
    public class SpirometerTestResult:NotifyPropertyChanged
    {     
        private int _spirometerTestResultId;
        public int SpirometerTestResultId
        {
            get
            {
                return _spirometerTestResultId;
            }
            set
            {
                _spirometerTestResultId = value;
                OnPropertyChanged("SpirometerTestResultId");
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
        private string _testType;
        public string TestType {
            get
            {
                return _testType;
            }
            set
            {
                _testType = value;
                OnPropertyChanged("TestType");
            }
        }
        private string _code;
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
                OnPropertyChanged("Code");
            }
        }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        private string _measuredUnit;
        public string MeasuredUnit
        {
            get
            {
                return _measuredUnit;
            }
            set
            {
                _measuredUnit = value;
                OnPropertyChanged("MeasuredUnit");
            }
        }
        private double? _measuredValue;
        public double? MeasuredValue
        {
            get
            {
                return _measuredValue;
            }
            set
            {
                _measuredValue = value;
                OnPropertyChanged("MeasuredValue");
            }
        }
        private string _parameterType;
        public string ParameterType
        {
            get
            {
                return _parameterType;
            }
            set
            {
                _parameterType = value;
                OnPropertyChanged("ParameterType");
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
