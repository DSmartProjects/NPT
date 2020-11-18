using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoKallMCCST.Model;

namespace VideoKallMCCST.ViewModel
{
    public class VideoCallViewModel:NotifyPropertyChanged
    {
        private Patient _patientDetails = null;
        public Patient PatientDetails { get { return _patientDetails; } set { _patientDetails = value; OnPropertyChanged("PatientDetails"); } }
    }
}
