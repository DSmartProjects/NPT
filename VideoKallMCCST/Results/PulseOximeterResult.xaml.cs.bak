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
        }
        async void UpdateNotification(object sender, CommunicationMsg msg)
        {
            if (msg.Id != DeviceResponseType.PULSEOXIMETERSTATUS)
                return;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                switch (msg.Id)
                {
                    case DeviceResponseType.PULSEOXIMETERSTATUS:
                        BtnStreamdata.IsEnabled = true;
                        TxtSpiroMeterConnectionStatus.Text = msg.Msg.Split('>')[1];
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
