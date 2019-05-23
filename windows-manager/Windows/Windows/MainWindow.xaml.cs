using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using Windows;
using LiveCharts;

namespace Arduino_Viewer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static int Counter = 0;
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            Gauge.Value = 25;

            /*Points.Add(new DataPoint(1, 2.5));
            Points.Add(new DataPoint(2, 2.3));*/
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public void AddNewTemperature(double temp)
        {
            Counter++;
            //ToDo
        }

        public void AddLog(string text)
        {
            Dispatcher.Invoke(() =>
            {
                TbxLogs.AppendText(text);
                TbxLogs.ScrollToEnd();                
            });
        }


        public void CommanderOfCommand(string text)
        {
            AddLog(text);
            var requests = ComStringSplitter(text);


            foreach(Request request in requests)
            {
                switch (request.Type)
                {
                    case Request.TypeState:
                        Dispatcher.Invoke(() =>
                        {
                            LblState.Content = request.Content;
                        });
                        break;
                    case Request.TypeInfo:
                        Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                var temperature = Convert.ToDouble(request.Content.Replace(".", ","));
                                AddNewTemperature(temperature);
                                Gauge.Value = temperature;
                            }
                            catch(Exception e) { Console.Write(e); }
                        });
                        break;
                }
                    
            }
        }


        public  List<Request> ComStringSplitter(string s)
        {
            List<Request> array = new List<Request>();

            var requests = ArduinoConnection.GetRequests(s);

            foreach (var request in requests)
            {

                Request r = new Request {FullRequest = request};
                if (request.StartsWith(Request.TypeInfo))
                {
                    r.Type = Request.TypeInfo;
                    r.Content = request.Replace(Request.TypeInfo, string.Empty);
                }
                else if (request.StartsWith(Request.TypeHeatingPower))
                {
                    r.Type = Request.TypeHeatingPower;
                    r.Content = request.Replace(Request.TypeHeatingPower, string.Empty);
                }
                else if (request.StartsWith(Request.TypeState))
                {
                    r.Type = Request.TypeState;
                    r.Content = request.Replace(Request.TypeState, string.Empty);
                }
                array.Add(r);
            }
            return array;
        }

        public void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            CommanderOfCommand(sp.ReadExisting());
        }

        private void lbxCom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lbx = (ListBox)sender;
            if(lbx.SelectedItem != null)
            {
                BtnCloseCom.IsEnabled = true;
                ArduinoConnection.SetCom(lbx.SelectedItem.ToString());

                ArduinoConnection.GetInstance().DataReceived += DataReceivedHandler;
                ArduinoConnection.Go();
                AddLog($"Connection to {ArduinoConnection.GetInstance().PortName} is open\r\n");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ArduinoConnection.GetInstance().DataReceived -= DataReceivedHandler;
            ArduinoConnection.Close();
            TbxLogs.Clear();
        }

        private void btn_closeCOM_Click(object sender, RoutedEventArgs e)
        {
            ArduinoConnection.GetInstance().DataReceived -= DataReceivedHandler;
            ArduinoConnection.Close();
            LbxCom.UnselectAll();
            AddLog($"Connection to {ArduinoConnection.GetInstance().PortName} is closed\r\n");
            BtnCloseCom.IsEnabled = false;
        }

        private void BtnRefreshCom_Click(object sender, RoutedEventArgs e)
        {
            LbxCom.Items.Clear();
            foreach (string port in SerialPort.GetPortNames())
            {
                LbxCom.Items.Add(port);
            }
        }
    }
}
