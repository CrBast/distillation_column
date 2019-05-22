using System;
using System.IO.Ports;
using System.Threading;

namespace Windows
{
    class ArduinoConnection
    {
        private static SerialPort arduino = new SerialPort() { BaudRate = 9600, Parity = Parity.None, StopBits = StopBits.One, DataBits = 8, Handshake = Handshake.None, RtsEnable = true };

        private ArduinoConnection()
        {

        }

        public static void setCOM(string com)
        {
            if (arduino != null)
            {
                if (arduino.PortName == null)
                {
                    arduino.PortName = com;
                }
                else
                {
                    arduino.Close();
                    arduino.PortName = com;
                } 
            }
        }

        /// <summary>
        /// Set BaudRate
        /// </summary>
        /// <param name="baudRate">String BaudRate</param>
        public static void setBaudRate(string baudRate)
        {
            if (arduino != null)
                arduino.PortName = baudRate;
        }

        /// <summary>
        /// Get instance of serial port
        /// !! COM must be defined
        /// </summary>
        /// <returns></returns>
        public static SerialPort GetInstance()
        {
            return arduino.PortName == null ? null : arduino;
        }

        /// <summary>
        /// Open SerialPort communication
        /// </summary>
        /// <returns>State of operation</returns>
        public static bool Go()
        {
            try
            {
                arduino.Open();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
                throw;
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
                arduino.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
                throw;
            }
        }
    }
}
