using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoKallMCCST.ViewModel;

namespace VideoKallMCCST.Model
{
    public class GlucoseMonitorTestResult : NotifyPropertyChanged
    {
        private int _id;
        public int GlucoseMonitorTestResultId { get { return _id; } set { _id = value; OnPropertyChanged("GlucoseMonitorTestResultId"); } }

        private int _patientId;
        public int PatientId { get { return _patientId; } set { _patientId = value; OnPropertyChanged("PatientId"); } }


        private int? _chairId;
        public int? ChairId { get { return _chairId; } set { _chairId = value; OnPropertyChanged("ChairId"); } }

        private int? _createdBy;
        public int? CreatedBy { get { return _createdBy; } set { _createdBy = value; OnPropertyChanged("CreatedBy"); } }


        private Guid? _sessionId;
        public Guid? SessionId { get { return _sessionId; } set { _sessionId = value; OnPropertyChanged("SessionId"); } }
        private DateTime? _createdDate;
        public DateTime? CreatedDate { get { return _createdDate; } set { _createdDate = value; OnPropertyChanged("CreatedDate"); } }

        private string _mode;
        public string Mode { get { return _mode; } set { _mode = value; OnPropertyChanged("Mode"); } }

    
        private double? _value;
        public double? Value { get { return _value; } set { _value = value; OnPropertyChanged("Value"); } }

        private  Patient _patient=null;
        public virtual Patient Patient { get { return _patient; } set { _patient = value; OnPropertyChanged("Patient"); } }

    }
}
