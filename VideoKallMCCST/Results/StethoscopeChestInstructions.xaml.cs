using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VideoKallMCCST.Communication;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        public StethoscopeChestInstructions()
        {
            this.InitializeComponent();
            MainPage.mainPage.StethoscopeNotification += UpdateNotification;
            TxtInstruction.Text = @"1. Please suggest user to place stethoscope in chest.";
        }

        async void UpdateNotification(string s, int code)
        {
            if(s.ToLower().Contains(("ready for streaming at").ToLower()))
            {
                MainPage.mainPage.isStethoscopeReadystreaming = true;
            }
            else if (s.ToLower().Contains("streaming at") || s.ToLower().Contains("receiving stream"))
            {
                MainPage.mainPage.isStethoscopeStreaming = true;
                MainPage.mainPage.StethoscopeStatus?.Invoke(true);
            }
            else if(s.ToLower().Contains("stopped receiving") || s.ToLower().Contains("streaming stopped"))
            {
                MainPage.mainPage.isStethoscopeStreaming = false;
            }
            else if(s.ToLower().Contains(("Cannot connect to TX failed to connect").ToLower()))
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
            if (!MainPage.mainPage.isStethoscopeStreaming)
                return;

           recordToggle  = !recordToggle;
           
            MainPage.mainPage.StethoscopeRecord?.Invoke();
            BtnRecord.Content = recordToggle ? "Stop Recording" : "Record";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
           

        }


        bool startstoptoggle = false;

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
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

                MainPage.mainPage.isStethoscopeStreaming = false;
            }
        } 

    }
}
