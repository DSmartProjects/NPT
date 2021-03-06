﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VideoKallMCCST.Communication;
using VideoKallMCCST.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VideoKallMCCST.Results
{
    public sealed partial class StethoscopeChestInstructions : UserControl
    {
        public string targetPath = string.Empty;
        public StethoscopeChestInstructions()
        {
            this.InitializeComponent();
            MainPage.mainPage.StethoscopeNotification += UpdateNotification;
            //TxtInstruction.Text = @"1. The chest piece should be held in your dominant hand, between the index and middle fingers, just above the knuckle. To prevent interfering noise, curl the thumb under the tube to keep it still";
            MainPage.mainPage.CASResult += CasNotification;
        }

        async void CasNotification(string message, int devicecode, int isresultornotificationmsg)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (devicecode == 4 && isresultornotificationmsg == 1)
                {
                    TxtStatus.Text = message;
                }
            });
        }


        async void UpdateNotification(string s, int code)
        {
            if (!MainPage.mainPage.IsStethescopeChest)
                return;

            if (s.ToLower().Contains(("ready for streaming at").ToLower()))
            {
                MainPage.mainPage.isStethoscopeReadystreaming = true;
            }
            else if (s.ToLower().Contains("streaming at") || s.ToLower().Contains("receiving stream"))
            {
                MainPage.mainPage.isStethoscopeStreaming = true;
              
            }
            else if (s.ToLower().Contains("stopped receiving") || s.ToLower().Contains("streaming stopped"))
            {
                if (MainPage.mainPage.isStethoscopeStreaming)
                MainPage.mainPage.StethoscopeStatus?.Invoke(true);

                MainPage.mainPage.isStethoscopeStreaming = false;
            }
            else if (s.ToLower().Contains(("Cannot connect to TX failed to connect").ToLower() )   ||
                s.ToLower().Contains(("Failed").ToLower()))
            {
                MainPage.mainPage.isStethoscopeStreaming = false;
                MainPage.mainPage.isStethoscopeReadystreaming = false;
                MainPage.mainPage.StethoscopeStatus?.Invoke(false);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                   if(startstoptoggle)
                    {
                        BtnStart_Click(null, null);
                    }
                });
            }
            else if (s.ToLower().Contains(("Failed").ToLower()))
            {
                MainPage.mainPage.isStethoscopeStreaming = false;
                MainPage.mainPage.isStethoscopeReadystreaming = false;
                MainPage.mainPage.StethoscopeStatus?.Invoke(false);
            }

            
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                TxtStatus.Text = s;
                BtnStart.IsEnabled = true;
            });
        }

        bool recordToggle = false;
        private void BtnRecord_Click(object sender, RoutedEventArgs e)
        {
            if (!MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployed())
            {
                TxtStatus.Text = Constants.MsgDevicenotDeployed;
                return;
            }

            if (!MainPage.mainPage.isStethoscopeStreaming)
                return;

           recordToggle  = !recordToggle;
           
            MainPage.mainPage.StethoscopeRecord?.Invoke();
            BtnRecord.Content = recordToggle ? "Stop Recording" : "Record";
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //string fileName = @"record.wav";
                //string fileRename = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fileName;
                //targetPath = "\\" + MainPage.VideoCallVM.PatientDetails.ID + "\\Chest Sethescope";
                //MainPage.mainPage.targetpath = targetPath + "\\" + fileRename;
                //var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                //StorageFile audioFile = await localFolder.GetFileAsync(fileName);
                //StorageFolder strRootFolderPath = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");
                //StorageFolder assetsFolder = await strRootFolderPath.CreateFolderAsync("Sethescope", CreationCollisionOption.FailIfExists);
                //await audioFile.MoveAsync(assetsFolder, fileRename);
                //Toast.ShowToast("", "File Moved Successfully to Destination Folder");

                string fileName = @"record.wav";
                string fileRename = MainPage.VideoCallVM.PatientDetails.ID +"_"+DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fileName; ;
                targetPath = "\\" + MainPage.VideoCallVM.PatientDetails.ID + "\\Chest Sethescope";
                MainPage.mainPage.targetpath = targetPath + "\\" + fileRename;
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile audioFile = await localFolder.GetFileAsync(fileName);
                StorageFolder strRootFolderPath = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");
                if (strRootFolderPath.Path != null && strRootFolderPath != null)
                {
                    StorageFolder assetsFolder = null; ;
                    var file = await strRootFolderPath.TryGetItemAsync("Sethescope");
                    if (file == null)
                    {
                        assetsFolder = await strRootFolderPath.CreateFolderAsync("Sethescope", CreationCollisionOption.FailIfExists);
                    }
                    else
                    {
                        assetsFolder = await strRootFolderPath.GetFolderAsync("Sethescope");
                       
                    }

                    await audioFile.MoveAsync(assetsFolder, fileRename);
                    MainPage.mainPage.targetpath = strRootFolderPath.Path + "\\Sethescope"+"\\" + fileRename;
                    Toast.ShowToast("", "File Moved Successfully to Destination Folder");
                }
                else {
                    Toast.ShowToast("", "Please set your SMC destination folder.");
                }

                if (!MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployed())
                {
                    TxtStatus.Text = Constants.MsgDevicenotDeployed;
                    return;
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                Toast.ShowToast("", "Failed to Move the File");
            }

        }


        bool startstoptoggle = false;

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployed())
            {
                TxtStatus.Text = Constants.MsgDevicenotDeployed;
                return;
            }
            if (recordToggle)
                return;

            if (!startstoptoggle && !MainPage.mainPage.isStethoscopeReadystreaming)
                return;

            startstoptoggle    = !startstoptoggle;
            BtnStart.Content = startstoptoggle ? "Stop streaming" : "Start streaming";
            if (startstoptoggle)
            {
                MainPage.mainPage.StethoscopeStartStop.Invoke("startST", 1);

            }
            else
            {
                MainPage.mainPage.StethoscopeStartStop.Invoke("stopST", 1);
                BtnStart.Content = startstoptoggle ? "Stop streaming" : "Start streaming";

                // MainPage.mainPage.isStethoscopeStreaming = false;
                if (BtnStart.Content == "Start streaming")
                {
                    BtnSave.IsEnabled = true;
                }
            }
        } 

    }
}
