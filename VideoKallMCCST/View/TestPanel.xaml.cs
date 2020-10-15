using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using VideoKallMCCST.Communication;
using VideoKallMCCST.Stethoscope;
using VideoKallMCCST.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace VideoKallMCCST.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestPanel : Page
    {

        public TestPanel()
        {
            this.InitializeComponent();
            MainPage.mainPage.mainpagecontext.NotifyResult += UpdateNotification;
            MainPage.mainPage.StethoscopeRecord += RecordStethocopeStream;
            MainPage.mainPage.OtoscopeComm += OtoscopecommandHandler;
            MainPage.mainPage.StethoscopeStartStop += StartStopStethoscope;
        }

        string strRootFolder = "VideoKall";
        string strRootFolderPath = @"\\192.168.0.33\";// VideoKall";
        private async Task SetImagefolder()
        {
            try
            {
                strRootFolderPath = "\\\\" + MainPage.mainPage.SMCCommChannel.IPAddress + "\\"; ;
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(strRootFolderPath + strRootFolder);
                //  StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                }
                // StorageFolder newFolder;

                MainPage.mainPage.rootImageFolder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");
                strRootFolderPath = MainPage.mainPage.rootImageFolder.Path;
                //  TxtImageFolder = "";


            }
            catch (Exception ex)
            {
                string s = ex.Message;
                MainPage.mainPage.NotifyStatusCallback?.Invoke(ex.Message);
            }
        }

        StethescopeRX _StethoscopeRx = null;
        private void _StethoscopeRx_TXevents(object sender, EventArgs e)
        {

            MainPage.mainPage.StethoscopeNotification?.Invoke((string)sender);
            // MainPage.mainPage.NotifyStatusCallback?.Invoke((string)sender);
        }

        void StartStopStethoscope(string msg, int code)
        {
            try
            {
                switch (msg.ToLower())
                {
                    case "startst":
                        {
                                ExecuteStethoscopeCommand(true);
                             //   isStarted = true;
                        }
                        break;
                    case "stopst":
                        {
                             
                                ExecuteStethoscopeCommand(false); 
                        }
                        break;
                }
            }catch(Exception ex)
            {
                MainPage.mainPage.LogExceptions(ex.Message);
            }
        }

        void ExecuteStethoscopeCommand(bool StethoscopeChesttoggle)
            {
                try
                {
                    if (_StethoscopeRx == null)
                    {
                        _StethoscopeRx = new StethescopeRX();
                        _StethoscopeRx.Initialize();
                        _StethoscopeRx.TXevents += _StethoscopeRx_TXevents;
                    }

                    if (StethoscopeChesttoggle)
                    {
                        _StethoscopeRx.ConnectTX();
                    }
                    else if (_StethoscopeRx != null)
                    {
                        _StethoscopeRx.DisconnectTX();
                    }
                }
                catch (Exception ex)
                {
                    MainPage.mainPage.LogExceptions(ex.Message);
                }
            }
        
        async void UpdateNotification(object sender, CommunicationMsg msg)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                string[] res = msg.Msg.Split('>');
                switch (msg.Id)
                {
                    case DeviceResponseType.PULSEOXIMETERRESULT:


                        TxtResultPulseOximeter.Text = res[1].Split(':')[1];
                        TxtResultPulseOximeterpulse.Text = res[2].Split(':')[1];
                        TxtResultPulseOximeterpulsedate.Text = res[4];
                        break;
                    case DeviceResponseType.GLUCORESULT:
                        //"GLUCMDRES>V:{0}>U:{1}>T:{2}>M:{3}>D:{4}>T:{5}";
                        TxtResultgluco.Text = res[1].Split(':')[1] + " " + res[2].Split(':')[1];
                        TxtTestType.Text = res[3].Split(':')[1];
                        TxtResultglucoTestMode.Text = res[4].Split(':')[1];
                        TxtDate.Text = res[5].Split(':')[1];
                        TxtTime.Text = res[7];
                        break;
                    case DeviceResponseType.THERMORESTULT:
                        {
                            if (res[2].Split(':')[1].ToLower().Contains("object"))
                                return;

                            tempresultreceived = true;
                            //"THERMORES>R:{0}>M:{1}>S:{2}>DT:{3}"
                            string tempformat = "{0}°{1}";
                            decimal Conversion = Convert.ToDecimal(res[1].Split(':')[1]);
                            tempResult = Conversion;
                            if (MainPage.mainPage.mainpagecontext.ThermometerUnitF)
                            {
                                Conversion = decimal.Round((Conversion * (decimal)1.8), 1) + 32;
                            }
                            else
                            {
                                Conversion = decimal.Round(Conversion, 1);
                            }

                            if (Convert.ToBoolean(res[3].Split(':')[1]))
                                ///  TxtTemprature.Text = string.Format(tempformat, Conversion.ToString(), TxtTmpUnitbtn.IsOn ? TxtTmpUnitbtn.OnContent : TxtTmpUnitbtn.OffContent);
                                TxtTemprature.Text = string.Format(tempformat, Conversion.ToString(), MainPage.mainPage.mainpagecontext.ThermometerUnitF ? "F" : "C");
                            else
                                TxtTemprature.Text = "Error: Lo";

                            TxtMode.Text = res[2].Split(':')[1];
                        //    "THERMORES>R:{0}>M:{1}>S:{2}>{3}";
                            TxtDateTime.Text = res[4];

                        }

                        break;
                    case DeviceResponseType.BPRES:
                        // "BPRES>D:{0}>S:{1}>P:{2}>DT:{3}>T:{4}";
                        TxtSys.Text = res[2].Split(':')[1];
                        TxtDia.Text = res[1].Split(':')[1];
                        TxtPulse.Text = res[3].Split(':')[1];
                        //  TxttestTime.Text = res[4].Split(':')[1] + " " + res[5];
                        break;

                }
            });
        }

        decimal tempResult = 0;
        bool tempresultreceived = false;
        private void TxtTmpUnitbtn_Toggled(object sender, RoutedEventArgs e)
        {
            if (!tempresultreceived)
                return;

            string tempformat = "{0}°{1}";

            if (TxtTmpUnitbtn.IsOn)
            {
                decimal Conversion = decimal.Round((tempResult * (decimal)1.8), 1) + 32;
                TxtTemprature.Text = string.Format(tempformat, Conversion.ToString(), TxtTmpUnitbtn.IsOn ? TxtTmpUnitbtn.OnContent : TxtTmpUnitbtn.OffContent);

            }
            else
            {
                decimal Conversion = decimal.Round(tempResult, 1);
                TxtTemprature.Text = string.Format(tempformat, Conversion, TxtTmpUnitbtn.IsOn ? TxtTmpUnitbtn.OnContent : TxtTmpUnitbtn.OffContent);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }


        bool BtnPulseoximeterToggle = false;
        private void BtnPulseoximeter_Click(object sender, RoutedEventArgs e)
        {
            if (TestIsInProgress && !BtnPulseoximeterToggle)
                return;
            BtnPulseoximeterToggle = !BtnPulseoximeterToggle;
            TestIsInProgress = BtnPulseoximeterToggle;

            BtnPulseoximeter.Background = BtnPulseoximeterToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlPulseoximterResult.Height = ht * 4;//gridInstrumentPanel.ActualHeight;
            CtrlPulseoximterResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            ResultPulseOximeterPopup.IsOpen = BtnPulseoximeterToggle;
            if (BtnPulseoximeterToggle)
                MainPage.mainPage.SMCCommChannel.SendMessage(string.Format(CommunicationCommands.SMCPODDEPLOY, 1));
            else
                MainPage.mainPage.SMCCommChannel.SendMessage(string.Format(CommunicationCommands.SMCUSAGEDONE, 1));
        }

        bool _thermotoggle = false;
        private void BtnThermoMeter_Click(object sender, RoutedEventArgs e)
        {
            if (TestIsInProgress && !_thermotoggle)
                return;
            _thermotoggle = !_thermotoggle;
            TestIsInProgress = _thermotoggle;

            BtnThermoMeter.Background = _thermotoggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlThermoResult.Height = ht * 4;//gridInstrumentPanel.ActualHeight;
            CtrlThermoResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            ResulThermoPopup.IsOpen = _thermotoggle;
        }

        bool _resultBpToggle = false;
        private void BtnBP_Click(object sender, RoutedEventArgs e)
        {
            if (TestIsInProgress && !_resultBpToggle)
                return;
            _resultBpToggle = !_resultBpToggle;
            TestIsInProgress = _resultBpToggle;

            BtnBP.Background = _resultBpToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlBPResultsInstructions.Height = ht * 4;//gridInstrumentPanel.ActualHeight;
            CtrlBPResultsInstructions.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            ResultBPPopup.IsOpen = _resultBpToggle;
        }
        public void updateTime()
        {
            //  THERMORESTULT
        }

        bool btnWeightToggle = false;
        private void BtnWeight_Click(object sender, RoutedEventArgs e)
        {
            if (TestIsInProgress && !btnWeightToggle)
                return;
            btnWeightToggle = !btnWeightToggle;
            TestIsInProgress = btnWeightToggle;

            BtnWeight.Background = btnWeightToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlWeightResult.Height = ht * 4;//gridInstrumentPanel.ActualHeight;
            CtrlWeightResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            ResuWeightPopup.IsOpen = btnWeightToggle;
        }

        bool btnHeighttoggle = false;
        private void BtnHeight_Click(object sender, RoutedEventArgs e)
        {
            if (TestIsInProgress && !btnHeighttoggle)
                return;

            btnHeighttoggle = !btnHeighttoggle;
            TestIsInProgress = btnHeighttoggle;

            BtnHeight.Background = btnHeighttoggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlHeightoResult.Height = ht * 4;//gridInstrumentPanel.ActualHeight;
            CtrlHeightoResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            ResulHeightPopup.IsOpen = btnHeighttoggle;
        }

        bool _otoscopeToggle = false;
        private async void BtnOtoscope_Click(object sender, RoutedEventArgs e)
        {
            if (TestIsInProgress && !_otoscopeToggle)
                return;
            if (MainPage.mainPage.rootImageFolder == null)
              await  SetImagefolder();

            _otoscopeToggle = !_otoscopeToggle;
            TestIsInProgress = _otoscopeToggle;
            if (_otoscopeToggle)
                MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.STARTOTOSCOPE);
            else
                MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.STOPOTOSCOPE);
            BtnOtoscope.Background = _otoscopeToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlOtoscopeResult.Height = ht * 6;//gridInstrumentPanel.ActualHeight;
            CtrlOtoscopeResult.Width = wdth * 4;//gridInstrumentPanel.ActualWidth; ;
            ResulOtoscopePopup.IsOpen = _otoscopeToggle;
        }
        bool _dermascopeToggle = false;
        private async void BtnDermoscope_Click(object sender, RoutedEventArgs e)
        {
            if (TestIsInProgress && !_dermascopeToggle)
                return;

            if (MainPage.mainPage.rootImageFolder == null)
               await SetImagefolder();

            _dermascopeToggle = !_dermascopeToggle;
            TestIsInProgress = _dermascopeToggle;
            if (_dermascopeToggle)
                MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.STARTDERMO);
            else
                MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.STOPDERMO);

            BtnDermoscope.Background = _dermascopeToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlDermascopeResult.Height = ht * 6;//gridInstrumentPanel.ActualHeight;
            CtrlDermascopeResult.Width = wdth * 4;//gridInstrumentPanel.ActualWidth; ;
            ResulDermascopePopup.IsOpen = _dermascopeToggle;
        }


        async void OtoscopecommandHandler()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                ResulOtoscopePopup.IsOpen = false;
                ResulDermascopePopup.IsOpen = false;
            });

        }
        bool _ekgToggle = false;
        private void BtnEKG_Click(object sender, RoutedEventArgs e)
        {
            if (TestIsInProgress && !_ekgToggle)
                return;

            _ekgToggle = !_ekgToggle;
            TestIsInProgress = _ekgToggle;

            BtnEKG.Background = _ekgToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlekgResult.Height = ht * 4;//gridInstrumentPanel.ActualHeight;
            CtrlekgResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            ResultEKGpopup.IsOpen = _ekgToggle;
        }


        bool _glucoToggle = false;
        private void BtnGlucometer_Click(object sender, RoutedEventArgs e)
        {
            if (TestIsInProgress && !_glucoToggle)
                return;

            _glucoToggle = !_glucoToggle;
            TestIsInProgress = _glucoToggle;

            BtnGlucometer.Background = _glucoToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlglucoResult.Height = ht * 4;//gridInstrumentPanel.ActualHeight;
            CtrlglucoResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            Resultglucopopup.IsOpen = _glucoToggle;
        }

        bool _spirometerToggle = false;
        private void BtnSpirometer_Click(object sender, RoutedEventArgs e)
        {
            if (TestIsInProgress && !_spirometerToggle)
                return;
            _spirometerToggle = !_spirometerToggle;
            TestIsInProgress = _spirometerToggle;

            BtnSpirometer.Background = _spirometerToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlspiroResult.Height = ht * 4;//gridInstrumentPanel.ActualHeight;
            CtrlspiroResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            Resultspiropopup.IsOpen = _spirometerToggle;

        }

        bool _stethoscopeChest = false;
       
        private void BtnSthethoscope_Click(object sender, RoutedEventArgs e)
        {
            if (MainPage.mainPage.isStethoscopeStreaming)
                return;
            if (TestIsInProgress && !_stethoscopeChest)
                return;

            _stethoscopeChest = !_stethoscopeChest;
            TestIsInProgress = _stethoscopeChest;

            BtnSthethoscope.Background = _stethoscopeChest ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlstethoscopechestResult.Height = ht * 4;//gridInstrumentPanel.ActualHeight;
            CtrlstethoscopechestResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            Resultstethoscopechestpopup.IsOpen = _stethoscopeChest;
            if (!MainPage.mainPage.isStethoscopeReadystreaming)
            {
                MainPage.mainPage.SMCCommChannel.SendMessage(string.Format(CommunicationCommands.STARTSTCHEST));
            }

        //    ExecuteStethoscopeCommand(true);

        }

        bool TestIsInProgress = false;

        private void BtnSthethoscopeLungs_Click(object sender, RoutedEventArgs e)
        {

        }

        
        private void RecordStethocopeStream( )
        {
            try
            {
              //  if(isStarted  )
                _StethoscopeRx.RecordRx();
            }catch(Exception ex)
            {
                MainPage.mainPage.LogExceptions(ex.Message);
            }

        }

        

        private void TxtTmpUnitbtn_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
