using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoKallMCCST.Model;
using VideoKallSBCApplication.Helpers;
using Windows.UI.Xaml;

namespace VideoKallMCCST.ViewModel
{
    public class PatientViewModel:NotifyPropertyChanged
    {
        private ObservableCollection<Patient> _patients = null;
        public ObservableCollection<Patient> Patients { get { return _patients; } set { _patients = value; OnPropertyChanged("Patients"); } }
        private Visibility _patientsGridVisibility = Visibility.Collapsed;
        public Visibility PatientsGridVisibility { get { return _patientsGridVisibility; } set { _patientsGridVisibility = value; OnPropertyChanged("PatientsGridVisibility"); } }
        private Visibility _lblSearchNotFoundVisibility = Visibility.Collapsed;
        public Visibility LblSearchNotFoundVisibility { get { return _lblSearchNotFoundVisibility; } set { _lblSearchNotFoundVisibility = value; OnPropertyChanged("LblSearchNotFoundVisibility"); } }

        private string _txtSearchNotFound = Constants.MSG_SEARCH_NOT_FOUND;
        public string TxtSearchNotFound { get { return _txtSearchNotFound; } set { _txtSearchNotFound = value; OnPropertyChanged("TxtSearchNotFound"); } }

        //private Visibility _isSelectPatientButtonVisible = Visibility.Collapsed;
        //public Visibility IsSelectPatientButtonVisible { get { return _isSelectPatientButtonVisible; } set { _isSelectPatientButtonVisible = value; OnPropertyChanged("IsSelectPatientButtonVisible"); } }

        private string _pmmURL = string.Empty;
        public string PMM_URL { get { return _pmmURL; } set { _pmmURL = value; OnPropertyChanged("PMM_URL"); } }


    }
}
