using GalaSoft.MvvmLight.Command;
using SBCDBModule;
using SBCDBModule.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VideoKallMCCST.Communication;
using VideoKallMCCST.Helpers;
using VideoKallMCCST.Model;
using VideoKallMCCST.View;
using Windows.UI.Xaml;

namespace VideoKallMCCST.ViewModel
{
    class LoginPageViewModel : INotifyPropertyChanged
    {
        Utility utility = null;
        private int _touserId = 0;
        public int TokUserId
        {
            get
            {
                return _touserId;
            }
            set
            {
                _touserId = value;
                OnPropertyChanged("ToUserId");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private ICommand _submitCommand = null;
        public ICommand SubmitCommand {
            get {
                 if(_submitCommand == null)
                    _submitCommand = new RelayCommand(ExecuteSubmitCommand);
                return _submitCommand;
            } 
        }

        private PMMConfiguration _pmmConfig = null;
        public PMMConfiguration PMMConfig { get { return _pmmConfig; } set { _pmmConfig = value; OnPropertyChanged("PMMConfig"); } }

        string _userid = string.Empty;
        public string Userid { 
            get{ return _userid; }
            set { 
                _userid = value;
                OnPropertyChanged("EnableSubmitButton");
            }
        }

        private string _token = string.Empty;
        public string Token { get { return _token; } set { _token = value; OnPropertyChanged("Token"); } }
        string _password=string.Empty;
        public string PasswordTxt {
            get {return _password; }
            set { _password = value;
                OnPropertyChanged("EnableSubmitButton");
            }  }

        public bool EnableSubmitButton { 
            get
            {    if (string.IsNullOrEmpty(PasswordTxt) || string.IsNullOrEmpty(Userid))
                {
                    return false;
                }
               return true;
            } }
        public bool LoginFailedMsg1Visible {get; set;}
        public bool LoginFailedMsg2Visible {get; set;}
        public bool LoginFailedMsg3Visible {get; set;}
        public string LoginErrorMessage { get; set; }
        public string LoginErrorMessage2 { get; set; }
        HttpClientManager _httpClient = null;
        PMMConfiguration _configuration = null;


        public async void ExecuteSubmitCommand( )
        {
            utility = new Utility();
            var pmm_Config = Task.Run(async () => { return await utility.ReadPMMConfigurationFile(); }).Result;
            _httpClient = VideoKallLoginPage.LoginPage.HttpClient;
            _configuration= VideoKallLoginPage.LoginPage._loginVM.PMMConfig;
            bool isLogin = false;
            bool isAdmin = !string.IsNullOrEmpty(Userid) && !string.IsNullOrEmpty(Constants.Admin_PWD) && Userid.Equals(Constants.Admin_UNAME, StringComparison.InvariantCultureIgnoreCase) && PasswordTxt.Equals(Constants.Admin_PWD, StringComparison.InvariantCultureIgnoreCase) ? true : false;

            //_httpClient = VideoKallLoginPage.LoginPage.HttpClient; 
            if (!string.IsNullOrEmpty(_configuration.API_URL)&&!string.IsNullOrEmpty(_configuration.TestResultAPI_URL)&&isAdmin==false)
            {
                _httpClient.basePMM_APIUrl = VideoKallLoginPage.LoginPage._loginVM.PMMConfig?.API_URL;
                _httpClient.base_APIUrl = VideoKallLoginPage.LoginPage._loginVM.PMMConfig?.TestResultAPI_URL;
                 isLogin= await _httpClient.Authenticate(Userid, PasswordTxt);
            }           
            if (isAdmin)
            {
                TokUserId = Constants.Admin_ID;
                VideoKallLoginPage.LoginPage.Frame.Navigate(typeof(MainPage));
            }
            else if (isLogin)
            { 
                VideoKallLoginPage.LoginPage.Frame.Navigate(typeof(MainPage));
            }
            else
            {
                Toast.ShowToast("", Constants.InValid_UNAME_PWD);
                return;
            }

            //SBCDB dbmodule = new SBCDB();
            //User loggedinUser =  dbmodule.GetLoggedinUser(Userid.Trim().ToLower());
            //if (loggedinUser == null)
            //{
            //    LoginFailedMsg1Visible = true;
            //    LoginFailedMsg2Visible = true;
            //    LoginFailedMsg3Visible = true;

            //    LoginErrorMessage = "User name: " + Userid + " not found.";
            //    LoginErrorMessage2 = "Please enter valid user id or contact admin.";
            //    OnPropertyChanged("LoginFailedMsg1Visible");
            //    OnPropertyChanged("LoginFailedMsg2Visible");
            //    OnPropertyChanged("LoginFailedMsg3Visible");
            //    OnPropertyChanged("LoginErrorMessage");
            //    OnPropertyChanged("LoginErrorMessage2");
            //   MainPage.mainPage.IsUserLogedin = false;
            //    return;
            //}

            //if (string.Compare(PasswordTxt, loggedinUser.Password) == 0)
            //{
            //    VideoKallLoginPage.LoginPage.Frame.Navigate(typeof(MainPage));
            //    //MainPage.mainPage.pagePlaceHolder.Navigate(typeof(TestPanelPage));
            //    if (videoCall == null)
            //        videoCall = new VideoCallPage();
            //    MainPage.mainPage.RightPanelHolder.Navigate(typeof(VideoCallPage), videoCall);
            //    MainPage.mainPage.IsUserLogedin = true;
            //}
            //else
            //{
            //    LoginFailedMsg1Visible = true;
            //    LoginFailedMsg2Visible = true;
            //    LoginFailedMsg3Visible = true;
            //    LoginErrorMessage = "Password not matched.";
            //    LoginErrorMessage2 = "Please enter valid password or contact admin.";

            //    OnPropertyChanged("LoginFailedMsg1Visible");
            //    OnPropertyChanged("LoginFailedMsg2Visible");
            //    OnPropertyChanged("LoginFailedMsg3Visible");
            //    OnPropertyChanged("LoginErrorMessage");
            //    OnPropertyChanged("LoginErrorMessage2");
            //    MainPage.mainPage.IsUserLogedin = false;
            //}

        }
        VideoCallPage videoCall = null;
       // TestPanelPageViewModel testPanel = null;
    }//class
}
