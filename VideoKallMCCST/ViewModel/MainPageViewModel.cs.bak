using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VideoKallMCCST.Communication;
using VideoKallMCCST.View;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;

namespace VideoKallMCCST.ViewModel
{
  public  class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public EventHandler<CommunicationMsg> NotifyResult;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool ThermometerUnitF { get; set; } = true;
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
            MainPage.mainPage.pagePlaceHolder.Navigate(typeof(Accounts) );
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
             MainPage.mainPage.pagePlaceHolder.Navigate(typeof(LogoPage));
             MainPage.mainPage.RightPanelHolder.Navigate(typeof(LoginPage));
            MainPage.mainPage.IsUserLogedin = false;

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
            if(!MainPage.mainPage.TestIsInProgress)
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

        public string TxtProtNo { get;
            set; }
        public string TxtIpAddress { get;
            set; }

        public void UpadateIPaddress(string Ip, string Port)
        {
            TxtIpAddress = Ip;
            TxtProtNo = Port;
            OnPropertyChanged(TxtIpAddress);
            OnPropertyChanged(TxtProtNo);
        }
      async  void ExecuteSaveIPAddress()
        {
            if (string.IsNullOrEmpty(TxtIpAddress) || string.IsNullOrEmpty(TxtProtNo))
                return;

            MainPage.mainPage.SMCCommChannel.IPAddress = TxtIpAddress.Trim();
            MainPage.mainPage.SMCCommChannel.PortNo = TxtProtNo.Trim();
            try
            {
                 string msg = "IP" + ":" + TxtIpAddress.Trim()+ Environment.NewLine + "PORT:" + TxtProtNo.Trim() + Environment.NewLine;
               // msg = Environment.NewLine + msg + Environment.NewLine;
                string filename = "SMCIPAddress.txt";
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile pinfofile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
                //  await Windows.Storage.FileIO.AppendTextAsync(pinfofile, msg, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                await Windows.Storage.FileIO.WriteTextAsync(pinfofile, msg, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
            catch (Exception)
            { }
        }
     
        public async void ExecuteBrowserCommand()
        {
            StorageFolder appFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
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
        public  void ExecuteDoneCommand()
        {
            if (!MainPage.mainPage.IsUserLogedin)
                MainPage.mainPage.pagePlaceHolder.Navigate(typeof(LogoPage));
            else
                MainPage.mainPage.pagePlaceHolder.Navigate(typeof(TestPanel));
        }


    }//class
}
