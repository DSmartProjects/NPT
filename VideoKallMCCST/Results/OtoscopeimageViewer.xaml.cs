using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VideoKallMCCST.Communication;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VideoKallMCCST.Results
{
    public sealed partial class OtoscopeimageViewer : UserControl
    {
        public OtoscopeimageViewer()
        {
            this.InitializeComponent();

            MainPage.mainPage.ImageDisplay += SetImage;
          //  ImageViewer.Source = null;
        }

        public async void SetImage(String img)
        {
            string[] cmd = img.Split('>');
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
               switch(cmd[0].ToLower())
                {
                    case "mrpic":
                        DisplayImage(ImageName);
                        break;
                }
               
            });
        }
        private void Btndone_Click(object sender, RoutedEventArgs e)
        {
            ImageViewer.Source = null;
            BtnTakePic.IsEnabled = true;
            MainPage.mainPage.OtoscopeComm?.Invoke();
        }

        private void BtnTakePic_Click(object sender, RoutedEventArgs e)
        {
            BtnTakePic.IsEnabled = false;
            MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.TAKEPIC);
           
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SetImagefolder();
           // DisplayImage(ImageName);
        }

       
        string strRootFolder = "VideoKall";
        string strRootFolderPath = @"\\192.168.0.33\";// VideoKall";
        string ImageName = "capturedImage.png";

        private async void SetImagefolder( )
        {
            try
            {
                strRootFolderPath = "\\\\" + MainPage.mainPage.SMCCommChannel.IPAddress + "\\" ;
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(strRootFolderPath + strRootFolder);
              //  StorageFolder folder = await folderPicker.PickSingleFolderAsync();
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


        async void DisplayImage(string imageName)
        {
            try
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(MainPage.mainPage.rootImageFolder.Path );
                StorageFile storageFile = await folder.GetFileAsync(imageName);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    // Set the image source to the selected bitmap
                    BitmapImage bitmapImage = new BitmapImage();
                    if (storageFile != null)
                    {
                        // Ensure the stream is disposed once the image is loaded
                        using (IRandomAccessStream fileStream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                        {
                            await bitmapImage.SetSourceAsync(fileStream);
                        }
                    }
                  //  ShowHideMessage(false);
                    ImageViewer.Source = bitmapImage;
                    BtnTakePic.IsEnabled = true;
                });
            }
            catch (Exception ex)
            {
                MainPage.mainPage.LogExceptions(ex.Message);
                Debug.Print(ex.Message);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    BtnTakePic.IsEnabled = true;
                    //ContentDialog dlg = new ContentDialog();
                    //dlg.Title = "Error Message";
                    //dlg.Content = "Please Browse Image Folder.\n" + "\\\\" + MainPage.mainPage.SMCCommChannel.IPAddress + "\\VideoKall";
                    //dlg.PrimaryButtonText = "YES";
                    //ContentDialogResult res = await dlg.ShowAsync();

                });
            }
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
