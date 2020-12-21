using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using VideoKallMCCST.Communication;
using Windows.UI.Core;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VideoKallMCCST.Results
{
    public sealed partial class Weight : UserControl
    {
        public Weight()
        {
            this.InitializeComponent();
            MainPage.mainPage.CASResult += CasNotification;
        }

        async void CasNotification(string message, int devicecode, int isresultornotificationmsg)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (devicecode == 2 && isresultornotificationmsg == 1)
                {
                    TxtStatus.Text = message;
                }
            });
        }

        private void BtnGetWeight_Click(object sender, RoutedEventArgs e)
        {
           
            MainPage.mainPage.CommToDataAcq.SendMessageToDataacquistionapp(CommunicationCommands.WM);
        }
    }
}
