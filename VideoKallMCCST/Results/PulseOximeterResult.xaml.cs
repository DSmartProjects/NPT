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
    public sealed partial class PulseOximeterResult : UserControl
    {
        public PulseOximeterResult()
        {
            this.InitializeComponent();
            MainPage.mainPage.mainpagecontext.NotifyResult += UpdateNotification;
            BtnStreamdata.IsEnabled = true;
            MainPage.mainPage.CASResult += CasNotification;
        }

        async void CasNotification(string message, int devicecode, int isresultornotificationmsg)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (devicecode == 4 && isresultornotificationmsg == 1)
                {
                    TxtSpiroMeterConnectionStatus.Text = message;
                }
            });
        }

        async void UpdateNotification(object sender, CommunicationMsg msg)
        {
            string status = String.Empty;
            if (msg.Id != DeviceResponseType.PULSEOXIMETERSTATUS)
                return;
            string []cmd = msg.Msg.ToLower().Split('>') ;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                switch (msg.Id)
                {
                    case DeviceResponseType.PULSEOXIMETERSTATUS:
                        BtnStreamdata.IsEnabled = true;
                        status= msg.Msg.Split('>')[1];
                        if (status == "successfully subscribed")
                        {
                            TxtSpiroMeterConnectionStatus.Text = "Successfully Subscribed";
                        }
                        else
                            TxtSpiroMeterConnectionStatus.Text= msg.Msg.Split('>')[1];
                        if (cmd.Length>2 && cmd[2].Equals("error"))
                        {
                            MainPage.mainPage.Thermostatusdelegate?.Invoke(false, 1);
                        }
                        break;

                }
            });
        }
        private void BtnStreamdata_Click(object sender, RoutedEventArgs e)
        {
            MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.SMCPULSEOXIMETERSTART);
            BtnStreamdata.IsEnabled = false;
        }

        private void BtnStreamdataDone_Click(object sender, RoutedEventArgs e)
        {
            //TxtSpiroMeterConnectionStatus
        }
    }
}
