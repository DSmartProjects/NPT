using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VideoKallMCCST.Communication;
using VideoKallMCCST.Helpers;
using VideoKallMCCST.Model;
using VideoKallMCCST.Results;
using VideoKallMCCST.Stethoscope;
using VideoKallMCCST.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace VideoKallMCCST.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestPanelExpander : Page
    {
        string height = string.Empty;
        string weight = string.Empty;
        string dia = string.Empty;
        string sys = string.Empty;
        string glucoResult = string.Empty;
        string mode = string.Empty;
        string pulseoxispo2 = string.Empty;
        string pulseoxipulse = string.Empty;
        string tmpMode = string.Empty;
        HttpClientManager _httpClient = null;
        PMMConfiguration _configuration = null;
        public static TestPanelExpander TestPanelExp;
        FlowVolumeData flowVolume = new FlowVolumeData();
        VCResults vcResults = new VCResults();
        Spirometeruc spirometeruc = new Spirometeruc();
        BitmapImage bitmapImage = new BitmapImage();
        WriteableBitmap bitMap = null;
        byte[] buffer = null;
        OtoscopeimageViewer oto = new OtoscopeimageViewer();
        StethoscopeChestInstructions sethChest = new StethoscopeChestInstructions();
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
            ShowHideWeightdata(false);
            ShowHideHeightdata(false);
            MainPage.mainPage.NextPatient += NextPatient;
            MainPage.mainPage.MicroscopeStatus += MicroscopeStatus;

            MainPage.mainPage.Thermostatusdelegate += UpdateThermoStatus;
            MainPage.mainPage.CASResult += CasNotification;
            MainPage.mainPage.SeatReclineMsg += AdjustSeatReclination;
            MainPage.mainPage.SeatHeightAdjust += SeatBackIntegration;
            MainPage.mainPage.Heightdelegate += Heightdelegate;
            MainPage.mainPage.Weightdelegate += Weightdelegate;
            _httpClient = VideoKallLoginPage.LoginPage.HttpClient;
            _configuration = VideoKallLoginPage.LoginPage._loginVM.PMMConfig;
            _httpClient.basePMM_APIUrl = _configuration?.API_URL;
            _httpClient.base_APIUrl = _configuration?.TestResultAPI_URL;

        }
        async void Heightdelegate(bool status)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (MainPage.mainPage.Heightstatus)
                {
                    grdHeight.BorderBrush = GetColorFromHexa("#E96056");
                    grdHeight.BorderThickness = new Thickness(0, 0, 0, 10);
                    MainPage.mainPage.Heightstatus = false;
                    TxtResultHeight.Text = String.Empty;
                    TxtLableHeight.Visibility = Visibility.Collapsed;
                }
            });
        }
        async void Weightdelegate(bool status)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (MainPage.mainPage.Weightstatus)
                {
                    grdWeight.BorderBrush = GetColorFromHexa("#E96056");
                    grdWeight.BorderThickness = new Thickness(0, 0, 0, 10);
                    MainPage.mainPage.Weightstatus = false;
                    TxtResultWeight.Text = string.Empty;
                    TxtResultBMI.Text = string.Empty;
                    TxtLableWeight.Visibility = Visibility.Collapsed;
                    TxtLabelBMI.Visibility = Visibility.Collapsed;

                }
            });
        }
        private  string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                    sb.Append(".");
                }
            }
            return sb.ToString().TrimEnd('.');
        }
        async void CasNotification(string message, int devicecode, int isresultornotificationmsg)
        {
            if (isresultornotificationmsg == 1)
                return;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                switch (devicecode)
                {
                    case 1:
                        ShowHideHeightdata(true);
                        grdHeight.BorderBrush = GetColorFromHexa("#34CBA8");
                        grdHeight.BorderThickness = new Thickness(0, 0, 0, 10);
                        TxtResultHeight.Text =" "+message;
                        if (MainPage.mainPage.HeightMeasureUnit == 1)
                        {
                            height = RemoveSpecialCharacters(message);
                        }
                        else
                        {
                            string[] res = message.Split(' ');
                            height = res[0];
                        }
                        break;
                    case 2:
                        if (!(string.IsNullOrEmpty(height)))
                        {
                            ShowHideWeightdata(true);
                            grdWeight.BorderBrush = GetColorFromHexa("#34CBA8");
                            grdWeight.BorderThickness = new Thickness(0, 0, 0, 10);
                            string[] resW = message.Split('l');
                            TxtResultWeight.Text = " " +resW[0]+" " + "l" + resW[1];
                            weight = resW[0];
                            if (!(string.IsNullOrEmpty(height)) && !(string.IsNullOrEmpty(weight)) && Convert.ToDouble(weight) > 0.0 && Convert.ToDouble(height) > 0.0)
                            {
                                BMICalculation("lb", Convert.ToDouble(height), Convert.ToDouble(weight));
                            }
                        }
                        break;

                }
            });
        }

        void BMICalculation(string type, double height, double weight)
        {
            double inches = 0.0;
            double bmi = 0.0;
            //Weight in kg
            if (type == "kg")
            {
                bmi = weight / (height * height);
                TxtResultBMI.Text = " " + Convert.ToString(Math.Round(bmi, 2));
            }
            //Weight in lb(Pounds)
            if (type == "lb")
            {
                if (MainPage.mainPage.HeightMeasureUnit == 1)
                {
                    double cent = 30.48 * height;
                    inches = cent / 2.54;
                    bmi = weight * 703 / (inches * inches);
                }
                else
                {
                    inches = height / 2.54;
                    bmi = weight * 703 / (inches * inches);
                }

                TxtResultBMI.Text = " " + Convert.ToString(Math.Round(bmi, 2));
            }
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
                 || ResulOtoscopePopup.IsOpen || ResulDermascopePopup.IsOpen || Resultglucopopup.IsOpen ||
                 Resultspiropopup.IsOpen || Resultstethoscopechestpopup.IsOpen || ResulStethoscopelungPopup.IsOpen;
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

        void ShowHideHeightdata(bool visible)
        {
            if (visible)
            {
                TxtLableHeight.Visibility = Visibility.Visible;
            }
            else
            {
                TxtLableHeight.Visibility = Visibility.Collapsed;
                //TxtResultHeight.Text = "";
            }

        }

        void ShowHideWeightdata(bool visible)
        {
            if (visible)
            {
                TxtLableWeight.Visibility = Visibility.Visible;
                TxtLabelBMI.Visibility = Visibility.Visible;
            }
            else
            {
                TxtLableWeight.Visibility = Visibility.Collapsed;
                TxtLabelBMI.Visibility = Visibility.Collapsed;
                //TxtResultWeight.Text = "";
                //TxtResultBMI.Text = "";
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
                ShowHideWeightdata(false);
                ShowHideHeightdata(false);

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

                grdWeight.BorderBrush = GetColorFromHexa("#EEEEEE");
                grdWeight.BorderThickness = new Thickness(0, 0, 0, 10);

                grdHeight.BorderBrush = GetColorFromHexa("#EEEEEE");
                grdHeight.BorderThickness = new Thickness(0, 0, 0, 10);

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
                    grdGluco.BorderBrush = GetColorFromHexa("#E96056");
                    grdGluco.BorderThickness = new Thickness(0, 0, 0, 10);
                }

            });


        }

        async void UpdateNotification(object sender, CommunicationMsg msg)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                string[] res = msg.Msg.Split('>');
                switch (msg.Id)
                {
                    case DeviceResponseType.PULSEOXIMETERRESULT:

                        ShowHidePulseoximeterdata(true);
                        //BtnPulseoximeter.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                        grdPulse.BorderBrush = GetColorFromHexa("#34CBA8");
                        grdPulse.BorderThickness = new Thickness(0, 0, 0, 10);
                        TxtResultPulseOximeter.Text = " " + res[1].Split(':')[1] + "%";
                        pulseoxispo2 = res[1].Split(':')[1];
                        TxtResultPulseOximeterpulse.Text = " " + res[2].Split(':')[1] + "bpm";
                        pulseoxipulse = res[2].Split(':')[1];
                        //TxtResultPulseOximeterpulsedate.Text = res[4];
                        break;
                    case DeviceResponseType.GLUCORESULT:
                        ShowHideglucodata(true);
                        grdGluco.BorderBrush = GetColorFromHexa("#34CBA8");
                        grdGluco.BorderThickness = new Thickness(0, 0, 0, 10);
                        //BtnGlucometer.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                        //"GLUCMDRES>V:{0}>U:{1}>T:{2}>M:{3}>D:{4}>T:{5}";
                         glucoResult= res[1].Split(':')[1];
                         mode = res[3].Split(':')[1];
                        TxtResultgluco.Text = res[1].Split(':')[1] + " " + res[2].Split(':')[1]; ;                       
                        
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
                            tmpMode = res[2].Split(':')[1];
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
                        TxtSys.Text = res[2].Split(':')[1] + "/" + res[1].Split(':')[1] + " mmHg";
                        dia= res[1].Split(':')[1];
                        sys = res[2].Split(':')[1];
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
            ReadCASSettings();
            CASTimer();
        }


        bool BtnPulseoximeterToggle = false;
        private async void BtnPulseoximeter_Click(object sender, RoutedEventArgs e)
        {
            isTestResultOpened();
            if ((MainPage.mainPage.TestIsInProgress && !BtnPulseoximeterToggle) ||
                (!ConnectionCheck && !BtnPulseoximeterToggle) ||
                (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress)
                )
            {
                if (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress)
                    ShowDeployOperationinProgressMessage(Constants.PulseOximeter);
                return;
            }
            if (!MainPage.mainPage.PoddeployretractcmdStatus.isPodRetracted())
            {
                //pod not retracted yet
                if (!await ShowPodnotRetractedMessage())
                    return;
            }


            BtnPulseoximeterToggle = !BtnPulseoximeterToggle;
            MainPage.mainPage.TestIsInProgress = BtnPulseoximeterToggle;
            if (BtnPulseoximeterToggle)
            {
                //BtnPulseoximeter.Background = GetColorFromHexa("#FFBF00"); //BtnPulseoximeterToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
                grdPulse.BorderBrush = GetColorFromHexa("#FFC10D");
                grdPulse.BorderThickness = new Thickness(0, 0, 0, 10);
                TxtResultPulseOximeter.Text = string.Empty;
                TxtResultPulseOximeterpulse.Text = string.Empty;
                ShowHidePulseoximeterdata(false);
            }
            if (BtnPulseoximeterToggle == false && TxtResultPulseOximeter.Text != null && TxtResultPulseOximeterpulse.Text != null)
            {

                PulseOximeterTestResult pulseTestResult = new PulseOximeterTestResult();
                pulseTestResult.SpO2 = Convert.ToInt32(pulseoxispo2);
                pulseTestResult.HeartRate = Convert.ToInt32(pulseoxipulse);
                pulseTestResult.Patient = null;
                //pulseTestResult.PatientId = 16042;
                pulseTestResult.ChairId = 123456;
                pulseTestResult.CreatedBy = VideoKallLoginPage.LoginPage._loginVM.TokUserId;
                pulseTestResult.CreatedDate = DateTime.Now;
                pulseTestResult.PatientId = MainPage.VideoCallVM.PatientDetails != null && MainPage.VideoCallVM.PatientDetails.ID > 0 ? MainPage.VideoCallVM.PatientDetails.ID : 0;
                MainPage.mainPage.mainpagecontext.PulseResult = pulseTestResult;
                await VideoKallLoginPage.LoginPage.HttpClient.POST(MainPage.mainPage.mainpagecontext.PulseResult);

            }
            ResultPulseOximeterPopup.IsOpen = BtnPulseoximeterToggle;
            DeployRetractDevice(BtnPulseoximeterToggle, MainPage.mainPage.Podmapping.OximeterPodID);
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
        private async void BtnThermoMeter_Click(object sender, RoutedEventArgs e)
        {
            isTestResultOpened();

            if ((MainPage.mainPage.TestIsInProgress && !_thermotoggle) ||
                (!ConnectionCheck && !_thermotoggle) ||
                (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress)
                )
            {
                if (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress)
                    ShowDeployOperationinProgressMessage(Constants.Thermometer);

                return;
            }

            if (!MainPage.mainPage.PoddeployretractcmdStatus.isPodRetracted())
            {
                //pod not retracted yet
                if (!await ShowPodnotRetractedMessage())
                    return;
            }

            _thermotoggle = !_thermotoggle;

            MainPage.mainPage.TestIsInProgress = _thermotoggle;
            if (_thermotoggle)
            {
                //BtnThermoMeter.Background = GetColorFromHexa("#FFBF00"); // : new SolidColorBrush(Windows.UI.Colors.LightGray);
                gridThermo.BorderBrush = GetColorFromHexa("#FFC10D");
                gridThermo.BorderThickness = new Thickness(0, 0, 0, 10);
                TxtTemprature.Text = string.Empty;
                TblTemp.Visibility = Visibility.Collapsed;
            }
            if (_thermotoggle == false && !(string.IsNullOrEmpty(TxtTemprature.Text)))
            {
                ThermometerTestResult thermoResult = new ThermometerTestResult();
                thermoResult.Value = TxtTemprature.Text;               
                thermoResult.Patient = null;
                //thermoResult.PatientId = 16042;
                thermoResult.ChairId = 123456;
                thermoResult.Mode = tmpMode;
                thermoResult.CreatedDate = DateTime.Now;
                thermoResult.CreatedBy = VideoKallLoginPage.LoginPage._loginVM.TokUserId;
                thermoResult.PatientId = MainPage.VideoCallVM.PatientDetails != null && MainPage.VideoCallVM.PatientDetails.ID > 0 ? MainPage.VideoCallVM.PatientDetails.ID : 0;
                MainPage.mainPage.mainpagecontext.ThermoResult = thermoResult;
                await VideoKallLoginPage.LoginPage.HttpClient.POST(MainPage.mainPage.mainpagecontext.ThermoResult);

            }

            ResulThermoPopup.IsOpen = _thermotoggle;
            DeployRetractDevice(_thermotoggle, MainPage.mainPage.Podmapping.ThermoMeterPodID);
        }


        private string SelectDeviceTitle(string device) {

            string deviceTitle= string.Empty;
            if (device.Equals(Constants.BP_CUFF))
            {
                deviceTitle = Constants.BP_CUFF;
            }
            else if (device.Equals(Constants.PulseOximeter))
            {
                deviceTitle = Constants.PulseOximeter;
            }
            else if (device.Equals(Constants.Thermometer))
            {
                deviceTitle = Constants.Thermometer;
            }
            else if (device.Equals(Constants.Dermatoscope))
            {
                deviceTitle = Constants.Dermatoscope;
            }
            else if (device.Equals(Constants.Otoscope))
            {
                deviceTitle = Constants.Otoscope;
            }
            else if (device.Equals(Constants.Spirometer))
            {
                deviceTitle = Constants.Spirometer;
            }
            else if (device.Equals(Constants.GlucoseMonitor))
            {
                deviceTitle = Constants.GlucoseMonitor;
            }
            else if (device.Equals(Constants.ChestStethoscope))
            {
                deviceTitle = Constants.Deployment_Recline_Inprogress;
            }
            else if (device.Equals(Constants.SeatBackStethoscope))
            {
                deviceTitle = Constants.Deployment_Recline_Inprogress;
            }
            return deviceTitle;
        }

        bool _resultBpToggle = false;
        ContentDialog RetractInProgressMessageDlg = null;
        void ShowDeployOperationinProgressMessage()
        {
            int timeout = MainPage.mainPage.Podmapping.TimeOutPeriod - timeoutCount;
            RetractInProgressMessageDlg = new ContentDialog
            {
                Title = Constants.Deployment_Recline_Inprogress,
                Content = String.Format(Constants.Wait_Time, timeout > 0 ? timeout : 0),
                PrimaryButtonText = Constants.OK,
            };
            RetractInProgressMessageDlg.PrimaryButtonStyle = (Style)this.Resources["PurpleStyle"];
            var val = RetractInProgressMessageDlg.ShowAsync();


        }



        void ShowDeployOperationinProgressMessage(string deviceName)
        {
            int timeout = MainPage.mainPage.Podmapping.TimeOutPeriod - timeoutCount;
            Toast.ShowToast(SelectDeviceTitle(deviceName), String.Format(Constants.Wait_Time, timeout > 0 ? timeout : 0), timeout);
        }

        ContentDialog PodIsnotRetractedMsgdlg = null;
        async Task<bool> ShowPodnotRetractedMessage()
        {
            PodIsnotRetractedMsgdlg = new ContentDialog
            {
                Title = "Device is not retracted.",
                Content = Constants.MsgPodNotRetracted,
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No"
            };
            //PodIsnotRetractedMsgdlg.PrimaryButtonStyle = (Style)this.Resources["PurpleStyle"];
            var val = await PodIsnotRetractedMsgdlg.ShowAsync();
            if (val == ContentDialogResult.Primary)
                return true;
            else
                return false;
        }

        void DeployRetractDevice(bool deploy, string podID)
        {
            if (casTimer.IsEnabled && deploy)
            {
                casTimer.Stop();

            }
            if (deploy)
            {
                MainPage.mainPage.PoddeployretractcmdStatus.Reset();
                MainPage.mainPage.PoddeployretractcmdStatus.PodSelectionOperationStarted();
                //  casTimer main
                MainPage.mainPage.CommToDataAcq.SendMessageToDataacquistionapp(string.Format(CommunicationCommands.PODCMD, podID, "D"));
                casTimer.Start();
            }
            else
            {

            }

        }
        private async void BtnBP_Click(object sender, RoutedEventArgs e)
        {
            isTestResultOpened();

            if ((MainPage.mainPage.TestIsInProgress && !_resultBpToggle) ||
                (!ConnectionCheck && !_resultBpToggle) ||
               (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress))
            {
                if (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress)
                    ShowDeployOperationinProgressMessage(Constants.BP_CUFF);
                return;
            }

            if (!MainPage.mainPage.PoddeployretractcmdStatus.isPodRetracted())
            {
                //pod not retracted yet
                if (!await ShowPodnotRetractedMessage())
                    return;
            }

            _resultBpToggle = !_resultBpToggle;
            MainPage.mainPage.TestIsInProgress = _resultBpToggle;
            if (_resultBpToggle)
            {
                //BtnBP.Background = GetColorFromHexa("#FFBF00");//_resultBpToggle ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
                grBp.BorderBrush = GetColorFromHexa("#FFC10D");
                grBp.BorderThickness = new Thickness(0, 0, 0, 10);
                ShowHidebpdata(false);
                TxtSys.Text = string.Empty;
            }
            if (_resultBpToggle == false && TxtSys.Text != null)
            {
                var bpResult = TxtSys.Text;
                if (!string.IsNullOrEmpty(bpResult))
                {
                    BloodPressureTestResult bpTestResult = new BloodPressureTestResult();
                    bpTestResult.Cystolic = Convert.ToInt32(sys);
                    bpTestResult.Diastolic = Convert.ToDouble(dia);
                    bpTestResult.Patient = null;
                    bpTestResult.ChairId = 123456;
                    //bpTestResult.PatientId = 16042;
                    bpTestResult.CreatedDate = DateTime.Now;
                    bpTestResult.CreatedBy = VideoKallLoginPage.LoginPage._loginVM.TokUserId;
                    bpTestResult.PatientId = MainPage.VideoCallVM.PatientDetails != null && MainPage.VideoCallVM.PatientDetails.ID > 0 ? MainPage.VideoCallVM.PatientDetails.ID : 0;
                    MainPage.mainPage.mainpagecontext.BpResult = bpTestResult;
                    await VideoKallLoginPage.LoginPage.HttpClient.POST(MainPage.mainPage.mainpagecontext.BpResult);
                }
            }
            ResultBPPopup.IsOpen = _resultBpToggle;
            DeployRetractDevice(_resultBpToggle, MainPage.mainPage.Podmapping.BPCuffPodID);
        }
        public void updateTime()
        {
            //  THERMORESTULT
        }

        bool btnWeightToggle = false;
        private async void BtnWeight_Click(object sender, RoutedEventArgs e)
        {
            //ContentDialog noWeightDialog = null;
            isTestResultOpened();

            if ((MainPage.mainPage.TestIsInProgress && !btnWeightToggle) || (!ConnectionCheck && !btnWeightToggle))
                return;

            if (string.IsNullOrEmpty(height))
            {
                //noWeightDialog = new ContentDialog()
                //{
                //    Content = Constants.Measure_Height_First,
                //    ContentTemplate = (DataTemplate)this.Resources["ContentTemplateStyle"],
                //    PrimaryButtonText = Constants.OK,
                //};
                //noWeightDialog.PrimaryButtonStyle = (Style)this.Resources["PurpleStyle"];
                //var val = noWeightDialog.ShowAsync();
                // await noWeightDialog.ShowAsync();
                //noWifiDialog.Height = 100;
                //noWifiDialog.Width = 200;
                //noWifiDialog.CornerRadius = new CornerRadius(5, 5, 5, 5);
                //noWifiDialog.BorderThickness = new Thickness(1, 1, 1, 1);
                Toast.ShowToast("", Constants.Measure_Height_First);
                return;
            }            
            btnWeightToggle = !btnWeightToggle;

            MainPage.mainPage.TestIsInProgress = btnWeightToggle;
            if (btnWeightToggle)
            {
                grdWeight.BorderBrush = GetColorFromHexa("#FFC10D");
                grdWeight.BorderThickness = new Thickness(0, 0, 0, 10);
                ShowHideWeightdata(false);
                TxtResultWeight.Text = string.Empty;
                TxtResultBMI.Text = string.Empty;
            }

            if (btnWeightToggle == false && TxtResultWeight.Text != null)
            {
                var weightResult = TxtResultWeight.Text;
                if (!string.IsNullOrEmpty(weightResult))
                {
                    WeightTestResult weightTestResult = new WeightTestResult();
                    weightTestResult.Weight = Convert.ToDouble(weight);
                    weightTestResult.Patient = null;
                    //weightTestResult.PatientId = 16042;
                    weightTestResult.ChairId = 123456;
                    weightTestResult.CreatedBy = VideoKallLoginPage.LoginPage._loginVM.TokUserId;
                    weightTestResult.CreatedDate = DateTime.Now;
                    weightTestResult.PatientId = MainPage.VideoCallVM.PatientDetails != null && MainPage.VideoCallVM.PatientDetails.ID > 0 ? MainPage.VideoCallVM.PatientDetails.ID : 0;
                    MainPage.mainPage.mainpagecontext.WeightResult = weightTestResult;
                    await VideoKallLoginPage.LoginPage.HttpClient.POST(MainPage.mainPage.mainpagecontext.WeightResult);
                }
            }
            ResuWeightPopup.IsOpen = btnWeightToggle;
        }




        bool btnHeighttoggle = false;
        private async void BtnHeight_Click(object sender, RoutedEventArgs e)
        {

            isTestResultOpened();

            if ((MainPage.mainPage.TestIsInProgress && !btnHeighttoggle) ||
                (!ConnectionCheck && !btnHeighttoggle))
                return;

            btnHeighttoggle = !btnHeighttoggle;

            MainPage.mainPage.TestIsInProgress = btnHeighttoggle;
            if (btnHeighttoggle)
            {
                TxtLableHeight.Visibility = Visibility.Collapsed;
                TxtResultHeight.Text = string.Empty;
                grdHeight.BorderBrush = GetColorFromHexa("#FFC10D");
                grdHeight.BorderThickness = new Thickness(0, 0, 0, 10);
            }
            if(btnHeighttoggle==false && TxtResultHeight.Text!=null)
            {
                var HeightResult = TxtResultHeight.Text;              
                if (!string.IsNullOrEmpty(HeightResult))
                {
                    HeightTestResult heightTestResult = new HeightTestResult();
                    heightTestResult.Height = HeightResult;
                    heightTestResult.Patient = null;
                    //heightTestResult.PatientId = 16042;
                    heightTestResult.ChairId = 123456;
                    heightTestResult.CreatedBy = VideoKallLoginPage.LoginPage._loginVM.TokUserId;
                    heightTestResult.CreatedDate = DateTime.Now;
                    heightTestResult.PatientId = MainPage.VideoCallVM.PatientDetails != null && MainPage.VideoCallVM.PatientDetails.ID > 0 ? MainPage.VideoCallVM.PatientDetails.ID : 0;
                    MainPage.mainPage.mainpagecontext.heightResult = heightTestResult;
                    await VideoKallLoginPage.LoginPage.HttpClient.POST(MainPage.mainPage.mainpagecontext.heightResult);
                }
            }

            ResulHeightPopup.IsOpen = btnHeighttoggle;

            if(btnHeighttoggle)
            {
                MainPage.mainPage.HM_WMEvents?.Invoke("height", 1);
            }
        }

        bool _otoscopeToggle = false;
        private async void BtnOtoscope_Click(object sender, RoutedEventArgs e)
        {
            isTestResultOpened();

            if ((MainPage.mainPage.TestIsInProgress && !_otoscopeToggle) ||
                (!ConnectionCheck && !_otoscopeToggle) ||
                (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress)
                )
            {
                if (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress)
                    ShowDeployOperationinProgressMessage(Constants.Otoscope);
                return;
            }

            if (!MainPage.mainPage.PoddeployretractcmdStatus.isPodRetracted() && sender != null)
            {
                //pod not retracted yet
                if (!await ShowPodnotRetractedMessage())
                    return;
            }

            try
            {
                if (MainPage.mainPage.rootImageFolder == null)
                    await SetImagefolder();
                _otoscopeToggle = !_otoscopeToggle;
                MainPage.mainPage.TestIsInProgress = _otoscopeToggle;
                if (_otoscopeToggle)
                {
                    MainPage.mainPage.DigitalMicroscope?.Invoke(false);
                    MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.STARTOTOSCOPE);
                }

                if (_otoscopeToggle)
                {
                    TxtResultOtoscope.BorderBrush = GetColorFromHexa("#FFC10D");
                    TxtResultOtoscope.BorderThickness = new Thickness(0, 0, 0, 10);
                }

                if (_otoscopeToggle == false)
                {
                   // DisplayImage(oto.ImageName);
                    OtoscopeTestResult otoscope = new OtoscopeTestResult();
                    otoscope.ChairId = 123456;
                    otoscope.CreatedBy = VideoKallLoginPage.LoginPage._loginVM.TokUserId;
                    otoscope.CreatedDate = DateTime.Now;
                    //otoscope.PatientId = 16042;
                    otoscope.Image = oto.buffer;
                    otoscope.PatientId = MainPage.VideoCallVM.PatientDetails != null && MainPage.VideoCallVM.PatientDetails.ID > 0 ? MainPage.VideoCallVM.PatientDetails.ID : 0;
                    MainPage.mainPage.mainpagecontext.OtoResult = otoscope;
                    await VideoKallLoginPage.LoginPage.HttpClient.POST(MainPage.mainPage.mainpagecontext.OtoResult);
                }


                ResulOtoscopePopup.IsOpen = _otoscopeToggle;

                DeployRetractDevice(_otoscopeToggle, MainPage.mainPage.Podmapping.OtoscopePodID);
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
            if ((MainPage.mainPage.TestIsInProgress && !_dermascopeToggle) ||
                (!ConnectionCheck && !_dermascopeToggle) ||
                (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress)
                )
            {
                if (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress)
                    ShowDeployOperationinProgressMessage(Constants.Dermatoscope);
                return;
            }

            if (!MainPage.mainPage.PoddeployretractcmdStatus.isPodRetracted() && sender != null)
            {
                //pod not retracted yet
                if (!await ShowPodnotRetractedMessage())
                    return;
            }

            try
            {
                if (MainPage.mainPage.rootImageFolder == null)
                    await SetImagefolder();

                _dermascopeToggle = !_dermascopeToggle;
                MainPage.mainPage.TestIsInProgress = _dermascopeToggle;
                if (_dermascopeToggle)
                {
                    MainPage.mainPage.DigitalMicroscope?.Invoke(true);
                    MainPage.mainPage.SMCCommChannel.SendMessage(CommunicationCommands.STARTDERMO);

                }
                if (_dermascopeToggle)
                {
                    TxtResultDermascope.BorderBrush = GetColorFromHexa("#FFC10D");
                    TxtResultDermascope.BorderThickness = new Thickness(0, 0, 0, 10);
                }
                if (_dermascopeToggle == false)
                {
                   // DisplayImage(oto.ImageName);
                    DermatoscopeTestResult Dermoscope = new DermatoscopeTestResult();
                    Dermoscope.ChairId = 123456;
                    Dermoscope.CreatedBy = VideoKallLoginPage.LoginPage._loginVM.TokUserId;
                    Dermoscope.CreatedDate = DateTime.Now;
                    //Dermoscope.PatientId = 16042;
                    Dermoscope.Image = oto.buffer;
                    Dermoscope.PatientId = MainPage.VideoCallVM.PatientDetails != null && MainPage.VideoCallVM.PatientDetails.ID > 0 ? MainPage.VideoCallVM.PatientDetails.ID : 0;
                    MainPage.mainPage.mainpagecontext.DermoResult = Dermoscope;
                    await VideoKallLoginPage.LoginPage.HttpClient.POST(MainPage.mainPage.mainpagecontext.DermoResult);
                }
                ResulDermascopePopup.IsOpen = _dermascopeToggle;
                DeployRetractDevice(_dermascopeToggle, MainPage.mainPage.Podmapping.DermascopePodID);
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
                    //BtnOtoscope.Background = issuccess ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.Red);
                    TxtResultOtoscope.BorderBrush = issuccess ? GetColorFromHexa("#34CBA8") : GetColorFromHexa("#E96056");
                    TxtResultOtoscope.BorderThickness = new Thickness(0, 0, 0, 10);

                }

                if (_dermascopeToggle)
                {
                    TxtResultDermascope.BorderBrush = issuccess ? GetColorFromHexa("#34CBA8") : GetColorFromHexa("#E96056");
                    TxtResultDermascope.BorderThickness = new Thickness(0, 0, 0, 10);

                }

            });

        }

        bool _ekgToggle = false;
        private void BtnEKG_Click(object sender, RoutedEventArgs e)
        {
        }


        bool _glucoToggle = false;
        private async void BtnGlucometer_Click(object sender, RoutedEventArgs e)
        {
            isTestResultOpened();
            if (!ConnectionCheck && !_glucoToggle)
                return;

            if (MainPage.mainPage.TestIsInProgress && !_glucoToggle)
                return;

            if (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress)
            {
                ShowDeployOperationinProgressMessage(Constants.GlucoseMonitor);
                return;
            }

            if (!MainPage.mainPage.PoddeployretractcmdStatus.isPodRetracted())
            {
                //pod not retracted yet
                if (!await ShowPodnotRetractedMessage())
                    return;
            }
            _glucoToggle = !_glucoToggle;

            if (_glucoToggle)
            {
                //MainPage.mainPage.StatusTxt.Text = "";
                MainPage.mainPage.ResetGluco?.Invoke();
            }

            MainPage.mainPage.TestIsInProgress = _glucoToggle;
            if (_glucoToggle)
            {
                ShowHideglucodata(false);
                TxtResultgluco.Text = string.Empty;
                grdGluco.BorderBrush = GetColorFromHexa("#FFC10D");
                grdGluco.BorderThickness = new Thickness(0, 0, 0, 10);
            }
            if (_glucoToggle == false && TxtResultgluco.Text != null && (!string.IsNullOrEmpty(glucoResult)))
            {
                GlucoseMonitorTestResult glucoTestResult = new GlucoseMonitorTestResult();
                glucoTestResult.Mode = mode;
                glucoTestResult.Value = Convert.ToDouble(glucoResult);
                glucoTestResult.Patient = null;
                //glucoTestResult.PatientId = 16042;
                glucoTestResult.ChairId = 123456;
                glucoTestResult.CreatedDate = DateTime.Now;
                glucoTestResult.CreatedBy = VideoKallLoginPage.LoginPage._loginVM.TokUserId;
                glucoTestResult.PatientId = MainPage.VideoCallVM.PatientDetails != null && MainPage.VideoCallVM.PatientDetails.ID > 0 ? MainPage.VideoCallVM.PatientDetails.ID : 0; ;
                MainPage.mainPage.mainpagecontext.GlucoResult = glucoTestResult;
                await VideoKallLoginPage.LoginPage.HttpClient.POST(MainPage.mainPage.mainpagecontext.GlucoResult);
            }
            Resultglucopopup.IsOpen = _glucoToggle;
            DeployRetractDevice(_glucoToggle, MainPage.mainPage.Podmapping.GlucomonitorPodID);
        }

        bool _spirometerToggle = false;
        private async void BtnSpirometer_Click(object sender, RoutedEventArgs e)
        {
            isTestResultOpened();
            if ((MainPage.mainPage.TestIsInProgress && !_spirometerToggle) || (!ConnectionCheck && !_spirometerToggle))
                return;

            if (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress)
            {
                ShowDeployOperationinProgressMessage(Constants.Spirometer);
                return;
            }

            if (!MainPage.mainPage.PoddeployretractcmdStatus.isPodRetracted() && sender != null)
            {
                //pod not retracted yet
                if (!await ShowPodnotRetractedMessage())
                    return;
            }

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

            //double wdth = gridInstrumentPanel.ColumnDefinitions[0].ActualWidth;
            //double ht = gridInstrumentPanel.RowDefinitions[0].ActualHeight;
            int iterationFVC = 0;
            if (_spirometerToggle == false && spirometeruc.FVCResultsColl != null && spirometeruc.FVCResultsColl.Count>0)
            {
                foreach (var parm in spirometeruc.FVCResultsColl)
                {
                    iterationFVC++;
                    if (iterationFVC < spirometeruc.FVCResultsColl.Count)
                    {
                        if ((!string.IsNullOrEmpty(parm.MeasureUnit)) && (!string.IsNullOrEmpty(parm.MeasuredValue)))
                        {
                            SpirometerTestResult spiroResult = new SpirometerTestResult();
                            spiroResult.MeasuredUnit = parm.MeasureUnit;
                            spiroResult.MeasuredValue = Convert.ToDouble(parm.MeasuredValue);
                            spiroResult.ParameterType = parm.ParameterType;
                            spiroResult.Code = parm.Code;
                            spiroResult.TestType = "FVC";
                            spiroResult.Name = parm.Name;
                            spiroResult.Patient = null;
                            spiroResult.ChairId = 123456;
                            spiroResult.CreatedDate = DateTime.Now;
                            spiroResult.CreatedBy = VideoKallLoginPage.LoginPage._loginVM.TokUserId;
                            spiroResult.PatientId = MainPage.VideoCallVM.PatientDetails != null && MainPage.VideoCallVM.PatientDetails.ID > 0 ? MainPage.VideoCallVM.PatientDetails.ID : 0;
                            MainPage.mainPage.mainpagecontext.SpiroResult = spiroResult;
                            await VideoKallLoginPage.LoginPage.HttpClient.POST(MainPage.mainPage.mainpagecontext.SpiroResult);
                        }

                    }
                }
            }
            int iterationVC = 0;
            if (_spirometerToggle == false && spirometeruc.VCResultsColl != null && spirometeruc.VCResultsColl.Count > 0)
            {
                foreach (var parm in spirometeruc.VCResultsColl)
                {
                    iterationVC++;
                    if (iterationVC < spirometeruc.FVCResultsColl.Count)
                    {
                        if ((!string.IsNullOrEmpty(parm.MeasureUnit)) && (!string.IsNullOrEmpty(parm.MeasuredValue)))
                        {
                            SpirometerTestResult spiroResult = new SpirometerTestResult();
                            spiroResult.MeasuredUnit = parm.MeasureUnit;
                            spiroResult.MeasuredValue = Convert.ToDouble(parm.MeasuredValue);
                            spiroResult.Name = parm.Name;
                            spiroResult.Patient = null;
                            spiroResult.ChairId = 123456;
                            spiroResult.TestType = "VC";
                            spiroResult.Code = parm.Code;
                            spiroResult.ParameterType = parm.ParameterType;
                            spiroResult.CreatedDate = DateTime.Now;
                            spiroResult.CreatedBy = VideoKallLoginPage.LoginPage._loginVM.TokUserId;
                            spiroResult.PatientId = MainPage.VideoCallVM.PatientDetails != null && MainPage.VideoCallVM.PatientDetails.ID > 0 ? MainPage.VideoCallVM.PatientDetails.ID : 0;
                            MainPage.mainPage.mainpagecontext.SpiroResult = spiroResult;
                            await VideoKallLoginPage.LoginPage.HttpClient.POST(MainPage.mainPage.mainpagecontext.SpiroResult);
                        }
                    }
                }
                //Toast.ShowToast("", "Successfully Saved.");
            }

            //CtrlspiroResult.Height = ht * 6;//gridInstrumentPanel.ActualHeight;
            //CtrlspiroResult.Width = wdth * 4;//gridInstrumentPanel.ActualWidth; ;

            Resultspiropopup.IsOpen = _spirometerToggle;
            DeployRetractDevice(_spirometerToggle, MainPage.mainPage.Podmapping.SpirometerPodID);
        }

        private async void SpirometerStatus(bool issuccess)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // BtnSpirometer.Background = issuccess ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.Red);
                TxtResultspiro.BorderBrush = issuccess ? GetColorFromHexa("#34CBA8") : GetColorFromHexa("#E96056");
                TxtResultspiro.BorderThickness = new Thickness(0, 0, 0, 10);

            });
        }

        void SpirometerDone()
        {
            BtnSpirometer_Click(null, null);
        }

        bool _stethoscopeChest = false;

        private async void BtnSthethoscope_Click(object sender, RoutedEventArgs e)
        {
            isTestResultOpened();
            if (MainPage.mainPage.isStethoscopeStreaming || (!ConnectionCheck && !_stethoscopeChest))
                return;


            if (MainPage.mainPage.TestIsInProgress && !_stethoscopeChest)
                return;
            if (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress)
            {
                ShowDeployOperationinProgressMessage(Constants.ChestStethoscope);
                return;
            }

            if (!MainPage.mainPage.PoddeployretractcmdStatus.isPodRetracted())
            {
                //pod not retracted yet
                if (!await ShowPodnotRetractedMessage())
                    return;
            }

            _stethoscopeChest = !_stethoscopeChest;
            MainPage.mainPage.IsStethescopeChest = _stethoscopeChest;
            MainPage.mainPage.TestIsInProgress = _stethoscopeChest;
            if (_stethoscopeChest)
            {
                TxtResultstethoscopechest.BorderBrush = GetColorFromHexa("#FFC10D");
                TxtResultstethoscopechest.BorderThickness = new Thickness(0, 0, 0, 10);
            }

            Resultstethoscopechestpopup.IsOpen = _stethoscopeChest;

            DeployRetractDevice(_stethoscopeChest, MainPage.mainPage.Podmapping.StethoscopeChestPodID);
            if (_stethoscopeChest)// && !MainPage.mainPage.isStethoscopeReadystreaming)
            {
                MainPage.mainPage.SMCCommChannel.SendMessage(string.Format(CommunicationCommands.STARTSTCHEST));               

            }
            if (_stethoscopeChest == false)
            {
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;             
                if (MainPage.mainPage.targetpath!=null)
                {
                    ChestStethoscopeTestResult chestStethoscopeResult = new ChestStethoscopeTestResult();
                    chestStethoscopeResult.Patient = null;
                    chestStethoscopeResult.ChairId = 123456;
                    chestStethoscopeResult.CreatedDate = DateTime.Now;
                    chestStethoscopeResult.CreatedBy = VideoKallLoginPage.LoginPage._loginVM.TokUserId;
                    chestStethoscopeResult.PatientId = MainPage.VideoCallVM.PatientDetails != null && MainPage.VideoCallVM.PatientDetails.ID > 0 ? MainPage.VideoCallVM.PatientDetails.ID : 0;
                    MainPage.mainPage.mainpagecontext.ChestResult = chestStethoscopeResult;
                    chestStethoscopeResult.Recording_Path = MainPage.mainPage.targetpath;
                    await VideoKallLoginPage.LoginPage.HttpClient.POST(MainPage.mainPage.mainpagecontext.ChestResult);
                }
            }
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
            MainPage.mainPage.TestIsInProgress = _stethoscopelungs;
            if (_stethoscopelungs)
            {
                MainPage.mainPage.SMCCommChannel.SendMessage(string.Format(CommunicationCommands.STARTSTLUNGS));
                TbxSeatBackSet.BorderBrush = GetColorFromHexa("#FFC10D");
                TbxSeatBackSet.BorderThickness = new Thickness(0, 0, 0, 10); //_stethoscopeChest ? new SolidColorBrush(Windows.UI.Colors.LightSeaGreen) : new SolidColorBrush(Windows.UI.Colors.LightGray);
            }
            ResulStethoscopelungPopup.IsOpen = _stethoscopelungs;
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

        DispatcherTimer casTimer = null;
        void CASTimer()
        {
            casTimer = new DispatcherTimer();
            casTimer.Tick += CasTimer_Tick;
            casTimer.Interval = new TimeSpan(0, 0, 1);
            MainPage.mainPage.CASResult += CasNotification;
        }

        async void SeatBackIntegration(bool upordown)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (upordown)
                {
                    SeatUp();
                }
                else
                {
                    SeatDown();
                }

            });
        }

        void SeatUp()
        {
            if (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress)
            {
                ShowDeployOperationinProgressMessage(Constants.SeatBackStethoscope);
                return;
            }
            int val = _seatUpDownValue + MainPage.mainPage.Podmapping.SeatUpDownStepValue;
            _seatUpDownValue = (val) > 100 ? 100 : val;

            if (casTimer.IsEnabled)
            {
                casTimer.Stop();
                MainPage.mainPage.PoddeployretractcmdStatus.Reset();
            }

            MainPage.mainPage.PoddeployretractcmdStatus.PodSelectionOperationStarted();
            //  casTimer main
            string strval = (_seatUpDownValue.ToString()).PadLeft(2, '0');
            MainPage.mainPage.CommToDataAcq.SendMessageToDataacquistionapp(string.Format(CommunicationCommands.SeatBackheightCmd, strval));
            casTimer.Start();
        }

        void SeatDown()
        {
            if (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress)
            {
                ShowDeployOperationinProgressMessage(Constants.SeatBackStethoscope);
                return;
            }
            int val = _seatUpDownValue - MainPage.mainPage.Podmapping.SeatUpDownStepValue;
            _seatUpDownValue = (val) < 0 ? 0 : val;

            if (casTimer.IsEnabled)
            {
                casTimer.Stop();
                MainPage.mainPage.PoddeployretractcmdStatus.Reset();
            }

            MainPage.mainPage.PoddeployretractcmdStatus.PodSelectionOperationStarted();
            //  casTimer main
            string strval = (_seatUpDownValue.ToString()).PadLeft(2, '0');
            MainPage.mainPage.CommToDataAcq.SendMessageToDataacquistionapp(string.Format(CommunicationCommands.SeatBackheightCmd, strval));
            casTimer.Start();
        }
        async void AdjustSeatReclination(bool recline)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (recline)
                {
                    Recline();
                }
                else
                {
                    Decline();
                }

            });
        }

        public void Recline()
        {
            if (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress)
            {
                ShowDeployOperationinProgressMessage(Constants.Deployment_Recline_Inprogress);
                return;
            }
            int val = _reclinevalue + MainPage.mainPage.Podmapping.ReclineStepValue;
            _reclinevalue = (val) > 100 ? 100 : val;

            if (casTimer.IsEnabled)
            {
                casTimer.Stop();
                MainPage.mainPage.PoddeployretractcmdStatus.Reset();
            }

            MainPage.mainPage.PoddeployretractcmdStatus.PodSelectionOperationStarted();
            //  casTimer main
            string strval = (_reclinevalue.ToString()).PadLeft(2, '0');
            MainPage.mainPage.CommToDataAcq.SendMessageToDataacquistionapp(string.Format(CommunicationCommands.SeatReclineCmd, strval));
            casTimer.Start();
        }

        public void Decline()
        {
            if (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployInProgress)
            {
                ShowDeployOperationinProgressMessage(Constants.Deployment_Recline_Inprogress);
                return;
            }

            int val = _reclinevalue - MainPage.mainPage.Podmapping.ReclineStepValue;
            _reclinevalue = (val) < 0 ? 0 : val;

            if (casTimer.IsEnabled)
            {
                casTimer.Stop();
                MainPage.mainPage.PoddeployretractcmdStatus.Reset();
            }

            MainPage.mainPage.PoddeployretractcmdStatus.PodSelectionOperationStarted();
            //  casTimer main
            string strval = (_reclinevalue.ToString()).PadLeft(2, '0');
            MainPage.mainPage.CommToDataAcq.SendMessageToDataacquistionapp(string.Format(CommunicationCommands.SeatReclineCmd, strval));
            casTimer.Start();
        }
        private void CasTimer_Tick(object sender, object e)
        {

            if (MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployResponseReceived)
            {
                casTimer.Stop();

                timeoutCount = 0;
                return;
            }
            else
            {
                MainPage.mainPage.CASResult?.Invoke("Waiting for resp: " + timeoutCount.ToString() + " sec", 4, 1);
            }

            if (timeoutCount > MainPage.mainPage.Podmapping.TimeOutPeriod)
            {
                casTimer.Stop();
                timeoutCount = 0;

                MainPage.mainPage.CASResult?.Invoke("No Response Received.", 4, 1);
                MainPage.mainPage.PoddeployretractcmdStatus.Reset();
            }

            timeoutCount++;
        }

        int _reclinevalue = 0;
        int timeoutCount = 0;
        int _seatUpDownValue = 0;
        string CasConfigFile = "CASConfig.txt";
        public async void ReadCASSettings()
        {
            try
            {
                PodMapping podmap = new PodMapping();
                podmap.BPCuffPodID = "1";
                podmap.OximeterPodID = "2";
                podmap.ThermoMeterPodID = "3";
                podmap.DermascopePodID = "4";
                podmap.OtoscopePodID = "5";
                podmap.SpirometerPodID = "6";
                podmap.GlucomonitorPodID = "7";
                podmap.StethoscopeChestPodID = "8";
                podmap.TimeOutPeriod = 15;
                podmap.ReclineStepValue = 5;
                podmap.SeatUpDownStepValue = 5;
                MainPage.mainPage.Podmapping = podmap;
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                if (!File.Exists(localFolder.Path + "\\" + CasConfigFile))
                {
                    return;
                }

                Windows.Storage.StorageFile IpAddressFile = await localFolder.GetFileAsync(CasConfigFile);
                var alltext = await Windows.Storage.FileIO.ReadLinesAsync(IpAddressFile);
                foreach (var line in alltext)
                {
                    string[] data = line.Split(':');
                    switch (data[0])
                    {
                        case "BPCuffPodID":
                            podmap.BPCuffPodID = data[1].Trim();
                            break;
                        case "OximeterPodID":
                            podmap.OximeterPodID = data[1].Trim();
                            break;
                        case "ThermoMeterPodID":
                            podmap.ThermoMeterPodID = data[1].Trim();
                            break;
                        case "DermascopePodID":
                            podmap.DermascopePodID = data[1].Trim();
                            break;
                        case "OtoscopePodID":
                            podmap.OtoscopePodID = data[1].Trim();
                            break;
                        case "SpirometerPodID":
                            podmap.SpirometerPodID = data[1].Trim();
                            break;
                        case "GlucomonitorPodID":
                            podmap.GlucomonitorPodID = data[1].Trim();
                            break;
                        case "StethoscopeChestPodID":
                            podmap.StethoscopeChestPodID = data[1].Trim();
                            break;
                        case "TimeOutPeriod":
                            podmap.TimeOutPeriod = int.Parse(data[1].Trim());
                            break;
                        case "ReclineStepValue":
                            podmap.ReclineStepValue = int.Parse(data[1].Trim());
                            break;
                        case "SeatBackHeightStepValue":
                            podmap.SeatUpDownStepValue = int.Parse(data[1].Trim());
                            break;
                    }

                }
                MainPage.mainPage.Podmapping = podmap;
            }
            catch(Exception ex)
            {
                string s = ex.Message;
            }

        }
        async void DisplayImage(string imageName)
        {
            try
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(MainPage.mainPage.rootImageFolder.Path);
                StorageFile storageFile = await folder.GetFileAsync(imageName);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    // Set the image source to the selected bitmap

                    if (storageFile != null)
                    {
                        // Ensure the stream is disposed once the image is loaded
                        using (IRandomAccessStream fileStream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                        {
                           // bitMap = new WriteableBitmap(bitmapImage.PixelWidth, bitmapImage.PixelHeight);
                            await bitmapImage.SetSourceAsync(fileStream);
                            Convertion(fileStream);
                        }
                    }
                    //ImageToByeArray(bitMap);

                });
            }
            catch (Exception ex)
            {
                MainPage.mainPage.LogExceptions(ex.Message);
            }
        }

        private byte[] ImageToByeArray(WriteableBitmap bitMap)
        {
            using (Stream stream = bitMap.PixelBuffer.AsStream())
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                buffer = memoryStream.ToArray();
            }
            return buffer;
        }

        async Task<byte[]> Convertion(IRandomAccessStream s)
        {
            using (var dr = new DataReader(s.GetInputStreamAt(0)))
            {
                buffer = new byte[s.Size];
                await dr.LoadAsync((uint)s.Size);
                dr.ReadBytes(buffer);
            }
            return buffer;
        }
    }

}






