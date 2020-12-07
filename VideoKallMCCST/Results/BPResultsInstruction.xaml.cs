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
    public sealed partial class BPResultsInstructions : UserControl
    {
        public BPResultsInstructions()
        {
            this.InitializeComponent();
            MainPage.mainPage.mainpagecontext.NotifyResult += UpdateNotification;
            //TxtInstruction.Text = @"1. To connect with BP Monitor, Press data transmission button for 2 sec," +
            //    " then press Connect button.";
            // TxtInstruction2.Text=@"2. If connected then don't have to connect again. 3. Result will display around 30 sec after test completed.";
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
            if (msg.Id != DeviceResponseType.BPCONCHEC || msg.Id != DeviceResponseType.BPCONMSG)
            {
               
                string[] res = msg.Msg.Split('>');
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    BtnConnect.IsEnabled = true;
                    switch (msg.Id)
                    {
                        case DeviceResponseType.BPCONMSG:
                            TxtConnectionstatus.Text = res[1].Split(':')[1];
                            if(res.Length>2 && res[2].ToLower().Equals("error"))
                            {
                                MainPage.mainPage.Thermostatusdelegate?.Invoke(false, 2);
                            }
                        //  BtnGlucoResult.IsEnabled = true;
                        break;
                        case DeviceResponseType.BPCONCHEC:
                            // "BPCONCTED>M:{0}>T:{1}";
                            if (res[1].Split(':')[1].ToLower().Equals("true"))
                                TxtConnectionstatus.Text = " Connected";
                            else
                            {
                                TxtConnectionstatus.Text = " Not Connected";
                                MainPage.mainPage.Thermostatusdelegate?.Invoke(false, 2);
                            }


                            //  BtnGlucoResult.IsEnabled = true;
                            break;

                    }
                });
            }
        }



        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.BPCMD);
            BtnConnect.IsEnabled = false;
        }

        private void CheckConn_Click(object sender, RoutedEventArgs e)
        {
            MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.BPCONCMD);
        }
    }

     
}
