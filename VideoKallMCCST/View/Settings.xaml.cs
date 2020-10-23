using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Core;
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
        public Settings()
        {
            this.InitializeComponent();
            TxtimageFolder.Text = "\\\\"+ MainPage.mainPage.SMCCommChannel.IPAddress+"\\" + strRootFolder;
            TxtDataAcq.Text = MainPage.mainPage.isDataAcquitionappConnected ? "Connected" : "Not Connected ";
            
            MainPage.mainPage.DQConnectionCallback += UpdateConnectionStatus;
        }

      async  void UpdateConnectionStatus(bool status)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                TxtDataAcq.Text = MainPage.mainPage.isDataAcquitionappConnected ? "Connected" : "Not Connected ";
            });
            
        }
        private void TxtTmpUnitbtn_Toggled(object sender, RoutedEventArgs e)
        {
            MainPage.mainPage.mainpagecontext.ThermometerUnitF = TxtTmpUnitbtn.IsOn;
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
        }
    }
}
