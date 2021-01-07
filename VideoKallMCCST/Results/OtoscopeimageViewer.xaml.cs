using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using VideoKallMCCST.Communication;
using VideoKallMCCST.Helpers;
using VideoKallMCCST.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
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
        BitmapImage bitmapImage = new BitmapImage();
        WriteableBitmap bitMap = null;
      public  byte[] buffer = null;
        public OtoscopeimageViewer()
        {
            this.InitializeComponent();
            
            MainPage.mainPage.ImageDisplay += SetImage;
            MainPage.mainPage.DigitalMicroscope += InilializeMicroscope;
            //  ImageViewer.Source = null;
            MainPage.mainPage.CASResult += CasNotification;
        }
        bool isDermascope = false;
        async void CasNotification(string message, int devicecode, int isresultornotificationmsg)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (devicecode == 4 && isresultornotificationmsg == 1)
                {
                    StStatus.Text = message;
                }
            });
        }
        async void InilializeMicroscope(bool isdermo)
        {
            isDermascope = isdermo;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                ImageViewer.Source = null;
                //if (isDermascope)
                //    TxTHeader.Text = "Dermascope";
                //else
                //    TxTHeader.Text = "Otoscope";

                BtnSave.IsEnabled = true;
                BtnTakePic.IsEnabled = true;
            });


        }

        public async void SetImage(String img)
        {
            string[] cmd = img.Split('>');
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                switch (cmd[0].ToLower())
                {
                    case "mrpic":
                        DisplayImage(ImageName);
                        break;
                    case "imagesaved":
                        BtnSave.IsEnabled = true;
                        break;
                }

            });
        }
        private async void Btndone_Click(object sender, RoutedEventArgs e)
        {
            if (!MainPage.mainPage.PoddeployretractcmdStatus.isPodRetracted())
            {
                if (!await ShowPodnotRetractedMessage())
                    return;
            }

            if (!isDermascope)
                MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.STOPOTOSCOPE);
            else
                MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.STOPDERMO);

            ImageViewer.Source = null;
            BtnTakePic.IsEnabled = true;
            MainPage.mainPage.OtoscopeComm?.Invoke();
        }

        private void BtnTakePic_Click(object sender, RoutedEventArgs e)
        {
            if (!MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployed())
            {
                StStatus.Text = Constants.MsgDevicenotDeployed;
                return;
            }

            BtnTakePic.IsEnabled = false;
            MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.TAKEPIC);

        }

        private  void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployed())
            {
                StStatus.Text = Constants.MsgDevicenotDeployed;
                return;
            }
            // SetImagefolder();
            // DisplayImage(ImageName);           

            if (!isDermascope)
            {              
                MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.OTOSAVEIMAGE);
                Toast.ShowToast("", "Successfully Saved");
            }
            else
            {
                MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.DERSAVEIMAGE);
                Toast.ShowToast("", "Successfully Saved");
            }
        
            BtnSave.IsEnabled = false;           
        }


        string strRootFolder = "VideoKall";
        string strRootFolderPath = @"\\192.168.0.33\";// VideoKall";
       public string ImageName = "capturedImage.png";

        private async void SetImagefolder()
        {
            try
            {
                strRootFolderPath = "\\\\" + MainPage.mainPage.SMCCommChannel.IPAddress + "\\";
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
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(MainPage.mainPage.rootImageFolder.Path);
                StorageFile storageFile = await folder.GetFileAsync(imageName);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    // Set the image source to the selected bitmap
                    
                    if (storageFile != null)
                    {
                        // Ensure the stream is disposed once the image is loaded
                        using (IRandomAccessStream fileStream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                        {
                            //bitMap = new WriteableBitmap(bitmapImage.PixelWidth, bitmapImage.PixelHeight);
                            await bitmapImage.SetSourceAsync(fileStream);
                            Convert(fileStream);
                        }
                    }
                    //  ShowHideMessage(false);
                    ImageViewer.Source = bitmapImage;
                    //ImageToByeArray(bitMap);
                  
                    BtnTakePic.IsEnabled = true;
                                     
                });
                MainPage.mainPage.MicroscopeStatus?.Invoke(true);
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
                    MainPage.mainPage.NotifyStatusCallback(ex.Message, 0);
                    MainPage.mainPage.MicroscopeStatus?.Invoke(false);
                });
            }
        }

        private byte[] ImageToByeArray(WriteableBitmap bitMap)
        {           
            using (Stream stream = bitMap.PixelBuffer.AsStream())
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                buffer = memoryStream.ToArray();
            }
            return buffer;
        }

        async Task<byte[]> Convert(IRandomAccessStream s)
        {
            var dr = new DataReader(s.GetInputStreamAt(0));
            buffer = new byte[s.Size];
            await dr.LoadAsync((uint)s.Size);
            dr.ReadBytes(buffer);
            return buffer;
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {

        }

        ContentDialog PodIsnotRetractedMsgdlg = null;
        async Task<bool> ShowPodnotRetractedMessage()
        {
            PodIsnotRetractedMsgdlg = new ContentDialog
            {
                Title = "Device is not retracted.",
                Content = Constants.MsgPodNotRetracted,
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No"
            };
            //PodIsnotRetractedMsgdlg.PrimaryButtonStyle = (Style)this.Resources["PurpleStyle"];
            var val = await PodIsnotRetractedMsgdlg.ShowAsync();
            if (val == ContentDialogResult.Primary)
                return true;
            else
                return false;
        }

    }
}
