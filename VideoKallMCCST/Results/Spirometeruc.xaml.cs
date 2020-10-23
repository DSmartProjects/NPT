using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class FlowVolumeData
    {
     public string Flow { get; set; }
        public string Volume { get; set; }
        public string Volumeflow { get; set; }
        public string Time { get; set; }
    }


    public class VCResults
    {
        public string Btps { get; set; }
        public string CreationDate { get; set; }
        public string TempCelsius { get; set; }
        public string TempFahrenheit { get; set; }
        public string Code { get; set; }
        public string MeasureUnit { get; set; }
        public string MeasuredValue { get; set; }
        public string Name { get; set; }
        public string ParameterType { get; set; }
    }//     string header = string.Format("{0}!{1}!{2}!{3}!", Btps, CreationDate, TempCelsius, TempFahrenheit);
     //   strparameters += string.Format("{0}!{1}!{2}!{3}!@", Code, MeasureUnit, MeasuredValue, Name, ParameterType);


    public sealed partial class Spirometeruc : UserControl
    {
        string fvText = "Flow Volume";
        string vcText = "Volume Time";
        bool isStarted = false;
        ObservableCollection<FlowVolumeData> fvcFlowVolumeCollection = new ObservableCollection<FlowVolumeData>();
        ObservableCollection<FlowVolumeData> fvctFlowVolumeCollection = new ObservableCollection<FlowVolumeData>();
        ObservableCollection<FlowVolumeData>  vcFlowVolumeCollection = new ObservableCollection<FlowVolumeData>();
        ObservableCollection<VCResults> VCResultsColl = new ObservableCollection<VCResults>();
        ObservableCollection<VCResults> FVCResultsColl = new ObservableCollection<VCResults>();
        void dummydata()
         {
        //    FlowVolumeData data = new FlowVolumeData();
        //    data.Flow = "0.01";
        //    data.Volume = "0.22";
        //    fvcFlowVolumeCollection.Add(data);
        //    for(int i = 0; i<10 ; i++)
        //    { 
        //    data = new FlowVolumeData();
        //    data.Flow = "0.11";
        //    data.Volume = "0.12";
        //        data.Time = "0.323255";
        //    fvcFlowVolumeCollection.Add(data);
                
        //    }
        }
        public Spirometeruc()
        {
            this.InitializeComponent();
            MainPage.mainPage.SPirotResults += SpiroResults;
            MainPage.mainPage.ResetSpirometr += Reset;
           
            fvcFlowVolumeCollection.Clear();
            fvctFlowVolumeCollection.Clear();
            vcFlowVolumeCollection.Clear();
            FVCResultsColl.Clear();
            VCResultsColl.Clear();
            Fvcvolflowgrid.ItemsSource = fvcFlowVolumeCollection;
            Fvcvoltime.ItemsSource = fvctFlowVolumeCollection;
            vcvoltime.ItemsSource = vcFlowVolumeCollection;
            Fvcresults.ItemsSource = FVCResultsColl;
            vcresults.ItemsSource = VCResultsColl;
            StopFVC.IsEnabled = false;
            StopVC.IsEnabled = false;
        }

      async  void SpiroResults(string msg)
        {
            string[] cmd = msg.ToLower().Split('>');
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                switch (cmd[0])
                {
                    case "spirofvc":
                        isStarted = true;
                        TxtStatusspior.Text = "Blowing";
                        string[] fvc = cmd[1].Split(',');
                        FlowVolumeData data = new FlowVolumeData();
                        data.Flow = fvc[0];
                        data.Volume = fvc[1];
                        fvcFlowVolumeCollection.Add(data);
                        fvcfv.Text = string.Format("{0}({1})", fvText, fvcFlowVolumeCollection.Count());
                        break;
                    case "spirovc":
                        {
                          
                            isStarted = true;
                            TxtStatusspior.Text = "Blowing";
                            string[]  vct = cmd[1].Split(',');
                            FlowVolumeData datat1 = new FlowVolumeData();
                            datat1.Volumeflow =  vct[0];
                            datat1.Time =  vct[1];
                            vcFlowVolumeCollection.Add(datat1);
                            vctxt.Text = string.Format("{0}({1})", vcText, vcFlowVolumeCollection.Count());
                        }
                        break;
                    case "spirofvcvt":
                        {
                            isStarted = true;
                            TxtStatusspior.Text = "Blowing";
                            string[] fvct = cmd[1].Split(',');
                            FlowVolumeData datat1 = new FlowVolumeData();
                            datat1.Volumeflow = fvct[0];
                            datat1.Time = fvct[1];
                            fvctFlowVolumeCollection.Add(datat1);
                          //  fvcfv.Text = string.Format("{0}({1})", fvText, fvctFlowVolumeCollection.Count());
                        }

                        break;
                    case "spirostatussucess":
                        TxtStatusspior.Text = "Ready for Blow";
                        isStarted = true;
                        MainPage.mainPage.Spirometrystatus?.Invoke(true);
                        break;
                    case "spirostatusfailed":
                        isStarted = false;
                        TxtStatusspior.Text = "Error: " + cmd[1];
                        MainPage.mainPage.Spirometrystatus?.Invoke(false);
                        break;
                    case "stoppedspirometer":
                        isStarted = false;
                        TxtStatusspior.Text = "Stopped";
                        break;
                    case "spirofvcresult":
                        TxtStatusspior.Text = "FVC Test Results.";
                        {
                            VCResults FVCresult = new VCResults();

                            string[] FVCheader = msg.Split('>')[1].Split('!');

                            FVCresult.Btps = FVCheader.Length > 0 ? FVCheader[0] : "";
                            FVCresult.CreationDate = FVCheader.Length > 2 ? FVCheader[1] : "";
                            FVCresult.TempCelsius = FVCheader.Length > 2 ? FVCheader[2] : "";
                            FVCresult.TempFahrenheit = FVCheader.Length > 2 ? FVCheader[3] : "";

                            // VCResultsColl.Add(result);
                            string[] FVCParameteres = msg.Split('>')[2].Split('@');
                            foreach (var parm in FVCParameteres)
                            {
                                string[] paramsdata = parm.Split('!');
                                VCResults parmresults = new VCResults();
                                parmresults.Code = paramsdata.Length > 0 ? paramsdata[0] : "";
                                parmresults.MeasureUnit = paramsdata.Length > 2 ? paramsdata[1] : "";
                                parmresults.MeasuredValue = paramsdata.Length > 2 ? paramsdata[2] : "";
                                parmresults.Name = paramsdata.Length > 2 ? paramsdata[3] : "";
                                parmresults.ParameterType = paramsdata.Length > 2 ? paramsdata[3] : "";
                                FVCResultsColl.Add(parmresults);
                            }
                             

                            TextCreationTime1.Text = FVCresult.CreationDate;
                            TextBTPS1.Text = FVCresult.Btps;

                            tmpF1.Text = FVCresult.TempFahrenheit;
                            tmpc1.Text = FVCresult.TempCelsius;
                            pivotTab.SelectedIndex = 2;
                            isStarted = false;
                        }

                        break;
                    case "spirovcresult":
                        TxtStatusspior.Text = "VC Test Results."; 
                        VCResults result = new VCResults();

                        string[] header = msg.Split('>')[1].Split('!');

                        result.Btps = header.Length>0? header[0]:"";
                        result.CreationDate = header.Length > 2 ? header[1] : "";
                        result.TempCelsius = header.Length > 2 ? header[2] : "";
                        result.TempFahrenheit = header.Length > 2 ? header[3] : "";
                       // VCResultsColl.Add(result);
                        string[] Parameteres = msg.Split('>')[2].Split('@');
                        foreach(var parm in Parameteres)
                        {
                            string []paramsdata = parm.Split('!');
                            VCResults parmresults = new VCResults();
                            parmresults.Code = paramsdata.Length > 0 ? paramsdata[0] : "";
                            parmresults.MeasureUnit = paramsdata.Length > 2 ? paramsdata[1] : "";
                            parmresults.MeasuredValue = paramsdata.Length > 2 ? paramsdata[2] : "";
                            parmresults.Name = paramsdata.Length > 2 ? paramsdata[3] : "";
                            parmresults.ParameterType = paramsdata.Length > 2 ? paramsdata[3] : "";
                            VCResultsColl.Add(parmresults);
                        }
                        TextCreationTime.Text = result.CreationDate;
                        TextBTPS.Text = result.Btps;

                        TextCreationTime.Text = result.CreationDate;
                        TextBTPS.Text = result.Btps;

                        tmpF.Text = result.TempFahrenheit;
                        tmpc.Text = result.TempCelsius;
                        pivotTab.SelectedIndex = 4;
                        isStarted = false;
                        break;
                }
            });
              
            
        }

      async  void Reset()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                StartFVC.IsEnabled = true;
                StopFVC.IsEnabled = true;
                StartVC.IsEnabled = true;
                StopVC.IsEnabled = true;
                isStarted = false;
                TxtStatusspior.Text = "";
                fvcFlowVolumeCollection.Clear();
                fvctFlowVolumeCollection.Clear();
                vcFlowVolumeCollection.Clear();
                VCResultsColl.Clear();
                FVCResultsColl.Clear();
                vctxt.Text = "";
                StopFVC.IsEnabled = false;
                StopVC.IsEnabled = false;
            });
        }

        private void StartFVC_Click(object sender, RoutedEventArgs e)
        {
            fvcFlowVolumeCollection.Clear();
            fvctFlowVolumeCollection.Clear();
            StartFVC.IsEnabled = false;
            StartVC.IsEnabled = false;
            StopFVC.IsEnabled = true;
            vctxt.Text = "";
            //    vcFlowVolumeCollection.Clear();
            //  double top = MainPage.mainPage.ActualHeight - MainPage.mainPage.gridleftpanel.ActualHeight;
            //  MainPage.mainPage.gridleftpanel.ActualSize.Length.
            //   double w = MainPage.mainPage.ActualWidth - (MainPage.mainPage.RightPanelHolder.ActualWidth+350);
            //MainPage.mainPage.ActualWidth - (spgrid.ActualWidth-spfvcgrid.ActualWidth) ;

            double y = spgrid.ActualSize.Y;
            double x = spgrid.ActualSize.X;
            double h = spgrid.ActualHeight;
            double w = spgrid.ActualWidth - 350;
            
            MainPage.mainPage.CommToDataAcq.SendMessageToDataacquistionapp(string.Format(CommunicationCommands.StartSpiroFVC, x.ToString()+":"+ y.ToString()+":"+ h.ToString()+":"+w.ToString()));
        }

        private void StopFVC_Click(object sender, RoutedEventArgs e)
        {
            StartFVC.IsEnabled = true;
            StartVC.IsEnabled = true;
            isStarted = false;
            StopFVC.IsEnabled = false;


            MainPage.mainPage.CommToDataAcq.SendMessageToDataacquistionapp(CommunicationCommands.StopSpiro);
        }

        private void StartVC_Click(object sender, RoutedEventArgs e)
        {
            //  fvcFlowVolumeCollection.Clear();
            //  fvctFlowVolumeCollection.Clear();
            //double top = MainPage.mainPage.ActualHeight - MainPage.mainPage.gridleftpanel.ActualHeight;
            //double w = MainPage.mainPage.ActualWidth - (MainPage.mainPage.RightPanelHolder.ActualWidth + 350);

            StartFVC.IsEnabled = false;
            StartVC.IsEnabled = false;
            StopVC.IsEnabled = true;
            vctxt.Text = "";
            vcFlowVolumeCollection.Clear();
            VCResultsColl.Clear();

            double y = spgrid.ActualSize.Y;
            double x = spgrid.ActualSize.X;
            double h = spgrid.ActualHeight;
            double w = spgrid.ActualWidth - 350;
            MainPage.mainPage.CommToDataAcq.SendMessageToDataacquistionapp(string.Format(CommunicationCommands.StartSpiroVC, x.ToString() + ":" + y.ToString() + ":" + h.ToString() + ":" + w.ToString()));

        }

        private void StopVC_Click(object sender, RoutedEventArgs e)
        {
            StartFVC.IsEnabled = true;
            StartVC.IsEnabled = true;
            StopVC.IsEnabled = false;
            isStarted = false;
            MainPage.mainPage.CommToDataAcq.SendMessageToDataacquistionapp(CommunicationCommands.StopSpiro);
        }

        private void BtnDone_Click(object sender, RoutedEventArgs e)
        {
            if(!isStarted)
            MainPage.mainPage.Spirometercallback?.Invoke();
        }
    }
}
