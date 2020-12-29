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
using SBCDBModule;
using SBCDBModule.DB;
using VideoKallMCCST.View;
using VideoKallMCCST.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace VideoKallMCCST
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VideoKallLoginPage : Page
    {
        internal LoginPageViewModel _loginVM = null;
        public static VideoKallLoginPage LoginPage;
        public VideoKallLoginPage()
        {
            LoginPage = this;
            _loginVM = new LoginPageViewModel();
            this.InitializeComponent();
            this.DataContext = _loginVM;
        }


    }
}
