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
        private string incomplete_request = null;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            gauge.Value = 25;

            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                lbxCom.Items.Add(port);
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
            Dispatcher.Invoke(() =>
            {
                tbxLogs.AppendText(text.ToString());
                tbxLogs.ScrollToEnd();                
            });
        }


        public void commanderOfCommand(string text)
        {
            AddLog(text);
            var requests = COMStringSplitter(text);


            foreach(Request request in requests)
            {
                switch (request.Type)
                {
                    case Request.Type_State:
                        Dispatcher.Invoke(() =>
                        {
                            lbl_state.Content = request.Content;
                        });
                        break;
                    case Request.Type_Info:
                        Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                gauge.Value = Convert.ToDouble(request.Content.Replace(".", ","));
                            }
                            catch(Exception e) { Console.Write(e); }
                        });
                        break;
                }
                    
            }
        }


        public  List<Request> COMStringSplitter(string s)
        {
            List<Request> array = new List<Request>();

            if(incomplete_request != null)
            {
                s = incomplete_request + s;
            }

            var requests = Regex.Split(s, "\r\n").ToList();
            foreach (var request in requests)
            {
                Request r = new Request();
                r.FullRequest = request;
                if (request.StartsWith(Request.Type_Info))
                {
                    r.Type = Request.Type_Info;
                    r.Content = request.Replace(Request.Type_Info.ToString(), string.Empty);
                }
                else if (request.StartsWith(Request.Type_HeatingPower))
                {
                    r.Type = Request.Type_HeatingPower;
                    r.Content = request.Replace(Request.Type_HeatingPower.ToString(), string.Empty);
                }
                else if (request.StartsWith(Request.Type_State))
                {
                    r.Type = Request.Type_State;
                    r.Content = request.Replace(Request.Type_State.ToString(), string.Empty);
                }
                array.Add(r);
            }
            if (!s.EndsWith("\r\n"))
            {
                var lastCommand = array[array.Count - 1];
                incomplete_request = lastCommand.FullRequest;
                array.Remove(lastCommand);
            }
            else
            {
                incomplete_request = null;
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
            var lbx = (ListBox)sender;
            ArduinoConnection.setCOM(lbx.SelectedItem.ToString());

            ArduinoConnection.GetInstance().DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            ArduinoConnection.Go();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ArduinoConnection.GetInstance().DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
            ArduinoConnection.Close();
            tbxLogs.Clear();
        }
    }
}
