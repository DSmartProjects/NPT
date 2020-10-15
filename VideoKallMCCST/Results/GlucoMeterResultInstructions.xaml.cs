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
    public sealed partial class GlucoMeterResultInstructions : UserControl
    {
        public GlucoMeterResultInstructions()
        {
            this.InitializeComponent();
            MainPage.mainPage.mainpagecontext.NotifyResult += UpdateNotification;
            BtnGlucoResult.IsEnabled = true;
        }

        async void UpdateNotification(object sender, CommunicationMsg msg)
        {
            if (msg.Id != DeviceResponseType.GLUCORESULTSTATUS)
                return;
                
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                switch (msg.Id)
                {
                    case DeviceResponseType.GLUCORESULTSTATUS:
                        TxtGlucoTestStatus.Text = msg.Msg.Split('>')[1];
                        BtnGlucoResult.IsEnabled = true;
                        break;

                }
            });
        }

        private void BtnGlucoResult_Click(object sender, RoutedEventArgs e)
        {
            MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.GLUCORESULTCMD);
            BtnGlucoResult.IsEnabled = false;
        }

        private void BtnGlucoResultdone_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
