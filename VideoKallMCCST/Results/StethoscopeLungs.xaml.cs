using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VideoKallMCCST.Communication;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class StethoscopeLungs : UserControl
    {
        DispatcherTimer casTimer = null;

        public StethoscopeLungs()
        {
            this.InitializeComponent();
            this.DataContext = this;
            MainPage.mainPage.ResetSTLungs += Clearall;
            MainPage.mainPage.StethoscopeNotification += UpdateNotification;
            casTimer = new DispatcherTimer();
            casTimer.Tick += CasTimer_Tick;
            casTimer.Interval = new TimeSpan(0, 0, 1);
            MainPage.mainPage.CASResult += CasNotification;
        }
        async void CasNotification(string message, int devicecode, int isresultornotificationmsg)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (devicecode == 3 && isresultornotificationmsg == 1)
                {
                    StStatus.Text = message;
                    deployRetractoprtstionStarted = false;
                    BtnStart.IsEnabled = true;
                }
                else if (devicecode == 3 && isresultornotificationmsg == 2)
                {
                    StStatus.Text = message;
                    BtnStart.IsEnabled = true;

                }
                else if (devicecode == 3 && isresultornotificationmsg == 3)
                {
                    StStatus.Text = message;

                }
                else if (devicecode == 3 && isresultornotificationmsg == 4)
                {
                    BtnStart.IsEnabled = true;

                }
                else if (devicecode == 4 && isresultornotificationmsg == 1)
                {
                    StStatus.Text = message;

                }
            });
        }
        private void BtnDone_Click(object sender, RoutedEventArgs e)
        {
            if ((selectedIndex == -1 && !isoperationStarted))

                return;

            StartStopST();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if ((selectedIndex == -1 && !isoperationStarted) ||
               MainPage.mainPage.PoddeployretractcmdStatus.IsPodDeployedRetractInProgress)

                return;

            StartStopST();
        }
        void DeployST(int id)
        {
            string strID = string.Format("{0}{1}", (id / 8) + 1, (id % 8) + 1); //( id+1).ToString().PadLeft(2, '1');
            MainPage.mainPage.CommToDataAcq.SendMessageToDataacquistionapp(String.Format(CommunicationCommands.SeatBackSTCmd, strID, "D"));

        }

        void RetractST(int id)
        {
            string strID = string.Format("{0}{1}", (id / 8) + 1, (id % 8) + 1);
            MainPage.mainPage.CommToDataAcq.SendMessageToDataacquistionapp(String.Format(CommunicationCommands.SeatBackSTCmd, strID, "R"));
        }

        int tmptimpout = 0;
        bool deployRetractoprtstionStarted = false;
        private void CasTimer_Tick(object sender, object e)
        {
            if (MainPage.mainPage.isSTDeployed)
            {
                tmptimpout = 0;
                deployRetractoprtstionStarted = false;
                MainPage.mainPage.isSTDeployed = false;
                casTimer.Stop();
                MainPage.mainPage.StethoscopeStartStop.Invoke("startST", 1);
                SetBtnColor(selectedIndex, 1);
                CasNotification("", 3, 4);
            }


            if (tmptimpout > MainPage.mainPage.Podmapping.TimeOutPeriod)
            {
                MainPage.mainPage.isSTDeployed = false;

                tmptimpout = 0;
                casTimer.Stop();
                CasNotification("No Response, Timed-out", 3, 2);
                deployRetractoprtstionStarted = false;
                if (isoperationStarted)
                {
                    isoperationStarted = !isoperationStarted;
                    BtnStart.Content = isoperationStarted ? "Stop streaming" : "Start streaming";
                }
            }
            else if (!deployRetractoprtstionStarted)
            {
                tmptimpout = 0;
                casTimer.Stop();
                CasNotification("", 3, 4);
            }
            else
            {
                CasNotification("Waiting for resp: " + tmptimpout.ToString() + " sec", 3, 3);
            }
            tmptimpout++;
        }

        void StartStopST()
        {
            if (deployRetractoprtstionStarted)
                return;

            isoperationStarted = !isoperationStarted;
            BtnStart.IsEnabled = false;
            //  BtnDone.Content = isoperationStarted ? "Stop" : "Start";
            BtnStart.Content = isoperationStarted ? "Stop streaming" : "Start streaming";
            if (isoperationStarted)
            {
                deployRetractoprtstionStarted = true;
                SetBtnColor(selectedIndex, 1);
                DeployST(currentStethescope);
                casTimer.Start();
                //MainPage.mainPage.StethoscopeStartStop.Invoke("startST", 1);
                //SetBtnColor(selectedIndex, 1);
            }
            else
            {
                MainPage.mainPage.isSTDeployed = false;
                deployRetractoprtstionStarted = true;
                RetractST(currentStethescope);
                MainPage.mainPage.StethoscopeStartStop.Invoke("stopST", 1);
                casTimer.Start();
                // SetBtnColor(currentStethescope, 2);
            }

        }

        async void UpdateNotification(string s, int code)
        {
            if (MainPage.mainPage.IsStethescopeChest)
                return;

            if (s.ToLower().Contains(("ready for streaming at").ToLower()))
            {
                MainPage.mainPage.isStethoscopeReadystreaming = true;
            }
            else if (s.ToLower().Contains("streaming at") || s.ToLower().Contains("receiving stream"))
            {
                MainPage.mainPage.isStethoscopeStreaming = true;

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    SetBtnColor(currentStethescope, 1);
                });
            }
            else if (s.ToLower().Contains("stopped receiving") || s.ToLower().Contains("streaming stopped"))
            {
                if (MainPage.mainPage.isStethoscopeStreaming)
                    MainPage.mainPage.StethoscopeStatus?.Invoke(true);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (MainPage.mainPage.isStethoscopeStreaming)
                        SetBtnColor(currentStethescope, 2);
                });

                MainPage.mainPage.isStethoscopeStreaming = false;
            }
            else if (s.ToLower().Contains(("Cannot connect to TX failed to connect").ToLower()) ||
                s.ToLower().Contains(("Failed").ToLower()))
            {
                MainPage.mainPage.isStethoscopeStreaming = false;
                MainPage.mainPage.isStethoscopeReadystreaming = false;
                MainPage.mainPage.StethoscopeStatus?.Invoke(false);

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    SetBtnColor(currentStethescope, 3);
                });
            }
            else if (s.ToLower().Contains(("Failed").ToLower()))
            {
                MainPage.mainPage.isStethoscopeStreaming = false;
                MainPage.mainPage.isStethoscopeReadystreaming = false;
                MainPage.mainPage.StethoscopeStatus?.Invoke(false);
            }

            // stmessage = s;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                StStatus.Text = s;
            });
        }

        public double StlHeight
        {
            get
            {
                double ht = gridstarray.RowDefinitions[1].ActualHeight - 10;

                return ht;
            }
        }

        void SelectedStethoscope(int index, string msg = "selected")
        {
            StStatus.Text = string.Format("Number {0 } {1}", index + 1, msg);
        }

        private void BtnST1_Click(object sender, RoutedEventArgs e)
        {
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(0, 0);
        }

        private void BtnST2_Click(object sender, RoutedEventArgs e)
        {
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(1, 0);
        }

        private void BtnST3_Click(object sender, RoutedEventArgs e)
        {
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(2, 0);
        }

        private void BtnST4_Click(object sender, RoutedEventArgs e)
        {
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(3, 0);
        }

        private void BtnST5_Click(object sender, RoutedEventArgs e)
        {
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(4, 0);
        }

        private void BtnST9_Click(object sender, RoutedEventArgs e)
        {
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(8, 0);
        }

        private void BtnST13_Click(object sender, RoutedEventArgs e)
        {
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(12, 0);
        }

        private void BtnST14_Click(object sender, RoutedEventArgs e)
        {
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(13, 0);
        }

        private void BtnST10_Click(object sender, RoutedEventArgs e)
        {
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(9, 0);
        }

        private void BtnST6_Click(object sender, RoutedEventArgs e)
        {
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(5, 0);
        }

        private void BtnST8_Click(object sender, RoutedEventArgs e)
        {
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(7, 0);
        }
        private void BtnST15_Click(object sender, RoutedEventArgs e)
        {
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(14, 0);
        }

        private void BtnST11_Click(object sender, RoutedEventArgs e)
        {
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(10, 0);
        }

        private void BtnST12_Click(object sender, RoutedEventArgs e)
        {
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(11, 0);
        }
        private void BtnST7_Click(object sender, RoutedEventArgs e)
        {
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(6, 0);
        }

        private void BtnST16_Click(object sender, RoutedEventArgs e)
        {
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(15, 0);
        }




        private void Btnup_Click(object sender, RoutedEventArgs e)
        {
        }


        private void Btndown_Click(object sender, RoutedEventArgs e)
        {
        }

        async void Clearall()
        {
            isoperationStarted = false;
            selectedIndex = -1;
            currentStethescope = -1;
            STState.Clear();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                BtnST1.Background = new SolidColorBrush(Windows.UI.Colors.White);
                BtnST2.Background = new SolidColorBrush(Windows.UI.Colors.White);
                BtnST3.Background = new SolidColorBrush(Windows.UI.Colors.White);
                BtnST4.Background = new SolidColorBrush(Windows.UI.Colors.White);
                BtnST5.Background = new SolidColorBrush(Windows.UI.Colors.White);
                BtnST6.Background = new SolidColorBrush(Windows.UI.Colors.White);
                BtnST7.Background = new SolidColorBrush(Windows.UI.Colors.White);
                BtnST8.Background = new SolidColorBrush(Windows.UI.Colors.White);
                BtnST9.Background = new SolidColorBrush(Windows.UI.Colors.White);
                BtnST10.Background = new SolidColorBrush(Windows.UI.Colors.White);
                BtnST11.Background = new SolidColorBrush(Windows.UI.Colors.White);
                BtnST12.Background = new SolidColorBrush(Windows.UI.Colors.White);
                BtnST13.Background = new SolidColorBrush(Windows.UI.Colors.White);
                BtnST14.Background = new SolidColorBrush(Windows.UI.Colors.White);
                BtnST15.Background = new SolidColorBrush(Windows.UI.Colors.White);
                BtnST16.Background = new SolidColorBrush(Windows.UI.Colors.White);
                // BtnDone.Content = isoperationStarted ? "Stop" : "Start";
                BtnStart.Content = isoperationStarted ? "Stop streaming" : "Start streaming";
                StStatus.Text = "";


            });
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


        bool isoperationStarted = false;
        int selectedIndex = -1;
        int currentStethescope = 0;
        Dictionary<int, int> STState = new Dictionary<int, int>();
        void SetBtnColor(int idx, int state)
        {
            SelectedStethoscope(idx);
            switch (idx)
            {
                case 0:
                    if (state == 0)
                    {//selected
                     // BtnST1.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
                        BtnST1.Background = GetColorFromHexa("#EEEEEE");
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);

                    }
                    else if (state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST1.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                            STState[idx] = 1;
                    }
                    else if (state == 2)
                    {//completed
                     //  currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST1.Background = GetColorFromHexa("#34CBA8"); //new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);


                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST1.Background = GetColorFromHexa("#E96056"); //new SolidColorBrush(Windows.UI.Colors.Red);
                                                                         //  currentStethescope = -2;
                                                                         //error
                    }
                    else if (state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST1.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if (st == 1)
                                BtnST1.Background = GetColorFromHexa("#FFBF00");
                            else if (st == 2)
                                BtnST1.Background = GetColorFromHexa("#34CBA8"); //new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST1.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                //BtnST1.Background = new SolidColorBrush(Windows.UI.Colors.White);
                                BtnST1.Background = GetColorFromHexa("#FFBF00");
                        }
                    }
                    break;
                case 1:
                    if (state == 0)
                    {//selected
                        BtnST2.Background = GetColorFromHexa("#EEEEEE");//new SolidColorBrush(Windows.UI.Colors.LightGray);
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);

                    }
                    else if (state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST2.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                            STState[idx] = 1;
                    }
                    else if (state == 2)
                    {//completed
                     // currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST2.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST2.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                                                                        //  currentStethescope = -2;
                                                                        //error
                    }
                    else if (state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST2.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if (st == 1)
                                BtnST2.Background = GetColorFromHexa("#FFBF00");
                            else if (st == 2)
                                BtnST2.Background = GetColorFromHexa("#34CBA8"); //new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST2.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                //BtnST2.Background = new SolidColorBrush(Windows.UI.Colors.White);
                                BtnST2.Background = GetColorFromHexa("#FFBF00");
                        }
                    }
                    break;
                case 2:
                    if (state == 0)
                    {//selected
                        BtnST3.Background = GetColorFromHexa("#EEEEEE");//new SolidColorBrush(Windows.UI.Colors.LightGray);
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);

                    }
                    else if (state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST3.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                            STState[idx] = 1;
                    }
                    else if (state == 2)
                    {//completed
                        //currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST3.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST3.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                                                                        // currentStethescope = -2;
                                                                        //error
                    }
                    else if (state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST3.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if (st == 1)
                                BtnST3.Background = GetColorFromHexa("#FFBF00");
                            else if (st == 2)
                                BtnST3.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST3.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                // BtnST3.Background = new SolidColorBrush(Windows.UI.Colors.White);
                                BtnST3.Background = GetColorFromHexa("#FFBF00");
                        }
                    }
                    break;
                case 3:
                    if (state == 0)
                    {//selected
                        BtnST4.Background = GetColorFromHexa("#EEEEEE");//new SolidColorBrush(Windows.UI.Colors.LightGray);
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);

                    }
                    else if (state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST4.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                            STState[idx] = 1;
                    }
                    else if (state == 2)
                    {//completed
                     // currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST4.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST4.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                        //currentStethescope = -2;
                        //error
                    }
                    else if (state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST4.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if (st == 1)
                                BtnST4.Background = GetColorFromHexa("#FFBF00");
                            else if (st == 2)
                                BtnST4.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST4.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                // BtnST4.Background = new SolidColorBrush(Windows.UI.Colors.White);
                                BtnST4.Background = GetColorFromHexa("#FFBF00");
                        }
                    }
                    break;
                case 4:
                    if (state == 0)
                    {//selected
                        BtnST5.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);

                    }
                    else if (state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST5.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                            STState[idx] = 1;
                    }
                    else if (state == 2)
                    {//completed
                     // currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST5.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST5.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                                                                        // currentStethescope = -2;
                                                                        //error
                    }
                    else if (state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST5.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if (st == 1)
                                BtnST5.Background = GetColorFromHexa("#FFBF00");
                            else if (st == 2)
                                BtnST5.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST5.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                // BtnST5.Background = new SolidColorBrush(Windows.UI.Colors.White);
                                BtnST5.Background = GetColorFromHexa("#FFBF00");
                        }
                    }
                    break;
                case 5:
                    if (state == 0)
                    {//selected
                        BtnST6.Background = GetColorFromHexa("#EEEEEE");//new SolidColorBrush(Windows.UI.Colors.LightGray);
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);

                    }
                    else if (state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST6.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                            STState[idx] = 1;
                    }
                    else if (state == 2)
                    {//completed
                     // currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST6.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST6.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                                                                        // currentStethescope = -2;
                                                                        //error
                    }
                    else if (state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST6.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if (st == 1)
                                BtnST6.Background = GetColorFromHexa("#FFBF00");
                            else if (st == 2)
                                BtnST6.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST6.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                //BtnST6.Background = new SolidColorBrush(Windows.UI.Colors.White);
                                BtnST6.Background = GetColorFromHexa("#FFBF00");
                        }
                    }
                    break;
                case 6:
                    if (state == 0)
                    {//selected
                        BtnST7.Background = GetColorFromHexa("#EEEEEE");//new SolidColorBrush(Windows.UI.Colors.LightGray);
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);

                    }
                    else if (state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST7.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                            STState[idx] = 1;
                    }
                    else if (state == 2)
                    {//completed
                     // currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST7.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST7.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                                                                        // currentStethescope = -2;
                                                                        //error
                    }
                    else if (state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST7.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if (st == 1)
                                BtnST7.Background = GetColorFromHexa("#FFBF00");
                            else if (st == 2)
                                BtnST7.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST7.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                //BtnST7.Background = new SolidColorBrush(Windows.UI.Colors.White);
                                BtnST7.Background = GetColorFromHexa("#FFBF00");
                        }
                    }
                    break;
                case 7:
                    if (state == 0)
                    {//selected
                        BtnST8.Background = GetColorFromHexa("#EEEEEE");//new SolidColorBrush(Windows.UI.Colors.LightGray);
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);

                    }
                    else if (state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST8.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                            STState[idx] = 1;
                    }
                    else if (state == 2)
                    {//completed
                        //currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST8.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST8.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                        //currentStethescope = -2;
                        //error
                    }
                    else if (state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST8.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if (st == 1)
                                BtnST8.Background = GetColorFromHexa("#FFBF00");
                            else if (st == 2)
                                BtnST8.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST8.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                //BtnST8.Background = new SolidColorBrush(Windows.UI.Colors.White);
                                BtnST8.Background = GetColorFromHexa("#FFBF00");
                        }
                    }
                    break;
                case 8:
                    if (state == 0)
                    {//selected
                        BtnST9.Background = GetColorFromHexa("#EEEEEE");//new SolidColorBrush(Windows.UI.Colors.LightGray);
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);

                    }
                    else if (state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST9.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                            STState[idx] = 1;
                    }
                    else if (state == 2)
                    {//completed
                     //   currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST9.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST9.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                                                                        //  currentStethescope = -2;
                                                                        //error
                    }
                    else if (state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST9.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if (st == 1)
                                BtnST9.Background = GetColorFromHexa("#FFBF00");
                            else if (st == 2)
                                BtnST9.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST9.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                //BtnST9.Background = new SolidColorBrush(Windows.UI.Colors.White);
                                BtnST9.Background = GetColorFromHexa("#FFBF00");
                        }
                    }
                    break;
                case 9:
                    if (state == 0)
                    {//selected
                        BtnST10.Background = GetColorFromHexa("#EEEEEE");//new SolidColorBrush(Windows.UI.Colors.LightGray);
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);

                    }
                    else if (state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST10.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                            STState[idx] = 1;
                    }
                    else if (state == 2)
                    {//completed
                     //  currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST10.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST10.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                                                                         //  currentStethescope = -2;
                                                                         //error
                    }
                    else if (state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST10.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if (st == 1)
                                BtnST10.Background = GetColorFromHexa("#FFBF00");
                            else if (st == 2)
                                BtnST10.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST10.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                //BtnST10.Background = new SolidColorBrush(Windows.UI.Colors.White);
                                BtnST10.Background = GetColorFromHexa("#FFBF00");
                        }
                    }
                    break;
                case 10:
                    if (state == 0)
                    {//selected
                        BtnST11.Background = GetColorFromHexa("#EEEEEE");//new SolidColorBrush(Windows.UI.Colors.LightGray);
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);

                    }
                    else if (state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST11.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                            STState[idx] = 1;
                    }
                    else if (state == 2)
                    {//completed
                     // currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST11.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST11.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                                                                         //  currentStethescope = -2;
                                                                         //error
                    }
                    else if (state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST11.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if (st == 1)
                                BtnST11.Background = GetColorFromHexa("#FFBF00");
                            else if (st == 2)
                                BtnST11.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST11.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                //BtnST11.Background = new SolidColorBrush(Windows.UI.Colors.White);
                                BtnST11.Background = GetColorFromHexa("#FFBF00");
                        }
                    }
                    break;
                case 11:
                    if (state == 0)
                    {//selected
                        BtnST12.Background = GetColorFromHexa("#EEEEEE");//new SolidColorBrush(Windows.UI.Colors.LightGray);
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);

                    }
                    else if (state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST12.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                            STState[idx] = 1;
                    }
                    else if (state == 2)
                    {//completed
                     // currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST12.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST12.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                                                                         // currentStethescope = -2;
                                                                         //error
                    }
                    else if (state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST12.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if (st == 1)
                                BtnST12.Background = GetColorFromHexa("#FFBF00");
                            else if (st == 2)
                                BtnST12.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST12.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                //BtnST12.Background = new SolidColorBrush(Windows.UI.Colors.White);
                                BtnST12.Background = GetColorFromHexa("#FFBF00");
                        }
                    }
                    break;
                case 12:
                    if (state == 0)
                    {//selected
                        BtnST13.Background = GetColorFromHexa("#EEEEEE");//new SolidColorBrush(Windows.UI.Colors.LightGray);
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);

                    }
                    else if (state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST13.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                            STState[idx] = 1;
                    }
                    else if (state == 2)
                    {//completed
                     // currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST13.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST13.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                                                                         // currentStethescope = -2;
                                                                         //error
                    }
                    else if (state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST13.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if (st == 1)
                                BtnST13.Background = GetColorFromHexa("#FFBF00");
                            else if (st == 2)
                                BtnST13.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST13.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                // BtnST13.Background = new SolidColorBrush(Windows.UI.Colors.White);
                                BtnST13.Background = GetColorFromHexa("#FFBF00");
                        }
                    }
                    break;
                case 13:
                    if (state == 0)
                    {//selected
                        BtnST14.Background = GetColorFromHexa("#EEEEEE");//new SolidColorBrush(Windows.UI.Colors.LightGray);
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);

                    }
                    else if (state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST14.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                            STState[idx] = 1;
                    }
                    else if (state == 2)
                    {//completed
                        //currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST14.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST14.Background = GetColorFromHexa("#E96056"); //new SolidColorBrush(Windows.UI.Colors.Red);
                        ///  currentStethescope = -2;
                        //error
                    }
                    else if (state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST14.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if (st == 1)
                                BtnST14.Background = GetColorFromHexa("#FFBF00");
                            else if (st == 2)
                                BtnST14.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST14.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                //BtnST14.Background = new SolidColorBrush(Windows.UI.Colors.White);
                                BtnST14.Background = GetColorFromHexa("#FFBF00");
                        }
                    }
                    break;
                case 14:
                    if (state == 0)
                    {//selected
                        BtnST15.Background = GetColorFromHexa("#EEEEEE");//new SolidColorBrush(Windows.UI.Colors.LightGray);
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);

                    }
                    else if (state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST15.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                            STState[idx] = 1;
                    }
                    else if (state == 2)
                    {//completed
                     //  currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST15.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST15.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                                                                         // currentStethescope = -2;
                                                                         //error
                    }
                    else if (state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST15.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if (st == 1)
                                BtnST15.Background = GetColorFromHexa("#FFBF00");
                            else if (st == 2)
                                BtnST15.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST15.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                //BtnST15.Background = new SolidColorBrush(Windows.UI.Colors.White);
                                BtnST15.Background = GetColorFromHexa("#FFBF00");
                        }
                    }
                    break;
                case 15:
                    if (state == 0)
                    {//selected
                        BtnST16.Background = GetColorFromHexa("#EEEEEE");//new SolidColorBrush(Windows.UI.Colors.LightGray);
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);

                    }
                    else if (state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST16.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                            STState[idx] = 1;
                    }
                    else if (state == 2)
                    {//completed
                     //    currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST16.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST16.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                                                                         //  currentStethescope = -2;
                                                                         //error
                    }
                    else if (state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST16.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if (st == 1)
                                BtnST16.Background = GetColorFromHexa("#FFBF00");
                            else if (st == 2)
                                BtnST16.Background = GetColorFromHexa("#34CBA8");//new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST16.Background = GetColorFromHexa("#E96056");//new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                //BtnST16.Background = new SolidColorBrush(Windows.UI.Colors.White);
                                BtnST16.Background = GetColorFromHexa("#FFBF00");
                        }
                    }
                    break;


            }
        }


        private void UprightChair_Click(object sender, RoutedEventArgs e)
        {
            if (deployRetractoprtstionStarted)
                return;
            MainPage.mainPage.SeatHeightAdjust?.Invoke(true);

        }

        private void LeaningChair_Click(object sender, RoutedEventArgs e)
        {
            if (deployRetractoprtstionStarted)
                return;
            MainPage.mainPage.SeatHeightAdjust?.Invoke(false);

        }
        private void BtnRecord_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}