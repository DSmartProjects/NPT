using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using VideoKallMCCST.Communication;
using VideoKallSBCApplication.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace VideoKallMCCST.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        StConfig Dfaultsettings;
        StConfig StSettings;
        string StConfigFile = "ST_MCC_Config.txt";
        public Settings()
        {
            this.InitializeComponent();
            var mainpageContext = MainPage.mainPage.mainpagecontext;
            MainPage.mainPage.SaveSTConfig += SaveConfig;
            MainPage.mainPage.REQ_MSG_VisibilityCompleted += REQ_MSG_Visibility;
            TxtimageFolder.Text = "\\\\"+ MainPage.mainPage.SMCCommChannel.IPAddress+"\\" + strRootFolder;
            TxtDataAcq.Text = MainPage.mainPage.isDataAcquitionappConnected ? "Connected" : "Not Connected ";
            if(MainPage.mainPage.isDataAcquitionappConnected)
                TxtDataAcq.Foreground= GetColorFromHexa("#34CBA8");
            else
                TxtDataAcq.Foreground = GetColorFromHexa("#ED604A");

            MainPage.mainPage.DQConnectionCallback += UpdateConnectionStatus;
            TxtTmpUnitbtn.IsOn = MainPage.mainPage.mainpagecontext.ThermometerUnitF;
            Dfaultsettings.IP = MainPage.mainPage.SMCCommChannel.IPAddress;
            Dfaultsettings.PORT = "8445";
            Dfaultsettings.USERNAME = "rnk";
            Dfaultsettings.PASSWORD = "test";
            Dfaultsettings.CUTOFFFILTER = "0";
            Dfaultsettings.CUTOFFFILTERLUNGS = "-259"; 
            Dfaultsettings.GAIN = "0";
            Dfaultsettings.CODEC = ""; 

            StSettings.IP = MainPage.mainPage.SMCCommChannel.IPAddress;
            StSettings.PORT = "8445";
            StSettings.USERNAME = "rnk";
            StSettings.PASSWORD = "test";
            StSettings.CUTOFFFILTER = "0";
            StSettings.CUTOFFFILTERLUNGS = "-259"; 
            StSettings.GAIN = "0";
            StSettings.CODEC = "";

            //IP: 192.168.0.33
            //PORT: 8445
            //USERNAME: rnk
            // PASSWORD:test
            // CUTOFF:No filter
            //FILTER - CUTOFF - LUNGS:-250
            //GAIN: 0
            //CODEC: ""
        }

        private async void REQ_MSG_Visibility(string msg)
        {
           
            if (msg.Equals(Constants.Failure,StringComparison.InvariantCultureIgnoreCase))
            {
                MainPage.mainPage.mainpagecontext.REQ_MSG_Visibility = Visibility.Visible;
                tblRequireMsg.Visibility = Visibility.Visible;
            }
            if (msg.Equals(Constants.Success, StringComparison.InvariantCultureIgnoreCase))
            {
                MainPage.mainPage.mainpagecontext.REQ_MSG_Visibility = Visibility.Collapsed;
                tblRequireMsg.Visibility = Visibility.Collapsed;
                var messageDialog = new MessageDialog("Successfully saved.");
                messageDialog.Commands.Add(new UICommand(
                    "Ok",
                    new UICommandInvokedHandler(this.OkCommandInvokedHandler)));
                messageDialog.DefaultCommandIndex = 0;
                messageDialog.CancelCommandIndex = 1;
                // Show the message dialog
                await messageDialog.ShowAsync();
            }
        }

        private async void OkCommandInvokedHandler(IUICommand command)
        {
            MainPage.mainPage.mainpagecontext.ExecuteSaveNavigate();
        }
        public SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            return new SolidColorBrush(
                Color.FromArgb(
                    255,
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16)
                )
            );
        }

        async  void UpdateConnectionStatus(bool status)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                TxtDataAcq.Text = MainPage.mainPage.isDataAcquitionappConnected ? "Connected" : "Not Connected ";
            });
            if (MainPage.mainPage.isDataAcquitionappConnected)
                TxtDataAcq.Foreground = GetColorFromHexa("#34CBA8");
            else
                TxtDataAcq.Foreground = GetColorFromHexa("#ED604A");

        }
        private void TxtTmpUnitbtn_Toggled(object sender, RoutedEventArgs e)
        {
            MainPage.mainPage.mainpagecontext.ThermometerTempUnitF = TxtTmpUnitbtn.IsOn;
        }

        private void TxtTmpUnitbtn_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

      //  private StorageFolder rootFolder;
        string strRootFolder = "VideoKall";
        string strRootFolderPath = "";//@"\\192.168.0.17\VideoKall";
       // string ImageName = "capturedImage.png";
        private async void BtnBrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               
                FolderPicker folderPicker = new FolderPicker();
                folderPicker.FileTypeFilter.Add(".png");

                // folderPicker.SuggestedStartLocation   =  . strRootFolderPath + "\\" + strRootFolder;

                if (string.IsNullOrEmpty(strRootFolderPath))
                    folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;

                StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                }
                // StorageFolder newFolder;

                MainPage.mainPage.rootImageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");
                strRootFolderPath = MainPage.mainPage.rootImageFolder.Path;
                //  TxtImageFolder = "";


            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

        private void BtnConnectdaq_Click(object sender, RoutedEventArgs e)
        {
            MainPage.mainPage.isDataAcquitionappConnected = false;
            MainPage.mainPage.CommToDataAcq.SendMessageToDataacquistionapp("ConnectionTest");
             TxtDataAcq.Text = MainPage.mainPage.isDataAcquitionappConnected ? "Connected" : "Not Connected ";
            if (MainPage.mainPage.isDataAcquitionappConnected)
                TxtDataAcq.Foreground = GetColorFromHexa("#34CBA8");
            else
                TxtDataAcq.Foreground = GetColorFromHexa("#ED604A");
        }

        private void TxtFilterlungs_TextChanged(object sender, TextChangedEventArgs e)
        {
            string val = TxtFilterlungs.Text;
            if (val.Contains("-"))
            {
                string txt = val.Substring(1);
                if (!txt.All(char.IsDigit))
                    TxtFilterlungs.Text = "";
            }
            else
            {
                if (!TxtFilterlungs.Text.All(char.IsDigit))
                    TxtFilterlungs.Text = "";
            }
        }

        private void TxtFilterHeart_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!TxtFilterHeart.Text.All(char.IsDigit))
                TxtFilterHeart.Text = "";
        }

       async void ReadSTConfigFile()
        {
            try
            {
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile IpAddressFile = await localFolder.GetFileAsync(StConfigFile);
                var alltext = await Windows.Storage.FileIO.ReadLinesAsync(IpAddressFile);
                foreach (var line in alltext)
                {
                    string[] data = line.Split(':');
                    switch (data[0])
                    {
                        case "IP":
                            StSettings.IP = data[1];
                            break;
                        case "PORT":// 8445
                            StSettings.PORT = data[1];
                            break;
                        case "USERNAME":
                            StSettings.USERNAME = data[1];
                            break;
                        case "PASSWORD":
                            StSettings.PASSWORD = data[1];
                            break;
                        case "CUTOFF":
                            StSettings.CUTOFFFILTER = data[1];
                            break;
                        case "FILTER-CUTOFF-LUNGS":
                            StSettings.CUTOFFFILTERLUNGS = data[1];
                            break; 
                        case "GAIN":
                            StSettings.GAIN = data[1];
                            break;
                        case "CODEC":
                            StSettings.CODEC = data[1];
                            break; 

                    }
                }
            }
            catch (Exception) { }
            TxtFilterHeart.Text = StSettings.CUTOFFFILTER.Contains("No filter")?"0": StSettings.CUTOFFFILTER.Trim();
            TxtFilterlungs.Text = StSettings.CUTOFFFILTERLUNGS.Trim();
        }
      async  void WriteSTConfigFile()
        {
            try
            {
                StSettings.CUTOFFFILTER = TxtFilterHeart.Text.Equals("0")? "No filter": TxtFilterHeart.Text.Trim();
                StSettings.CUTOFFFILTERLUNGS = TxtFilterlungs.Text.Trim();

                string msg = Environment.NewLine+ "IP:" + StSettings.IP.Trim() + Environment.NewLine +
                             "PORT" + ":" + StSettings.PORT.Trim() + Environment.NewLine +
                             "USERNAME" + ":" + StSettings.USERNAME.Trim() + Environment.NewLine +
                             "PASSWORD" + ":" + StSettings.PASSWORD.Trim() + Environment.NewLine +
                             "CUTOFF" + ":" + StSettings.CUTOFFFILTER + Environment.NewLine +
                             "FILTER-CUTOFF-LUNGS" + ":" + StSettings.CUTOFFFILTERLUNGS + Environment.NewLine +
                             "GAIN" + ":" + StSettings.GAIN + Environment.NewLine +
                             "CODEC" + ":" + StSettings.CODEC ;
                             
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile pinfofile = await localFolder.CreateFileAsync(StConfigFile, CreationCollisionOption.OpenIfExists);
                await Windows.Storage.FileIO.WriteTextAsync(pinfofile, msg, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
            catch (Exception)
            { }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Utility ut = new Utility();
            var pmm_Config = Task.Run(async () => { return await ut.ReadPMMConfigurationFile(); }).Result;
            ReadSTConfigFile();
        }

        private void SaveConfig()
        {
            WriteSTConfigFile();
        }
             
        private void TxtPMM_URL_TextChanged(object sender, TextChangedEventArgs e)
        {
            MainPage.mainPage.mainpagecontext.PMMConfig.URL = txtPMM_URL.Text;
        }

        private void TxtPMM_API_URL_TextChanged(object sender, TextChangedEventArgs e)
        {
            MainPage.mainPage.mainpagecontext.PMMConfig.API_URL = txtPMM_API_URL.Text;
        }        

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var mainpageContext = MainPage.mainPage.mainpagecontext;
            mainpageContext.TxtIpAddress = TxtIPaddressCtrl.Text.Trim();
            mainpageContext.TxtProtNo = TxtPortNoCtrl.Text.Trim();
            mainpageContext.PMMConfig.URL = txtPMM_URL.Text.Trim();
            mainpageContext.PMMConfig.API_URL = txtPMM_API_URL.Text.Trim();                        
        }
    }

    struct StConfig
    {
        public string IP;
        public string PORT;//:8445
        public string USERNAME;//:rnk
        public string PASSWORD;//:test
        public string CUTOFFFILTER;//:0
        public string CUTOFFFILTERLUNGS;//:-259 
        public string GAIN;//:0
        public string CODEC;//:"" 
    }
}
