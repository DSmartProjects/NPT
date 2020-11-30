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
    public sealed partial class Resulttemplate : UserControl
    {
        public Resulttemplate()
        {
            this.InitializeComponent();
            MainPage.mainPage.HM_WMEvents += InitializeUI;
            MainPage.mainPage.CASResult += CasNotification;
        }

       async void CasNotification(string message, int devicecode, int isresultornotificationmsg)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if(devicecode == 1 && isresultornotificationmsg==1)
                {
                    TxtStatus.Text = message;
                }
            });
        }

        private async void InitializeUI(string message, int code)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (code == 0)
                {
                   // TXTHMWM.Text = "Weight Measure";
                    BtnHMWM.Content = "Get Weight";
                    RadCM.Visibility = Visibility.Collapsed;
                    RadFt.Visibility = Visibility.Collapsed;
                    RadPound.Visibility = Visibility.Visible;
                    RadKG.Visibility = Visibility.Visible;
                    if (MainPage.mainPage.WeightMeasureUnit == 0)
                    {
                        RadKG.IsChecked = null;
                        RadCM.IsChecked = null;
                        RadFt.IsChecked = null;
                        RadPound.IsChecked = true;
                    }
                    else
                    {
                        
                        RadCM.IsChecked = null;
                        RadFt.IsChecked = null;
                        RadPound.IsChecked = false;
                        RadKG.IsChecked = true;
                    }
                }
                else
                {
                    //TXTHMWM.Text = "Height Measure";
                    BtnHMWM.Content = "Get Height";
                    RadPound.Visibility = Visibility.Collapsed;
                    RadKG.Visibility = Visibility.Collapsed;
                    RadCM.Visibility = Visibility.Visible;
                    RadFt.Visibility = Visibility.Visible;

                    if (MainPage.mainPage.HeightMeasureUnit == 0)
                    {
                        RadPound.IsChecked = null;
                        RadKG.IsChecked = null;
                        RadCM.IsChecked = true;
                        RadFt.IsChecked = null;
                       
                    }
                    else
                    {
                        RadKG.IsChecked = null;
                        RadCM.IsChecked = null;
                        RadFt.IsChecked = true;
                        RadPound.IsChecked = false;
                    }
                }
            });
            
        }

        private void BtnHMWM_Click(object sender, RoutedEventArgs e)
        {
            if (BtnHMWM.Content.ToString().ToLower().Contains("height"))
                MainPage.mainPage.CommToDataAcq.SendMessageToDataacquistionapp(CommunicationCommands.HM);
            else               
            MainPage.mainPage.CommToDataAcq.SendMessageToDataacquistionapp(CommunicationCommands.WM);
        }

        private void RadPound_Checked(object sender, RoutedEventArgs e)
        {
           MainPage.mainPage.WeightMeasureUnit = 0;
        }

        private void RadKG_Checked(object sender, RoutedEventArgs e)
        {
            MainPage.mainPage.WeightMeasureUnit = 1;
        }

        private void RadCM_Checked(object sender, RoutedEventArgs e)
        {
            MainPage.mainPage.HeightMeasureUnit = 0;
        }

        private void RadFt_Checked(object sender, RoutedEventArgs e)
        {
            MainPage.mainPage.HeightMeasureUnit = 1;
        }
    }
}
