using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using VideoKallMCCST.Communication;
using VideoKallMCCST.Model;
using VideoKallMCCST.View;
using VideoKallMCCST.Helpers;
using Windows.Storage;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace VideoKallMCCST.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {

        #region
        private HeightTestResult _heightResult = null;
        public HeightTestResult heightResult { get { return _heightResult; } set { _heightResult = value; OnPropertyChanged("heightResult"); } }

        private WeightTestResult _weightResult = null;
        public WeightTestResult WeightResult { get { return _weightResult; } set { _weightResult = value; OnPropertyChanged("weightResult"); } }

        private BloodPressureTestResult _bpResult = null;
        public BloodPressureTestResult BpResult { get { return _bpResult; } set { _bpResult = value; OnPropertyChanged("BpResult"); } }

        private PulseOximeterTestResult _pulseResult = null;
        public PulseOximeterTestResult PulseResult { get { return _pulseResult; } set { _pulseResult = value; OnPropertyChanged("PulseResult"); } }

        private ThermometerTestResult _thermoResult = null;
        public ThermometerTestResult ThermoResult { get { return _thermoResult; } set { _thermoResult = value; OnPropertyChanged("ThermoResult"); } }

        private DermatoscopeTestResult _dermoResult = null;
        public DermatoscopeTestResult  DermoResult { get { return _dermoResult; } set { _dermoResult = value; OnPropertyChanged("DermoResult"); } }

        private OtoscopeTestResult _otoResult = null;
        public OtoscopeTestResult OtoResult { get { return _otoResult; } set { _otoResult = value; OnPropertyChanged("OtoResult"); } }

        private SpirometerTestResult _spiroResult = null;
        public SpirometerTestResult SpiroResult { get { return _spiroResult; } set { _spiroResult = value; OnPropertyChanged("SpiroResult"); } }

        private GlucoseMonitorTestResult _glucoResult = null;
        public GlucoseMonitorTestResult GlucoResult { get { return _glucoResult; } set { _glucoResult = value; OnPropertyChanged("GlucoResult"); }}

        private ChestStethoscopeTestResult _chestResult = null;
        public ChestStethoscopeTestResult ChestResult { get { return _chestResult; } set { _chestResult = value; OnPropertyChanged("ChestResult"); } }

        private SeatBackStethoscopeTestResult _seatResult = null;
        public SeatBackStethoscopeTestResult SeatResult { get { return _seatResult; } set { _seatResult = value; OnPropertyChanged("SeatResult"); } }

        private ClinicalNote _clinicalNote = null;
        public ClinicalNote ClinicalNote { get { return _clinicalNote; } set { _clinicalNote = value; OnPropertyChanged("ClinicalNote"); } }

        #endregion
        public StorageFolder appFolder= Windows.Storage.ApplicationData.Current.LocalFolder;       
        public event PropertyChangedEventHandler PropertyChanged;
        public EventHandler<CommunicationMsg> NotifyResult;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool ThermometerUnitF { get; set; } = true;
        public bool ThermometerTempUnitF { get; set; } = true;
        public void UpdateStatus(bool isSBCConnected)
        {
            TxtSBCConnected = isSBCConnected == true ? "Available" : "Not Available";
            IsSMCConnected = isSBCConnected;
            OnPropertyChanged(TxtSBCConnected);

        }
        public bool IsSMCConnected { get; set; }
        public string TxtSBCConnected { get; set; }
        private ICommand _accountCommand = null;
        public ICommand AccountCommand
        {
            get
            {
                if (_accountCommand == null)
                    _accountCommand = new RelayCommand(ExecuteAccountCommand);
                return _accountCommand;
            }
        }
        void ExecuteAccountCommand()
        {
            MainPage.mainPage.pagePlaceHolder.Navigate(typeof(Accounts));
        }

        private ICommand _logoffCommand = null;
        public ICommand LogOffCommand
        {
            get
            {
                if (_logoffCommand == null)
                    _logoffCommand = new RelayCommand(ExecuteLogOffCommand);
                return _logoffCommand;
            }
        }
        public void ExecuteLogOffCommand()
        {
            //MainPage.mainPage.pagePlaceHolder.Navigate(typeof(LogoPage));
            //MainPage.mainPage.RightPanelHolder.Navigate(typeof(LoginPage));
            Frame rootFrame = Window.Current.Content as Frame;
            Window.Current.Content = rootFrame;
            MainPage.mainPage.IsUserLogedin = false;
            rootFrame.Navigate(typeof(VideoKallLoginPage));

        }
        //Settings
        private ICommand _settingsCommand = null;
        public ICommand SettingsCommand
        {
            get
            {
                if (_settingsCommand == null)
                    _settingsCommand = new RelayCommand(ExecuteSettingsCommand);
                return _settingsCommand;
            }
        }
        public void ExecuteSettingsCommand()
        {
            if (!MainPage.mainPage.TestIsInProgress)
                MainPage.mainPage.pagePlaceHolder.Navigate(typeof(Settings));
        }

        private ICommand _browserCommand = null;
        public ICommand BrowserCommand
        {
            get
            {
                if (_browserCommand == null)
                    _browserCommand = new RelayCommand(ExecuteBrowserCommand);
                return _browserCommand;
            }
        }

        ICommand _saveIpaddress = null;
        public ICommand SaveIPAddress
        {
            get
            {
                if (_saveIpaddress == null)
                    _saveIpaddress = new RelayCommand(ExecuteSaveIPAddress);
                return _saveIpaddress;
            }
        }

        public string TxtProtNo
        {
            get;
            set;
        }
        public string TxtIpAddress
        {
            get;
            set;
        }

        private PMMConfiguration _pmmConfig = null;
        public PMMConfiguration PMMConfig { get { return _pmmConfig; } set { _pmmConfig = value; } }

        private Visibility _req_MSG_Visibility = Visibility.Collapsed;
        public Visibility REQ_MSG_Visibility { get { return _req_MSG_Visibility; } set { _req_MSG_Visibility = value; } }



        public void UpadateIPaddress(string Ip, string Port)
        {
            TxtIpAddress = Ip;
            TxtProtNo = Port;
            OnPropertyChanged(TxtIpAddress);
            OnPropertyChanged(TxtProtNo);
        }
        async void ExecuteSaveIPAddress()
        {
            PMMConfig = VideoKallLoginPage.LoginPage._loginVM.PMMConfig;
            //// Create the message dialog and set its content
            //var messageDialog = new MessageDialog("Do you want to save the details");

            //// Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            //messageDialog.Commands.Add(new UICommand(
            //    "Proceed",
            //    new UICommandInvokedHandler(this.ProceedCommandInvokedHandler)));
            //messageDialog.Commands.Add(new UICommand(
            //    "Cancel",
            //    new UICommandInvokedHandler(this.CancelCommandInvokedHandler)));

            //// Set the command that will be invoked by default
            //messageDialog.DefaultCommandIndex = 0;

            //// Set the command to be invoked when escape is pressed
            //messageDialog.CancelCommandIndex = 1;

            //// Show the message dialog
            //await messageDialog.ShowAsync();
            REQ_MSG_Visibility = Visibility.Collapsed;
            var isMandatoryFieldValuesFilled = (!string.IsNullOrEmpty(TxtIpAddress)
                                   && !string.IsNullOrEmpty(TxtProtNo)
                                   && !string.IsNullOrEmpty(PMMConfig?.URL)
                                   && !string.IsNullOrEmpty(PMMConfig?.API_URL)
                                   && !string.IsNullOrEmpty(PMMConfig?.TestResultAPI_URL));
            if (!isMandatoryFieldValuesFilled)
            {
                MainPage.mainPage.REQ_MSG_VisibilityCompleted?.Invoke(Constants.Failure);
                return;
            }



            if (string.IsNullOrEmpty(TxtIpAddress) || string.IsNullOrEmpty(TxtProtNo))
                return;

            MainPage.mainPage.SMCCommChannel.IPAddress = TxtIpAddress.Trim();
            MainPage.mainPage.SMCCommChannel.PortNo = TxtProtNo.Trim();
            MainPage.mainPage.mainpagecontext.ThermometerUnitF = MainPage.mainPage.mainpagecontext.ThermometerTempUnitF;

            try
            {
                string msg = "IP" + ":" + TxtIpAddress.Trim() + Environment.NewLine +
                   "PORT:" + TxtProtNo.Trim() + Environment.NewLine +
                   "TEMP:" + (MainPage.mainPage.mainpagecontext.ThermometerUnitF ? "1" : "0");
                // msg = Environment.NewLine + msg + Environment.NewLine;
                string filename = "SMCIPAddress.txt";
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile pinfofile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
                //  await Windows.Storage.FileIO.AppendTextAsync(pinfofile, msg, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                await Windows.Storage.FileIO.WriteTextAsync(pinfofile, msg, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
            catch (Exception)
            {
            }

            MainPage.mainPage.SaveSTConfig?.Invoke();
            Write_PMM_ConfigFile();

            isMandatoryFieldValuesFilled = (!string.IsNullOrEmpty(TxtIpAddress)
                                          && !string.IsNullOrEmpty(TxtProtNo)
                                          && !string.IsNullOrEmpty(PMMConfig?.URL)
                                          && !string.IsNullOrEmpty(PMMConfig?.API_URL)
                                          && !string.IsNullOrEmpty(PMMConfig?.TestResultAPI_URL));
            if (isMandatoryFieldValuesFilled)
            {
                MainPage.mainPage.REQ_MSG_VisibilityCompleted?.Invoke(Constants.Success);
                MainPage.mainPage.mainpagecontext.REQ_MSG_Visibility = Visibility.Collapsed;
                Toast.ShowToast("", "Successfully saved.");
                //var messageDialog = new MessageDialog("Successfully saved.");
                //messageDialog.Commands.Add(new UICommand("OK", new UICommandInvokedHandler(this.CancelCommandInvokedHandler)));
                //// Set the command that will be invoked by default
                //messageDialog.DefaultCommandIndex = 0;
                //await messageDialog.ShowAsync();
                ExecuteSaveNavigate();

            }

        }

        private void CancelCommandInvokedHandler(IUICommand command)
        {
            return;
        }

        private async void ProceedCommandInvokedHandler(IUICommand command)
        {
            REQ_MSG_Visibility = Visibility.Collapsed;
            var isMandatoryFieldValuesFilled = (!string.IsNullOrEmpty(TxtIpAddress)
                                   && !string.IsNullOrEmpty(TxtProtNo)
                                   && !string.IsNullOrEmpty(PMMConfig?.URL)
                                   && !string.IsNullOrEmpty(PMMConfig?.API_URL)
                                   && !string.IsNullOrEmpty(PMMConfig?.TestResultAPI_URL));
            if (!isMandatoryFieldValuesFilled)
            {
                MainPage.mainPage.REQ_MSG_VisibilityCompleted?.Invoke(Constants.Failure);
                return;
            }



            if (string.IsNullOrEmpty(TxtIpAddress) || string.IsNullOrEmpty(TxtProtNo))
                return;

            MainPage.mainPage.SMCCommChannel.IPAddress = TxtIpAddress.Trim();
            MainPage.mainPage.SMCCommChannel.PortNo = TxtProtNo.Trim();
            MainPage.mainPage.mainpagecontext.ThermometerUnitF = MainPage.mainPage.mainpagecontext.ThermometerTempUnitF;

            try
            {
                string msg = "IP" + ":" + TxtIpAddress.Trim() + Environment.NewLine +
                   "PORT:" + TxtProtNo.Trim() + Environment.NewLine +
                   "TEMP:" + (MainPage.mainPage.mainpagecontext.ThermometerUnitF ? "1" : "0");
                // msg = Environment.NewLine + msg + Environment.NewLine;
                string filename = "SMCIPAddress.txt";
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile pinfofile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
                //  await Windows.Storage.FileIO.AppendTextAsync(pinfofile, msg, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                await Windows.Storage.FileIO.WriteTextAsync(pinfofile, msg, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
            catch (Exception)
            {
            }

            MainPage.mainPage.SaveSTConfig?.Invoke();
            Write_PMM_ConfigFile();

            isMandatoryFieldValuesFilled = (!string.IsNullOrEmpty(TxtIpAddress)
                                          && !string.IsNullOrEmpty(TxtProtNo)
                                          && !string.IsNullOrEmpty(PMMConfig?.URL)
                                          && !string.IsNullOrEmpty(PMMConfig?.API_URL)
                                          && !string.IsNullOrEmpty(PMMConfig?.TestResultAPI_URL));
            if (isMandatoryFieldValuesFilled)
            {
                MainPage.mainPage.REQ_MSG_VisibilityCompleted?.Invoke(Constants.Success);
                MainPage.mainPage.mainpagecontext.REQ_MSG_Visibility = Visibility.Collapsed;
                Toast.ShowToast("","Successfully saved.");
                //var messageDialog = new MessageDialog("Successfully saved.");
                //messageDialog.Commands.Add(new UICommand("OK", new UICommandInvokedHandler(this.CancelCommandInvokedHandler)));
                //// Set the command that will be invoked by default
                //messageDialog.DefaultCommandIndex = 0;
                //await messageDialog.ShowAsync();
                ExecuteSaveNavigate();
               
            }
        }


     


        async void Write_PMM_ConfigFile()
        {
            try
            {
                string pmm_Config_FileName = "PMM_Config.txt";
                string msg = "URL :" + " " + PMMConfig.URL + Environment.NewLine + "API_URL :" + " " + PMMConfig.API_URL + Environment.NewLine + "TestResult_API_URL :" + " " + PMMConfig.TestResultAPI_URL;
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile pinfofile = await localFolder.CreateFileAsync(pmm_Config_FileName, CreationCollisionOption.OpenIfExists);
                await Windows.Storage.FileIO.WriteTextAsync(pinfofile, msg, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
            catch (Exception)
            {

            }
        }
       

        public async void ExecuteBrowserCommand()
        {
             appFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            await Launcher.LaunchFolderAsync(appFolder);
        }
        ///
        private ICommand _Done = null;
        public ICommand Done
        {
            get
            {
                if (_Done == null)
                    _Done = new RelayCommand(ExecuteDoneCommand);
                return _Done;
            }
        }
        public void ExecuteDoneCommand()
        {
            if (!MainPage.mainPage.IsUserLogedin)
                MainPage.mainPage.pagePlaceHolder.Navigate(typeof(TestPanelExpander));
            else
                MainPage.mainPage.pagePlaceHolder.Navigate(typeof(TestPanelExpander));
        }

        private ICommand _consultation = null;
        public ICommand Consultation
        {
            get
            {
                if (_consultation == null)
                    _consultation = new RelayCommand(ExecuteConsultationCommand);
                return _consultation;
            }
        }
        public void ExecuteConsultationCommand()
        {
            MainPage.mainPage.pagePlaceHolder.Navigate(typeof(TestPanelExpander));
        }
        //public void ExecuteSaveNavigate()
        //{​​​​​​​
        //    if (!MainPage.mainPage.IsUserLogedin)
        //        MainPage.mainPage.pagePlaceHolder.Navigate(typeof(LogoPage));
        //    else
        //        MainPage.mainPage.pagePlaceHolder.Navigate(typeof(TestPanelExpander));
        //}​​​​
        public async void ExecuteSaveNavigate()
        {

            if (!MainPage.mainPage.IsUserLogedin)
            {
                MainPage.mainPage.pagePlaceHolder.Navigate(typeof(LogoPage));
            }
            else
            {
                MainPage.mainPage.pagePlaceHolder.Navigate(typeof(TestPanelExpander));
            }

        }

        private ICommand _logOut = null;
        public ICommand LogoutCommand
        {
            get
            {
                if (_logOut == null)
                    _logOut = new RelayCommand(ExecuteLogoutCommand);
                return _logOut;
            }
        }
        public void ExecuteLogoutCommand()
        {
            //MainPage.mainPage.Frame.Navigate(typeof(VideoKallLoginPage));
            MainPage.mainPage.VideoCallReset?.Invoke(true);
            Frame rootFrame = Window.Current.Content as Frame;
            Window.Current.Content = rootFrame;
            MainPage.mainPage.IsUserLogedin = false;
            rootFrame.BackStack.Clear();           
            rootFrame.Navigate(typeof(VideoKallLoginPage));
        }
        

    }//class
}
