﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        DispatcherTimer  StethoscopeTimerobj = null;
        
        public StethoscopeLungs()
        {
            this.InitializeComponent();
            this.DataContext = this;
            MainPage.mainPage.ResetSTLungs += Clearall ; 
            MainPage.mainPage.StethoscopeNotification += UpdateNotification;
        }

        private void BtnDone_Click(object sender, RoutedEventArgs e)
        {
            if (isAutomode || (selectedIndex ==-1 && !isoperationStarted))
                
                return;

            StartStopST();
        }

        void DeployST()
        {

        }
        void StartStopST()
        {
            isoperationStarted = !isoperationStarted;

            BtnDone.Content = isoperationStarted ? "Stop" : "Start";

            if (isoperationStarted)
            {
                MainPage.mainPage.StethoscopeStartStop.Invoke("startST", 1);
                SetBtnColor(selectedIndex, 1);
                if(!isAutomode)
                stinprogressIndex = currentStethescope;
            } 
            else
            {
                MainPage.mainPage.StethoscopeStartStop.Invoke("stopST", 1);
               // SetBtnColor(currentStethescope, 2);
            }
                
        }

        async void UpdateNotification(string s, int code)
        {
            if ( MainPage.mainPage.IsStethescopeChest)
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
                    if (MainPage.mainPage.isStethoscopeStreaming  )
                    SetBtnColor(stinprogressIndex, 2);
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

        bool isAutomode = false;
        private void BtnAuto_Click(object sender, RoutedEventArgs e)
        {
            if (btnAuto.IsChecked == true)
            {
                isAutomode = true;
                Clearall();
            
                if (StethoscopeTimerobj == null)
                {
                    StethoscopeTimerobj = new DispatcherTimer();
                    StethoscopeTimerobj.Tick += StethoscopeTimerobj_Tick;
                    
                }
                if (!StethoscopeTimerobj.IsEnabled)
                {
                   
                    StethoscopeTimerobj.Interval = new TimeSpan(0, 0, 2);
                    StethoscopeTimerobj.Start();
                }
               
            }
            else
            {
                StethoscopeTimerobj.Stop();
                isAutomode = false;

                if (isoperationStarted)
                    StartStopST();
            }

        }

        int stinprogressIndex = 0;
        private void StethoscopeTimerobj_Tick(object sender, object e)
        {
            StethoscopeTimerobj.Stop(); 
            
            StethoscopeTimerobj.Interval = new TimeSpan(0, 0, MainPage.mainPage.STAutomodeTime);
            //   if (MainPage.mainPage.isStethoscopeStreaming)
            //       SetBtnColor(currentStethescope, 2);
            stinprogressIndex = currentStethescope;
            if (isoperationStarted)
            StartStopST();

            
            if (travarseSTIndex > 15)
            {
                StethoscopeTimerobj.Stop();
                travarseSTIndex = 0;
                btnAuto.IsChecked = false;
                isAutomode = false;
                return;
            }
            travarseSTIndex++;
            if(travarseSTIndex<16)
            {
                SetBtnColor(selectedIndex, -1);
                SetBtnColor(travarseSTIndex, 0);

                StartStopST();
            }
            StethoscopeTimerobj.Start();
        }

        public   double StlHeight
        {
             
            get
            {
                double ht = gridstarray.RowDefinitions[1].ActualHeight-10;

                return ht;
            }
        }

        void SelectedStethoscope(int index,string msg = "selected")
        {
            StStatus.Text = string.Format( "Stethoscope number {0 } {1}", index + 1, msg);
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
            if (isAutomode)
                return;

            if (travarseSTIndex <= 0)
                travarseSTIndex = 0;
            else
                travarseSTIndex--;
            SetBtnColor(selectedIndex, -1);
            SetBtnColor(travarseSTIndex, 0);

        }


        private void Btndown_Click(object sender, RoutedEventArgs e)
        {
            if (isAutomode)
                return;
            if (travarseSTIndex >= 15)
                travarseSTIndex = 15;
            else
                travarseSTIndex++;

            SetBtnColor(selectedIndex, -1);
            SetBtnColor(travarseSTIndex, 0);
        }

      async  void Clearall()
        {
           
              travarseSTIndex = -1;
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
                BtnDone.Content = isoperationStarted ? "Stop" : "Start";
                if(isAutomode)
                    StStatus.Text = "Streaming will start within 5 sec.";
                else
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

        int travarseSTIndex = -1;
        bool isoperationStarted = false;
        int selectedIndex = -1;
        int currentStethescope = 0;
        Dictionary<int, int> STState = new Dictionary<int, int>();
        void SetBtnColor(int idx,int state)
        {
            SelectedStethoscope(idx);
            switch (idx)
            {
               case 0:
                    if(state == 0)
                    {//selected
                        BtnST1.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
                        selectedIndex = idx;
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 0);
                        
                    }
                    else if(state == 1)
                    {//inprogress
                        currentStethescope = idx;
                        BtnST1.Background = GetColorFromHexa("#FFBF00");
                        if (!STState.ContainsKey(idx))
                            STState.Add(idx, 1);
                        else
                        STState[idx] = 1;
                    }
                    else if(state == 2)
                    {//completed
                      //  currentStethescope = -2;
                        STState[idx] = 2;
                        BtnST1.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if(state == 3)
                    {
                        STState[idx] = 3;
                        BtnST1.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                      //  currentStethescope = -2;
                        //error
                    }
                    else if(state == -1)
                    {
                        if (!STState.ContainsKey(idx))
                            BtnST1.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        else
                        {
                            int st = STState[idx];
                            if(st == 1)
                                BtnST1.Background = GetColorFromHexa("#FFBF00");
                            else if(st == 2)
                                BtnST1.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if(st == 3)
                                BtnST1.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            else if(st == 0)
                                BtnST1.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        }       
                    }
                    break;
                case 1:
                    if (state == 0)
                    {//selected
                        BtnST2.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
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
                        BtnST2.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST2.Background = new SolidColorBrush(Windows.UI.Colors.Red);
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
                                BtnST2.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST2.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                BtnST2.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        }
                    }
                    break;
                case 2:
                    if (state == 0)
                    {//selected
                        BtnST3.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
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
                        BtnST3.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST3.Background = new SolidColorBrush(Windows.UI.Colors.Red);
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
                                BtnST3.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST3.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                BtnST3.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        }
                    }
                    break;
                case 3:
                    if (state == 0)
                    {//selected
                        BtnST4.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
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
                        BtnST4.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST4.Background = new SolidColorBrush(Windows.UI.Colors.Red);
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
                                BtnST4.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST4.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                BtnST4.Background = new SolidColorBrush(Windows.UI.Colors.White);
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
                        BtnST5.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST5.Background = new SolidColorBrush(Windows.UI.Colors.Red);
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
                                BtnST5.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST5.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                BtnST5.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        }
                    }
                    break;
                case 5:
                    if (state == 0)
                    {//selected
                        BtnST6.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
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
                        BtnST6.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST6.Background = new SolidColorBrush(Windows.UI.Colors.Red);
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
                                BtnST6.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST6.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                BtnST6.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        }
                    }
                    break;
                case 6:
                    if (state == 0)
                    {//selected
                        BtnST7.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
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
                        BtnST7.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST7.Background = new SolidColorBrush(Windows.UI.Colors.Red);
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
                                BtnST7.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST7.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                BtnST7.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        }
                    }
                    break;
                case 7:
                    if (state == 0)
                    {//selected
                        BtnST8.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
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
                        BtnST8.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST8.Background = new SolidColorBrush(Windows.UI.Colors.Red);
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
                                BtnST8.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST8.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                BtnST8.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        }
                    }
                    break;
                case 8:
                    if (state == 0)
                    {//selected
                        BtnST9.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
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
                        BtnST9.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST9.Background = new SolidColorBrush(Windows.UI.Colors.Red);
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
                                BtnST9.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST9.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                BtnST9.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        }
                    }
                    break;
                case 9:
                    if (state == 0)
                    {//selected
                        BtnST10.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
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
                        BtnST10.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST10.Background = new SolidColorBrush(Windows.UI.Colors.Red);
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
                                BtnST10.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST10.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                BtnST10.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        }
                    }
                    break;
                case 10:
                    if (state == 0)
                    {//selected
                        BtnST11.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
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
                        BtnST11.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST11.Background = new SolidColorBrush(Windows.UI.Colors.Red);
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
                                BtnST11.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST11.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                BtnST11.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        }
                    }
                    break;
                case 11:
                    if (state == 0)
                    {//selected
                        BtnST12.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
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
                        BtnST12.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST12.Background = new SolidColorBrush(Windows.UI.Colors.Red);
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
                                BtnST12.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST12.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                BtnST12.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        }
                    }
                    break;
                case 12:
                    if (state == 0)
                    {//selected
                        BtnST13.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
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
                        BtnST13.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST13.Background = new SolidColorBrush(Windows.UI.Colors.Red);
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
                                BtnST13.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST13.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                BtnST13.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        }
                    }
                    break;
                case 13:
                    if (state == 0)
                    {//selected
                        BtnST14.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
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
                        BtnST14.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST14.Background = new SolidColorBrush(Windows.UI.Colors.Red);
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
                                BtnST14.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST14.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                BtnST14.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        }
                    }
                    break;
                case 14:
                    if (state == 0)
                    {//selected
                        BtnST15.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
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
                        BtnST15.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST15.Background = new SolidColorBrush(Windows.UI.Colors.Red);
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
                                BtnST15.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST15.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                BtnST15.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        }
                    }
                    break;
                case 15:
                    if (state == 0)
                    {//selected
                        BtnST16.Background = new SolidColorBrush(Windows.UI.Colors.LightGray);
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
                        BtnST16.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                    }
                    else if (state == 3)
                    {
                        STState[idx] = 3;
                        BtnST16.Background = new SolidColorBrush(Windows.UI.Colors.Red);
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
                                BtnST16.Background = new SolidColorBrush(Windows.UI.Colors.LightSeaGreen);
                            else if (st == 3)
                                BtnST16.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                            else if (st == 0)
                                BtnST16.Background = new SolidColorBrush(Windows.UI.Colors.White);
                        }
                    }
                    break;
                 

            }
        }

      
    }
}
