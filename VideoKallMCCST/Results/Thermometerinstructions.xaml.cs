using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VideoKallMCCST.Communication;
using VideoKallMCCST.Helpers;
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
    public sealed partial class Thermometerinstructions : UserControl
    {
        public Thermometerinstructions()
        {
            this.InitializeComponent();
            MainPage.mainPage.mainpagecontext.NotifyResult += UpdateNotification;
            BtnCMD.IsEnabled = true;
            //TXTInstrction.Text = @"1. Suggest user to Press temperature button on thermometer," +
            //    $"then Press connect button. ";
            MainPage.mainPage.CASResult += CasNotification;
        }
        async void CasNotification(string message, int devicecode, int isresultornotificationmsg)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (devicecode == 4 && isresultornotificationmsg == 1)
                {
                    TxtConnectionstatus.Text = message;
                }
            });
        }


        async void UpdateNotification(object sender, CommunicationMsg msg)
        {
            if (msg.Id != DeviceResponseType.THERMORESTULTSTATUS)
                return;
            string[] cmd = msg.Msg.ToLower().Split('>');
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                BtnCMD.IsEnabled = true;
                switch (cmd[0])
                {
                    case "thermocon"://DeviceResponseType.THERMORESTULTSTATUS: 

                        TxtConnectionstatus.Text = msg.Msg.Split('>')[1]; 
                        break;
                    
                    case "thermoerror":
                        TxtConnectionstatus.Text = msg.Msg.Split('>')[1];
                        MainPage.mainPage.Thermostatusdelegate?.Invoke(false,0);
                        break;

                    case "thermonotpaired":
                        MainPage.mainPage.Thermostatusdelegate?.Invoke(false,0);
                        TxtConnectionstatus.Text = msg.Msg.Split('>')[1];
                        break;
                }
            });
        }

        private void BtnCMD_Click(object sender, RoutedEventArgs e)
        {
            if (!MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployed())
            {
                TxtConnectionstatus.Text = Constants.MsgDevicenotDeployed;
                return;
            }
            MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.THERMORESULTCMD);
             BtnCMD.IsEnabled = false;


        }
    }
}
