using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using VideoKallMCCST.Communication;
using VideoKallMCCST.Model;
using VideoKallMCCST.ViewModel;
using VideoKallMCCST.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.System;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace VideoKallMCCST.View
{
  
    public sealed partial class SearchPatient : ContentDialog
    {
        private PatientViewModel _patientVM = null;
        private Patient patient = null;
        HttpClientManager _httpClient = null;
        PMMConfiguration _configuration = null;
        public SearchPatient()
        {
            this.InitializeComponent();
            _patientVM = new PatientViewModel();
            this.DataContext = _patientVM;
            _httpClient = VideoKallLoginPage.LoginPage.HttpClient;
            _configuration = VideoKallLoginPage.LoginPage._loginVM.PMMConfig;
            _httpClient.basePMM_APIUrl = _configuration?.API_URL;
            _httpClient.base_APIUrl = _configuration?.TestResultAPI_URL;
            _patientVM.PMM_URL = _configuration?.URL;
        }

     

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            _patientVM.Patients = new ObservableCollection<Patient>();
            _patientVM.PatientsGridVisibility = Visibility.Collapsed;

        }

        private async void BtnSearchPatient_Click(object sender, RoutedEventArgs e)
        {
            //_patientVM.LblSearchNotFoundVisibility = Visibility.Collapsed;
            //httpClient = new HttpClientManager();
            //string name = txtPatientName.Text.Trim();
            //if (!string.IsNullOrWhiteSpace(name))
            //{
            //    _patientVM.Patients = null;

            //    var patients = await httpClient.PatientsAsync(name);
            //    if (patients!=null&&patients.Count>0)
            //    {

            //        _patientVM.Patients = new ObservableCollection<Patient>(patients);
            //    }             
            //}
            //else
            //{
            //    PatientsGrid.Visibility = Visibility.Collapsed;
            //    _patientVM.PatientsGridVisibility = Visibility.Collapsed;
            //    _patientVM.LblSearchNotFoundVisibility = Visibility.Visible;
            //}

            //if (_patientVM.Patients!=null&&_patientVM.Patients.Count>0&& !string.IsNullOrWhiteSpace(name))
            //{
            //    PatientsGrid.Visibility = Visibility.Visible;
            //    _patientVM.PatientsGridVisibility = Visibility.Visible;
            //}
            //else
            //{
            //    PatientsGrid.Visibility = Visibility.Collapsed;
            //    _patientVM.PatientsGridVisibility = Visibility.Collapsed;
            //    _patientVM.LblSearchNotFoundVisibility = Visibility.Visible;               
            //}
            BtnSearchPatient();

        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            MainPage.VideoCallVM.PatientDetails = patient;
            MainPage.mainPage.pagePlaceHolder.Navigate(typeof(TestPanelExpander));
        }

        private void GridPatients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            patient=  new Patient();
            patient.ID = ((VideoKallMCCST.Model.Patient)e.AddedItems[0]).ID;
            patient.Name= ((VideoKallMCCST.Model.Patient)e.AddedItems[0]).Name;
            patient.DOB = ((VideoKallMCCST.Model.Patient)e.AddedItems[0]).DOB;
        }
            

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            MainPage.VideoCallVM.PatientDetails = patient;
            if (patient != null && patient.ID > 0 && !string.IsNullOrEmpty(patient.Name))
                MainPage.mainPage.pagePlaceHolder.Navigate(typeof(TestPanelExpander));
            this.Hide();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _patientVM.Patients = new ObservableCollection<Patient>();
            _patientVM.PatientsGridVisibility = Visibility.Collapsed;
             this.Hide();

        }
        private async void BtnAddPatient_Click(object sender, RoutedEventArgs e)
        {
            if (VideoKallLoginPage.LoginPage._loginVM.PMMConfig != null && !string.IsNullOrEmpty(VideoKallLoginPage.LoginPage._loginVM.PMMConfig.URL))
            {
                Uri uri = new Uri(VideoKallLoginPage.LoginPage._loginVM.PMMConfig.URL);
                await Windows.System.Launcher.LaunchUriAsync(uri);
            }

        }

        private void TxtPatientName_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                BtnSearchPatient();
            }

        }

        private async void BtnSearchPatient()
        {
            _patientVM.LblSearchNotFoundVisibility = Visibility.Collapsed;
            string name = txtPatientName.Text.Trim();
            if (!string.IsNullOrWhiteSpace(name))
            {
                _patientVM.Patients = null;

                var patients = await _httpClient.PatientsAsync(name);
                if (patients != null && patients.Count > 0)
                {

                    _patientVM.Patients = new ObservableCollection<Patient>(patients);
                }
            }
            else
            {
                PatientsGrid.Visibility = Visibility.Collapsed;
                _patientVM.PatientsGridVisibility = Visibility.Collapsed;
                _patientVM.LblSearchNotFoundVisibility = Visibility.Visible;
            }

            if (_patientVM.Patients != null && _patientVM.Patients.Count > 0 && !string.IsNullOrWhiteSpace(name))
            {
                PatientsGrid.Visibility = Visibility.Visible;
                _patientVM.PatientsGridVisibility = Visibility.Visible;
            }
            else
            {
                PatientsGrid.Visibility = Visibility.Collapsed;
                _patientVM.PatientsGridVisibility = Visibility.Collapsed;
                _patientVM.LblSearchNotFoundVisibility = Visibility.Visible;
            }


        }
    }
}
