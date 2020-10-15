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
using LiveCharts;
using LiveCharts.Uwp;
using LiveCharts.Configurations;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using System.ComponentModel;
using System.Collections.ObjectModel;
using LineSeries = WinRTXamlToolkit.Controls.DataVisualization.Charting.LineSeries;
using Windows.UI;
using System.Threading.Tasks;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VideoKallMCCST.SpiroMeter
{
    public class ChartData : INotifyPropertyChanged
    {
        int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));

            }
        }

        public string Name { get; set; }
        string _receivedData = string.Empty;
        public string ReceivedData
        {
            get { return _receivedData; }
            set
            {
                _receivedData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReceivedData)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    public class DateModel
    {
        public System.DateTime DateTime { get; set; }
        public double Value { get; set; }
    }
    public sealed partial class SPMChart : UserControl
    {
        ObservableCollection<ChartData> EKGdata = new ObservableCollection<ChartData>();
        ObservableCollection<ChartData> items2 = new ObservableCollection<ChartData>();
        ObservableCollection<ChartData> items3 = new ObservableCollection<ChartData>();
        private Random _random = new Random();
        int maxdata = 20;
        int maxValue = 250;
        int interval = 10;
        int minVal = 0;
        public SPMChart()
        {
            this.InitializeComponent();
        }

        public void InitializeChartDisplay()
        {


            LinearAxis ay = new LinearAxis
            {
                Minimum = minVal,
                Maximum = maxValue,
                Orientation = WinRTXamlToolkit.Controls.DataVisualization.Charting.AxisOrientation.Y,
                Interval = interval,
                ShowGridLines = true,
                Width = 0,

            };

            CategoryAxis ax = new CategoryAxis
            {
                Orientation = WinRTXamlToolkit.Controls.DataVisualization.Charting.AxisOrientation.X,
                Height = 0,


            };

            LineChart.FlowDirection = FlowDirection.LeftToRight;

            EKGdata = new ObservableCollection<ChartData>();

            ((LineSeries)this.LineChart.Series[0]).ItemsSource = EKGdata;
            ((LineSeries)LineChart.Series[0]).DependentRangeAxis = ay;
            ((LineSeries)LineChart.Series[0]).IndependentAxis = ax;

            SolidColorBrush br = new SolidColorBrush
            {
                Color = Colors.WhiteSmoke
            };
            ay.Background = br;
            //   ((LineSeries)LineChart.Series[0]).Background = br;
            // LineChart.Height = LineChart.Height+400;
            //  LineChart.Width = LineChart.Width + 400;
            LineChart.Background = br;
            for (int i = 0; i < maxdata; i++)
            {
                EKGdata.Insert(indx, new ChartData { Name = i.ToString(), Value = -1 });

            }

            EKGdata.Clear();
        }
        int indx = 0;
        bool removeItem = false;
        public void ChartDisplay(object s, EventArgs e)
        {
            try
            {
                string data="0";// = //((ChartDataeventMsg)e).Msg;
                if (data.Equals("Initialize"))
                {
                    InitializeChartDisplay();
                    return;
                }
                else if (data.Equals("clear"))
                {
                    EKGdata.Clear();
                    indx = 0;
                    removeItem = false;
                    return;
                }

                ChartData chartdata = new ChartData { Name = indx.ToString(), Value = Convert.ToInt32(data) };
               this.DataContext = chartdata;
                chartdata.ReceivedData = data;
                if (indx >= maxdata)
                {
                    indx = 0;
                    removeItem = true;
                }
                if (removeItem && EKGdata.Count > 0)
                {
                    EKGdata.RemoveAt(indx);

                }

                EKGdata.Insert(indx, chartdata);
                indx++;

            }
            catch (Exception ex)
            {
                string exx = ex.Message;
            }

        }

        public async void ChartDisplayDemo(object s, EventArgs e)
        {
            SolidColorBrush br = new SolidColorBrush
            {
                Color = Colors.LightGray
            };
            EKGdata.Clear();
            int k = 0;
            bool rem = false;
            for (int i = 0; i < 1000; i++)
            {
                int a = _random.Next(100);

                if (k >= 20)
                {
                    k = 0;

                    rem = true;
                }

                if (rem)
                    EKGdata.RemoveAt(k);

                EKGdata.Insert(k, new ChartData { Name = k.ToString(), Value = a });
                await Task.Delay(TimeSpan.FromSeconds(1));
                k++;
            }

        }
    }
}
