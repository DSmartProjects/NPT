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
using VideoKallSBCApplication.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace VideoKallMCCST.View
{
  
    public sealed partial class SearchPatient : ContentDialog
    {
        private PatientViewModel _patientVM = null;
        private Patient patient = null;
        HttpClientManager httpClient = null;
        public SearchPatient()
        {
            this.InitializeComponent();
            _patientVM = new PatientViewModel();
            this.DataContext = _patientVM;
        }

     

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            _patientVM.Patients = new ObservableCollection<Patient>();
            _patientVM.PatientsGridVisibility = Visibility.Collapsed;

        }

        private async void BtnSearchPatient_Click(object sender, RoutedEventArgs e)
        {
            _patientVM.LblSearchNotFoundVisibility = Visibility.Collapsed;
            httpClient = new HttpClientManager();
            string name = txtPatientName.Text.Trim();
            if (!string.IsNullOrWhiteSpace(name))
            {
                _patientVM.Patients = null;
                _patientVM.Patients = new ObservableCollection<Patient>(await httpClient.PatientsAsync(name));               
            }
            else
            {
                PatientsGrid.Visibility = Visibility.Collapsed;
                _patientVM.PatientsGridVisibility = Visibility.Collapsed;
                _patientVM.LblSearchNotFoundVisibility = Visibility.Visible;
            }
           
            if (_patientVM.Patients!=null&&_patientVM.Patients.Count>0&& !string.IsNullOrWhiteSpace(name))
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
        
    }
}
