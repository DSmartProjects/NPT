﻿
using SBCDBModule;
using SBCDBModule.DB;
using VideoKallMCCST.View;
using VideoKallMCCST.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace VideoKallMCCST
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        LoginPageViewModel dbcontext = new LoginPageViewModel();
        public LoginPage()
        {
            this.InitializeComponent();
            this.DataContext = dbcontext;
        }  
    }
}
