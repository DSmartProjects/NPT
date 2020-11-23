using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using VideoKallMCCST.Communication;
using VideoKallMCCST.Results;
using VideoKallMCCST.Stethoscope;
using VideoKallMCCST.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI;
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
    public sealed partial class TestPanelExpander : Page
    {
        public static TestPanelExpander TestPanelExp;
        public TestPanelExpander()
        {
            this.InitializeComponent();
            TestPanelExp = this;
            MainPage.mainPage.mainpagecontext.NotifyResult += UpdateNotification;
            MainPage.mainPage.StethoscopeRecord += RecordStethocopeStream;
            MainPage.mainPage.OtoscopeComm += OtoscopecommandHandler;
            MainPage.mainPage.StethoscopeStartStop += StartStopStethoscope;
            MainPage.mainPage.StethoscopeStatus += UpdateNotification;
            MainPage.mainPage.Spirometercallback += SpirometerDone;
            MainPage.mainPage.Spirometrystatus += SpirometerStatus;
            ShowHidePulseoximeterdata(false);
            ShowHideglucodata(false);
            ShowHidebpdata(false);
            ShowTemppdata(false);
            MainPage.mainPage.NextPatient += NextPatient;
            MainPage.mainPage.MicroscopeStatus += MicroscopeStatus;

            MainPage.mainPage.Thermostatusdelegate += UpdateThermoStatus;

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

        private void isTestResultOpened()
        {
            MainPage.mainPage.TestIsInProgress =
                ResulThermoPopup.IsOpen || ResultPulseOximeterPopup.IsOpen || ResultBPPopup.IsOpen || ResuWeightPopup.IsOpen || ResulHeightPopup.IsOpen
                 || ResulOtoscopePopup.IsOpen || ResulDermascopePopup.IsOpen || ResultEKGpopup.IsOpen || Resultglucopopup.IsOpen ||
                 Resultspiropopup.IsOpen || Resultstethoscopechestpopup.IsOpen;
        }

        void ShowHidePulseoximeterdata(bool visible)
        {
            if (visible)
            {
                TxtLableSPO2.Visibility = Visibility.Visible;
                TxtLabelPulseRate.Visibility = Visibility.Visible;
                //TxtLabelPulseTime.Visibility = Visibility.Visible;
            }
            else
            {
                TxtLableSPO2.Visibility = Visibility.Collapsed;
                TxtLabelPulseRate.Visibility = Visibility.Collapsed;
                //TxtLabelPulseTime.Visibility = Visibility.Collapsed;
                TxtResultPulseOximeter.Text = "";
                TxtResultPulseOximeterpulse.Text = "";
                //TxtResultPulseOximeterpulsedate.Text = "";
            }

        }

        void ShowHideglucodata(bool visible)
        {
            if (visible)
            {
                TxtLabelResultgluco.Visibility = Visibility.Visible;
                //TxtLabeltestType.Visibility = Visibility.Visible;
                //TxtLabelTestMode.Visibility = Visibility.Visible;
                //TxtlabelDate.Visibility = Visibility.Visible;
                //TxtLabelTime.Visibility = Visibility.Visible;
            }
            else
            {
                TxtLabelResultgluco.Visibility = Visibility.Collapsed;
                //TxtLabeltestType.Visibility = Visibility.Collapsed;
                //TxtLabelTestMode.Visibility = Visibility.Collapsed;
                //TxtlabelDate.Visibility = Visibility.Collapsed;
                //TxtLabelTime.Visibility = Visibility.Collapsed;
                //TxtResultgluco.Text = "";
                //TxtTestType.Text = "";
                //TxtResultglucoTestMode.Text = "";
                //TxtTime.Text = "";
                //TxtDate.Text = "";
            }

        }

        void ShowHidebpdata(bool visible)
        {
            if (visible)
            {
                TxtLabelSys.Visibility = Visibility.Visible;
               // Txtlabeldia.Visibility = Visibility.Visible;
               // TxtLabelpulsebp.Visibility = Visibility.Visible;
            }
            else
            {
                TxtLabelSys.Visibility = Visibility.Collapsed;
                //Txtlabeldia.Visibility = Visibility.Collapsed;
                //TxtLabelpulsebp.Visibility = Visibility.Collapsed;
                TxtSys.Text = "";
                //TxtDia.Text = "";
                //TxtPulse.Text = "";
            }

        }

        void ShowTemppdata(bool visible)
        {
            if (!visible)
            {

                //TxtMode.Text = "";
                TxtTemprature.Text = "";
                TblTemp.Visibility = Visibility.Collapsed;
               // TxtDateTime.Text = "";
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
            }
            catch (Exception ex)
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

        async void NextPatient()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ShowHidePulseoximeterdata(false);
                ShowHideglucodata(false);
                ShowHidebpdata(false);
                ShowTemppdata(false);

                //BtnThermoMeter.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
                ////BtnPulseoximeter.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
                //BtnBP.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
                //BtnGlucometer.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
                //BtnOtoscope.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
                //BtnDermoscope.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
                //BtnSthethoscope.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
                ////BtnEKG.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
                //BtnWeight.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
                //BtnHeight.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
                //// BtnEKG.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
                //BtnSpirometer.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);

                grdGluco.BorderBrush = GetColorFromHexa("#EEEEEE");
                grdGluco.BorderThickness = new Thickness(0, 0, 0, 10);


                TxtResultstethoscopechest.BorderBrush = GetColorFromHexa("#EEEEEE");
                TxtResultstethoscopechest.BorderThickness = new Thickness(0, 0, 0, 10);


                gridThermo.BorderBrush = GetColorFromHexa("#EEEEEE");
                gridThermo.BorderThickness = new Thickness(0, 0, 0, 10);

                grBp.BorderBrush = GetColorFromHexa("#EEEEEE");
                grBp.BorderThickness = new Thickness(0, 0, 0, 10);

                TxtResultWeight.BorderBrush = GetColorFromHexa("#EEEEEE");
                TxtResultWeight.BorderThickness = new Thickness(0, 0, 0, 10);

                TxtResultHeight.BorderBrush = GetColorFromHexa("#EEEEEE");
                TxtResultHeight.BorderThickness = new Thickness(0, 0, 0, 10);

                TxtResultOtoscope.BorderBrush = GetColorFromHexa("#EEEEEE");
                TxtResultOtoscope.BorderThickness = new Thickness(0, 0, 0, 10);

                TxtResultDermascope.BorderBrush = GetColorFromHexa("#EEEEEE");
                TxtResultDermascope.BorderThickness = new Thickness(0, 0, 0, 10);

                TxtResultspiro.BorderBrush = GetColorFromHexa("#EEEEEE");
                TxtResultspiro.BorderThickness = new Thickness(0, 0, 0, 10);
                TxtResultstethoscopechest.BorderBrush = GetColorFromHexa("#EEEEEE");
                TxtResultstethoscopechest.BorderThickness = new Thickness(0, 0, 0, 10);

                //BtnSthethoscopeLungs.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
                TbxSeatBackSet.BorderBrush = GetColorFromHexa("#EEEEEE");
                TbxSeatBackSet.BorderThickness = new Thickness(0, 0, 0, 10);
            }
            );

        }

        async void UpdateNotification(bool issuccess)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (issuccess)
                {
                    if (_stethoscopeChest)
                    {
                        //BtnSthethoscope.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                        TxtResultstethoscopechest.BorderBrush = GetColorFromHexa("#34CBA8");
                        TxtResultstethoscopechest.BorderThickness = new Thickness(0, 0, 0, 10);
                    }
                       
                    else if (_stethoscopelungs)
                    {
                        //BtnSthethoscopeLungs.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);                     
                        TbxSeatBackSet.BorderBrush = GetColorFromHexa("#34CBA8");
                        TbxSeatBackSet.BorderThickness = new Thickness(0, 0, 0, 10);
                    }
                        
                }
                else if (!issuccess)
                {
                    if (_stethoscopeChest)
                    {
                        TxtResultstethoscopechest.BorderBrush = GetColorFromHexa("#E96056");
                        TxtResultstethoscopechest.BorderThickness = new Thickness(0, 0, 0, 10);
                    }
                        //BtnSthethoscope.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    else if (_stethoscopelungs)
                    {
                        TbxSeatBackSet.BorderBrush = GetColorFromHexa("#E96056");
                        TbxSeatBackSet.BorderThickness = new Thickness(0, 0, 0, 10);
                    }
                        //BtnSthethoscopeLungs.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                }
            }
            );

        }

        async void UpdateThermoStatus(bool success, int devicetypes)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (!success && devicetypes == 0)
                {
                    gridThermo.BorderBrush = GetColorFromHexa("#E96056");
                    gridThermo.BorderThickness = new Thickness(0, 0, 0, 10);
                    //BtnThermoMeter.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                }
                    
                else if (!success && devicetypes == 1)
                {
                    //BtnPulseoximeter.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    grdPulse.BorderBrush = GetColorFromHexa("#E96056");
                    grdPulse.BorderThickness = new Thickness(0, 0, 0, 10);
                }
                else if (!success && devicetypes == 2)
                {
                    // BtnBP.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    grBp.BorderBrush = GetColorFromHexa("#E96056");
                    grBp.BorderThickness = new Thickness(0, 0, 0, 10);
                }
                else if (!success && devicetypes == 3)
                {
                    //BtnGlucometer.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    //grdGluco.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
                    grdGluco.BorderBrush =GetColorFromHexa("#E96056");
                    grdGluco.BorderThickness = new Thickness(0, 0, 0, 10);
                }

            });


        }



        async void UpdateNotification(object sender, CommunicationMsg msg)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                string[] res = msg.Msg.Split('>');
                switch (msg.Id)
                {
                    case DeviceResponseType.PULSEOXIMETERRESULT:

                        ShowHidePulseoximeterdata(true);
                        //BtnPulseoximeter.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                        grdPulse.BorderBrush = GetColorFromHexa("#34CBA8");
                        grdPulse.BorderThickness = new Thickness(0, 0, 0, 10);
                        TxtResultPulseOximeter.Text =res[1].Split(':')[1]+" %";
                        TxtResultPulseOximeterpulse.Text = res[2].Split(':')[1]+" bpm";
                        //TxtResultPulseOximeterpulsedate.Text = res[4];
                        break;
                    case DeviceResponseType.GLUCORESULT:
                        ShowHideglucodata(true);
                        grdGluco.BorderBrush = GetColorFromHexa("#34CBA8");
                        grdGluco.BorderThickness = new Thickness(0, 0, 0, 10);
                        //BtnGlucometer.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                        //"GLUCMDRES>V:{0}>U:{1}>T:{2}>M:{3}>D:{4}>T:{5}";
                         TxtResultgluco.Text = res[1].Split(':')[1] + " " + res[2].Split(':')[1];
                        //TxtTestType.Text = res[3].Split(':')[1];
                        //TxtResultglucoTestMode.Text = res[4].Split(':')[1];
                        //TxtDate.Text = res[5].Split(':')[1];
                        //TxtTime.Text = res[7];
                        break;
                    case DeviceResponseType.THERMORESTULT:
                        {
                            if (res[2].Split(':')[1].ToLower().Contains("object"))
                                return;
                            //BtnThermoMeter.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            gridThermo.BorderBrush = GetColorFromHexa("#34CBA8");
                            gridThermo.BorderThickness = new Thickness(0, 0, 0, 10);
                            tempresultreceived = true;
                            //"THERMORES>R:{0}>M:{1}>S:{2}>DT:{3}"
                            string tempformat = "{0} °{1}";
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

                            TblTemp.Visibility = Visibility.Visible;
                            //TxtMode.Text = res[2].Split(':')[1];
                            //    "THERMORES>R:{0}>M:{1}>S:{2}>{3}";
                            // TxtDateTime.Text = res[4];

                        }

                        break;
                    case DeviceResponseType.BPRES:
                        ShowHidebpdata(true);
                        grBp.BorderBrush = GetColorFromHexa("#34CBA8"); ;
                        grBp.BorderThickness = new Thickness(0, 0, 0, 10);

                        //BtnBP.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                        // "BPRES>D:{0}>S:{1}>P:{2}>DT:{3}>T:{4}";
                        TxtSys.Text = res[2].Split(':')[1]+"/"+ res[1].Split(':')[1]+" mmHg";                      
                       
                        //TxtDia.Text = res[1].Split(':')[1];
                        //TxtPulse.Text = res[3].Split(':')[1];
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
            isTestResultOpened();
            if ((MainPage.mainPage.TestIsInProgress && !BtnPulseoximeterToggle) || (!ConnectionCheck && !BtnPulseoximeterToggle))
                return;

            BtnPulseoximeterToggle = !BtnPulseoximeterToggle;
            //if (BtnPulseoximeterToggle)
            //MainPage.mainPage.StatusTxt.Text = "";
            MainPage.mainPage.TestIsInProgress = BtnPulseoximeterToggle;
            if (BtnPulseoximeterToggle)
            {
                //BtnPulseoximeter.Background = GetColorFromHexa("#FFBF00"); //BtnPulseoximeterToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
                grdPulse.BorderBrush = GetColorFromHexa("#FFC10D");
                grdPulse.BorderThickness = new Thickness(0, 0, 0, 10);
            }
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlPulseoximterResult.Height = ht * 2;//gridInstrumentPanel.ActualHeight;
            CtrlPulseoximterResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            ResultPulseOximeterPopup.IsOpen = BtnPulseoximeterToggle;
            if (BtnPulseoximeterToggle)
                MainPage.mainPage.SMCCommChannel.SendMessage(string.Format(CommunicationCommands.SMCPODDEPLOY, 1));
            else
                MainPage.mainPage.SMCCommChannel.SendMessage(string.Format(CommunicationCommands.SMCUSAGEDONE, 1));
        }
        public SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            return new SolidColorBrush(
                Color.FromArgb(
                    255,
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16)
                )
            );
        }


        bool _thermotoggle = false;
        private void BtnThermoMeter_Click(object sender, RoutedEventArgs e)
        {
            isTestResultOpened();

            if ((MainPage.mainPage.TestIsInProgress && !_thermotoggle) || (!ConnectionCheck && !_thermotoggle))
                return;

            _thermotoggle = !_thermotoggle;
            //if (_thermotoggle)
                //MainPage.mainPage.StatusTxt.Text = "";

                MainPage.mainPage.TestIsInProgress = _thermotoggle;
            if (_thermotoggle)
            { 
                //BtnThermoMeter.Background = GetColorFromHexa("#FFBF00"); // : new SolidColorBrush(Windows.UI.Colors.LightGray);
                gridThermo.BorderBrush= GetColorFromHexa("#FFC10D");
               gridThermo.BorderThickness = new Thickness(0, 0, 0, 10);
            }
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlThermoResult.Height = ht * 1.5;//gridInstrumentPanel.ActualHeight;
            CtrlThermoResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            ResulThermoPopup.IsOpen = _thermotoggle;
        }

        bool _resultBpToggle = false;
        private void BtnBP_Click(object sender, RoutedEventArgs e)
        {
            isTestResultOpened();

            if ((MainPage.mainPage.TestIsInProgress && !_resultBpToggle) || (!ConnectionCheck && !_resultBpToggle))
                return;

            _resultBpToggle = !_resultBpToggle;
           // if (_resultBpToggle)
                //MainPage.mainPage.StatusTxt.Text = "";

                MainPage.mainPage.TestIsInProgress = _resultBpToggle;
            if (_resultBpToggle)
            { 
                //BtnBP.Background = GetColorFromHexa("#FFBF00");//_resultBpToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
                grBp.BorderBrush = GetColorFromHexa("#FFC10D");
                grBp.BorderThickness = new Thickness(0, 0, 0, 10);
            }
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlBPResultsInstructions.Height = ht * 2;//gridInstrumentPanel.ActualHeight;
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
            isTestResultOpened();

            if ((MainPage.mainPage.TestIsInProgress && !btnWeightToggle) || (!ConnectionCheck && !btnWeightToggle))
                return;

            btnWeightToggle = !btnWeightToggle;

            //if (btnWeightToggle)
                //MainPage.mainPage.StatusTxt.Text = "";

                MainPage.mainPage.TestIsInProgress = btnWeightToggle;
            if (btnWeightToggle)
            {
                TxtResultWeight.BorderBrush = GetColorFromHexa("#FFC10D");
                TxtResultWeight.BorderThickness = new Thickness(0, 0, 0, 10);
            }
                //BtnWeight.Background = GetColorFromHexa("#FFBF00");// btnWeightToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
               
          
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlWeightResult.Height = ht * 2;//gridInstrumentPanel.ActualHeight;
            CtrlWeightResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            ResuWeightPopup.IsOpen = btnWeightToggle;
        }

        bool btnHeighttoggle = false;
        private void BtnHeight_Click(object sender, RoutedEventArgs e)
        {

            isTestResultOpened();

            if ((MainPage.mainPage.TestIsInProgress && !btnHeighttoggle) || (!ConnectionCheck && !btnHeighttoggle))
                return;

            btnHeighttoggle = !btnHeighttoggle;

            //if (btnHeighttoggle)
            //    //MainPage.mainPage.StatusTxt.Text = "";

            //MainPage.mainPage.TestIsInProgress = btnHeighttoggle;
            //if (btnHeighttoggle)
            //    BtnHeight.Background = GetColorFromHexa("#FFBF00");// btnHeighttoggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            //double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            //double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            //CtrlHeightoResult.Height = ht * 2;//gridInstrumentPanel.ActualHeight;
            //CtrlHeightoResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            //ResulHeightPopup.IsOpen = btnHeighttoggle;

            MainPage.mainPage.TestIsInProgress = btnHeighttoggle;
            if (btnHeighttoggle)
            { 
                TxtResultHeight.BorderBrush = GetColorFromHexa("#FFC10D");
                TxtResultHeight.BorderThickness = new Thickness(0, 0, 0, 10);
            }// btnHeighttoggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;
            CtrlHeightoResult.Height = ht * 2;//gridInstrumentPanel.ActualHeight;
            CtrlHeightoResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            ResulHeightPopup.IsOpen = btnHeighttoggle;

        }

        bool _otoscopeToggle = false;
        private async void BtnOtoscope_Click(object sender, RoutedEventArgs e)
        {
            isTestResultOpened();

            if ((MainPage.mainPage.TestIsInProgress && !_otoscopeToggle) || (!ConnectionCheck && !_otoscopeToggle))
                return;
            try
            {
                if (MainPage.mainPage.rootImageFolder == null)
                    await SetImagefolder();


                _otoscopeToggle = !_otoscopeToggle;
               // if (_otoscopeToggle)
                    //MainPage.mainPage.StatusTxt.Text = "";
                    MainPage.mainPage.TestIsInProgress = _otoscopeToggle;
                if (_otoscopeToggle)
                {
                    MainPage.mainPage.DigitalMicroscope?.Invoke(false);
                    MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.STARTOTOSCOPE);
                }
                // else
                //   MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.STOPOTOSCOPE);
                if (_otoscopeToggle)
                    TxtResultOtoscope.BorderBrush = GetColorFromHexa("#FFC10D");
                TxtResultOtoscope.BorderThickness = new Thickness(0, 0, 0, 10);
                /* BtnOtoscope.Background = GetColorFromHexa("#FFBF00");*/ // _otoscopeToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
                double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
                double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

                CtrlOtoscopeResult.Height = ht * 2;//gridInstrumentPanel.ActualHeight;
                CtrlOtoscopeResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
                ResulOtoscopePopup.IsOpen = _otoscopeToggle;
            }
            catch (Exception ex)
            {
                MainPage.mainPage.NotifyStatusCallback?.Invoke(ex.Message, 3);
            }
        }

        bool _dermascopeToggle = false;
        private async void BtnDermoscope_Click(object sender, RoutedEventArgs e)
        {
            isTestResultOpened();
            if ((MainPage.mainPage.TestIsInProgress && !_dermascopeToggle) || (!ConnectionCheck && !_dermascopeToggle))
                return;

            try
            {
                if (MainPage.mainPage.rootImageFolder == null)
                    await SetImagefolder();

                _dermascopeToggle = !_dermascopeToggle;
                MainPage.mainPage.TestIsInProgress = _dermascopeToggle;
                // if (_dermascopeToggle)
                //MainPage.mainPage.StatusTxt.Text = "";
                if (_dermascopeToggle)
                {
                    MainPage.mainPage.DigitalMicroscope?.Invoke(true);
                    MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.STARTDERMO);

                }
                //   else
                // MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.STOPDERMO);
                if (_dermascopeToggle)
                {
                    TxtResultDermascope.BorderBrush = GetColorFromHexa("#FFC10D");
                    TxtResultDermascope.BorderThickness = new Thickness(0, 0, 0, 10);
                }
                //BtnDermoscope.Background = GetColorFromHexa("#FFBF00"); //_dermascopeToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
                double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
                double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

                CtrlDermascopeResult.Height = ht * 2;//gridInstrumentPanel.ActualHeight;
                CtrlDermascopeResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
                ResulDermascopePopup.IsOpen = _dermascopeToggle;

            }
            catch (Exception ex)
            {
                MainPage.mainPage.NotifyStatusCallback?.Invoke(ex.Message, 3);
            }
        }


        async void OtoscopecommandHandler()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                ResulOtoscopePopup.IsOpen = false;
                ResulDermascopePopup.IsOpen = false;

                if (_otoscopeToggle)
                {
                    // BtnOtoscope.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    BtnOtoscope_Click(null, null);
                }

                if (_dermascopeToggle)
                {
                    //  BtnDermoscope.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    BtnDermoscope_Click(null, null);
                }

            });

        }

        public async void MicroscopeStatus(bool issuccess)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (_otoscopeToggle)
                {
                    BtnOtoscope.Background = issuccess ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.Red);

                }

                if (_dermascopeToggle)
                {
                    BtnDermoscope.Background = issuccess ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.Red);

                }

            });

        }

        bool _ekgToggle = false;
        private void BtnEKG_Click(object sender, RoutedEventArgs e)
        {
            //isTestResultOpened();

            //if ((MainPage.mainPage.TestIsInProgress && !_ekgToggle) || (!ConnectionCheck && !_ekgToggle))
            //    return;


            //_ekgToggle = !_ekgToggle;
            //if (_ekgToggle)
            //    // MainPage.mainPage.StatusTxt.Text = "";
            //    MainPage.mainPage.TestIsInProgress = _ekgToggle;
            //if (_ekgToggle)
            //    BtnEKG.Background = GetColorFromHexa("#FFBF00");  //_ekgToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            //double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            //double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            //CtrlekgResult.Height = ht * 4;//gridInstrumentPanel.ActualHeight;
            //CtrlekgResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            //ResultEKGpopup.IsOpen = _ekgToggle;
        }


        bool _glucoToggle = false;
        private void BtnGlucometer_Click(object sender, RoutedEventArgs e)
        {
            isTestResultOpened();
            if (!ConnectionCheck && !_glucoToggle)
                return;

            if (MainPage.mainPage.TestIsInProgress && !_glucoToggle)
                return;

            _glucoToggle = !_glucoToggle;

            if (_glucoToggle)
            {
                //MainPage.mainPage.StatusTxt.Text = "";
                MainPage.mainPage.ResetGluco?.Invoke();
            }

            MainPage.mainPage.TestIsInProgress = _glucoToggle;
            if (_glucoToggle)
            {
                grdGluco.BorderBrush = GetColorFromHexa("#FFC10D");
                grdGluco.BorderThickness = new Thickness(0, 0, 0, 10);
            }           
            // BtnGlucometer.Background = GetColorFromHexa("#FFBF00");// _glucoToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlglucoResult.Height = ht * 2;//gridInstrumentPanel.ActualHeight;
            CtrlglucoResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            Resultglucopopup.IsOpen = _glucoToggle;         
        }

        bool _spirometerToggle = false;
        private void BtnSpirometer_Click(object sender, RoutedEventArgs e)
        {
            isTestResultOpened();
            if ((MainPage.mainPage.TestIsInProgress && !_spirometerToggle) || (!ConnectionCheck && !_spirometerToggle))
                return;

            _spirometerToggle = !_spirometerToggle;
            //if (_spirometerToggle)
                //MainPage.mainPage.StatusTxt.Text = "";

                MainPage.mainPage.TestIsInProgress = _spirometerToggle;
            if (_spirometerToggle)
            {
                MainPage.mainPage.ResetSpirometr?.Invoke();
                TxtResultspiro.BorderBrush = GetColorFromHexa("#FFC10D");
                TxtResultspiro.BorderThickness = new Thickness(0, 0, 0, 10);
                /*BtnSpirometer.Background = GetColorFromHexa("#FFBF00");*/ //_spirometerToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            }

            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlspiroResult.Height = ht * 3;//gridInstrumentPanel.ActualHeight;
            CtrlspiroResult.Width = wdth * 4;//gridInstrumentPanel.ActualWidth; ;
            Resultspiropopup.IsOpen = _spirometerToggle;
        }

        private async void SpirometerStatus(bool issuccess)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                BtnSpirometer.Background = issuccess ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.Red);

            });
        }

        void SpirometerDone()
        {
            BtnSpirometer_Click(null, null);
        }

        bool _stethoscopeChest = false;

        private void BtnSthethoscope_Click(object sender, RoutedEventArgs e)
        {
            isTestResultOpened();
            if (MainPage.mainPage.isStethoscopeStreaming || (!ConnectionCheck && !_stethoscopeChest))
                return;


            if (MainPage.mainPage.TestIsInProgress && !_stethoscopeChest)
                return;

            _stethoscopeChest = !_stethoscopeChest;
            MainPage.mainPage.IsStethescopeChest = _stethoscopeChest;

            //if (_stethoscopeChest)
                // MainPage.mainPage.StatusTxt.Text = "";
                MainPage.mainPage.TestIsInProgress = _stethoscopeChest;
            if (_stethoscopeChest)
            {
                TxtResultstethoscopechest.BorderBrush = GetColorFromHexa("#FFC10D");
                TxtResultstethoscopechest.BorderThickness = new Thickness(0, 0, 0, 10);
            }
                //BtnSthethoscope.Background = GetColorFromHexa("#FFBF00"); //_stethoscopeChest ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;

            CtrlstethoscopechestResult.Height = ht * 3;//gridInstrumentPanel.ActualHeight;
            CtrlstethoscopechestResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            Resultstethoscopechestpopup.IsOpen = _stethoscopeChest;
            if (_stethoscopeChest)// && !MainPage.mainPage.isStethoscopeReadystreaming)
            {
                MainPage.mainPage.SMCCommChannel.SendMessage(string.Format(CommunicationCommands.STARTSTCHEST));
            }

            //  ExecuteStethoscopeCommand(true);

        }

        bool ConnectionCheck
        {
            get
            {
                return MainPage.mainPage.mainpagecontext.IsSMCConnected;
                //  && MainPage.mainPage.isDataAcquitionappConnected;
            }

        }

        bool _stethoscopelungs = false;
        private void BtnSthethoscopeLungs_Click(object sender, RoutedEventArgs e)
        {
            isTestResultOpened();

            if ((MainPage.mainPage.TestIsInProgress && !_stethoscopelungs) ||
                (!ConnectionCheck && !_stethoscopelungs) ||
                 MainPage.mainPage.isStethoscopeStreaming
                )
                return;

            _stethoscopelungs = !_stethoscopelungs;
            MainPage.mainPage.IsStethescopeChest = false;
            MainPage.mainPage.ResetSTLungs?.Invoke();

            //if (_stethoscopelungs)
                // MainPage.mainPage.StatusTxt.Text = "";
                MainPage.mainPage.TestIsInProgress = _stethoscopelungs;
            if (_stethoscopelungs)
            {
                MainPage.mainPage.SMCCommChannel.SendMessage(string.Format(CommunicationCommands.STARTSTLUNGS));
                TbxSeatBackSet.BorderBrush = GetColorFromHexa("#FFC10D");
                TbxSeatBackSet.BorderThickness = new Thickness(0, 0, 0, 10);
                // BtnSthethoscopeLungs.Background = GetColorFromHexa("#FFBF00"); //_stethoscopeChest ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            }

            double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;
            // StethoscopeLungs.StlHeight = ht;
            StethoscopeLungsResult.Height = ht * 2;//gridInstrumentPanel.ActualHeight;
            StethoscopeLungsResult.Width = wdth * 2;//gridInstrumentPanel.ActualWidth; ;
            ResulStethoscopelungPopup.IsOpen = _stethoscopelungs;
            //  StethoscopeLungs
        }

        private void RecordStethocopeStream()
        {
            try
            {
                //  if(isStarted  )
                _StethoscopeRx.RecordRx();
            }
            catch (Exception ex)
            {
                MainPage.mainPage.LogExceptions(ex.Message);
            }
        }
        private void TxtTmpUnitbtn_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}






