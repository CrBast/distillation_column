using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;

namespace Arduino_Viewer
{
    class ArduinoConnection
    {
        private static SerialPort _serialPortArduino = new SerialPort() { BaudRate = 9600, Parity = Parity.None, StopBits = StopBits.One, DataBits = 8, Handshake = Handshake.None, RtsEnable = true };

        private static string _incompleteRequest;

        public static string LineSeparator = "\r\n";

        private ArduinoConnection()
        {

        }

        public static void SetCom(string com)
        {
            if (_serialPortArduino != null)
            {
                if (_serialPortArduino.PortName == null)
                {
                    _serialPortArduino.PortName = com;
                }
                else
                {
                    _serialPortArduino.Close();
                    _serialPortArduino.PortName = com;
                } 
            }
        }

        /// <summary>
        /// Set BaudRate
        /// </summary>
        /// <param name="baudRate">String BaudRate</param>
        public static void SetBaudRate(string baudRate)
        {
            if (_serialPortArduino != null)
                _serialPortArduino.PortName = baudRate;
        }

        /// <summary>
        /// Get instance of serial port
        /// !! COM must be defined
        /// </summary>
        /// <returns></returns>
        public static SerialPort GetInstance()
        {
            return _serialPortArduino.PortName == null ? null : _serialPortArduino;
        }

        /// <summary>
        /// Open SerialPort communication
        /// </summary>
        /// <returns>State of operation</returns>
        public static bool Go()
        {
            try
            {
                _serialPortArduino.Open();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// Close SerialPort connection
        /// !! Remove Event Handler for Data reception
        /// </summary>
        /// <returns>State of operation</returns>
        public static bool Close()
        {
            try
            {
                _serialPortArduino.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// Allows to check that the entry is correct and ends with `lineSeparator`.
        /// </summary>
        /// <param name="s">Input string from Arduino</param>
        /// <returns></returns>
        public static List<string> GetRequests(string s)
        {
            if (_incompleteRequest != null) s = _incompleteRequest + s;

            var requests = Regex.Split(s, LineSeparator).ToList();

            if (!s.EndsWith(LineSeparator))
            {
                if (requests.Count != 0)
                {
                    var lastCommand = requests[requests.Count - 1];
                    if (lastCommand == null) return requests;
                    _incompleteRequest = lastCommand;
                    requests.Remove(lastCommand);
                } 
            }
            else
            {
                _incompleteRequest = null;
            }
            return requests;
        }
    }
}
