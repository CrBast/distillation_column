using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Windows
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort sp;
        private double _value;
        public MainWindow()
        {
            InitializeComponent();

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
                },
                new LineSeries
                {
                    Title = "Series 2",
                    Values = new ChartValues<double> { 6, 7, 3, 4 ,6 },
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "Series 3",
                    Values = new ChartValues<double> { 4,2,7,2,7 },
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 15
                }
            };

            Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
            YFormatter = value => value.ToString("C");

            //modifying the series collection will animate and update the chart
            SeriesCollection.Add(new LineSeries
            {
                Title = "Series 4",
                Values = new ChartValues<double> { 5, 3, 2, 4 },
                LineSmoothness = 0, //0: straight lines, 1: really smooth lines
                PointGeometry = Geometry.Parse("m 25 70.36218 20 -28 -20 22 -8 -6 z"),
                PointGeometrySize = 50,
                PointForeground = Brushes.Gray
            });

            //modifying any series values will also animate and update the chart
            SeriesCollection[3].Values.Add(5d);

            DataContext = this;


            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                lbxCom.Items.Add(port);
            }

            
        }

        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public void AddNewTemperature(double temp)
        {

        }

        public void AddLog(string text)
        {
            //text = text.Replace("\r", string.Empty);
            //text = text.Replace("\n", string.Empty);
            string last = Regex.Split(text, "\r\n").Last();
            Dispatcher.Invoke(() =>
            {
                tbxLogs.AppendText(text.ToString());
                tbxLogs.ScrollToEnd();                
            });
        }


        public void commanderOfCommand(string text)
        {
            text = Regex.Split(text, "\r\n").Last();

            text = text.Replace("\r", string.Empty);

            text = text.Replace("\n", string.Empty);

            if (text.StartsWith("-i"))
            {
                try
                {
                    Value = Convert.ToDouble(text.Replace("-i ", string.Empty));
                }
                catch { }
                return;
            }
            if (text.StartsWith("-s"))
            {
                string result = text.Replace("-s ", string.Empty);
                lbl_state.Content = result == "" || result == "-s" ? lbl_state.Content : result;
                return;
            } 
        }


        public static List<Request> COMStringSplitter(string s)
        {
            List<Request> array = new List<Request>();
            var requests = Regex.Split(s, "\r\n").ToList();
            foreach (var request in requests)
            {
                Request r = new Request();
                if (request.StartsWith("-i"))
                {
                    r.Type = Request.Type_Info;
                    r.Content = request.Replace("-i ", string.Empty);
                    array.Add(r);
                }
                else if (request.StartsWith("-h"))
                {
                    r.Type = Request.Type_HeatingPower;
                    r.Content = request.Replace("-h ", string.Empty);
                    array.Add(r);
                }
                else if (request.StartsWith("-s"))
                {
                    r.Type = Request.Type_State;
                    r.Content = request.Replace("-s ", string.Empty);
                    array.Add(r);
                }
            }
            return array;
        }

        public void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            commanderOfCommand(indata);
        }

        private void lbxCom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sp != null) {
                sp = null;
            }

            try
            {
                var lbx = (ListBox)sender;
                sp = new SerialPort(lbx.SelectedItem.ToString());
                sp.BaudRate = 9600;
                sp.Parity = Parity.None;
                sp.StopBits = StopBits.One;
                sp.DataBits = 8;
                sp.Handshake = Handshake.None;
                sp.RtsEnable = true;

                sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                sp.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
            sp.Open();
            } catch {
                sp = null;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(sp != null)
                sp.Close();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
