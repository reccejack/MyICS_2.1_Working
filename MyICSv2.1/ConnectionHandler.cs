using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MyICSv2._1.Models;
using MyICSv2._1.ViewModels;
using MyICSv2._1.Views;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using Microsoft.Extensions.Caching.Memory;

namespace MyICSv2._1
{
    public class ConnectionHandler
    {
        public Socket socket;
        public Socket accepted;
        public string? strData = null;
        public byte[]? Buffer { get; set; }
        public bool? ConnectionTaskComplete = null;
        string ConnectionType { get; set; } //TCP, UDP, HTTP, HTTPS, MQTT, etc.
        IPAddress ServerIPAddress { get; set; }
        string DeviceIPAddress { get; set; }
        //IPAddress DeviceIPAddress { get; set; }
        IPEndPoint localEndPoint { get; set; }
        public string ReadingOutput { get; set; }
        public int PortNumber { get; set; }
        public List<string> NAT = new List<string>()
        {
            "169.254.102.3",
            "169.254.102.20",
            "169.254.102.22"
        };

        private readonly Device? _device;
        private readonly Action<Device, string>? _onReading;

        

        public ConnectionHandler() { }
        public ConnectionHandler(Device device)
        {
            //Target specific Device 
            DeviceIPAddress = device.DeviceAddressIPv4.ToString();            
        }

        public ConnectionHandler(Device device, Action<Device, string>? onReading = null)
        {
            //Target specific Device 
            DeviceIPAddress = device.DeviceAddressIPv4.ToString();
            _device = device;
            _onReading = onReading;
        }

        /*Currently need to add a means in which the ConnectionHandler is able to preemptively connect to a pre-determined IP address or Device ID, and 
         then utilize these propterties to both authenticate inbound data, and to establish a I/O stream from which to port incoming device data*/

        public void TcpConnection(string deviceIPAddress, int port)
        {
            //TCP Connection Handler//Acquiring the IP Address of the host machine
            //IPAddress and IPEndPoint information for Server (DCS or SCADA System)        
            
            ServerIPAddress = IPAddress.Any;
            DeviceIPAddress = deviceIPAddress;
            PortNumber = port;

            //Validating the inbound device IP address
            localEndPoint = new IPEndPoint(ServerIPAddress, PortNumber);
            ConnectionTaskComplete = false;

            new Thread(() =>
            {
                //Instantiate the socket
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //Bind the new socket to the local endpoint
                socket.Bind(localEndPoint);

                //Begin listening on the socket and accept the inbound data
                socket.Listen(100);
                TaskCompletionSource<bool> taskComplete = new TaskCompletionSource<bool>();
                while (ConnectionTaskComplete == false)
                {
                try
                {                      
                    
                    accepted = socket.Accept();

                    //Instantiate an empty Buffer        <--Buffer will require more bandwidth to accomodate new string data format "|"
                    //Buffer = new byte[2];
                    Buffer = new byte[3];

                    //Quantify the amount of data read from the socket
                    int bytesRead = accepted.Receive(Buffer);

                        //Instantiate a new byte array and append each new received byte from the Buffer (via bytesRead) per the length of the bytesRead quantity 

                        //Encode the new data on the formatted byte array to a string and update the UI with the new reading                    
                        //strData = Encoding.ASCII.GetString(Buffer);

                        //strData = bytesRead > 0 ? Buffer[0].ToString() : "";
                        //ReadingOutput = strData;
                        //if (_device is not null) _onReading?.Invoke(_device, strData);
                        
                        if(bytesRead == 3)
                        {
                            byte deviceId = Buffer[0];
                            short raw = (short)((Buffer[1] << 8 ) | Buffer[2]); // Combine the two bytes into a short value
                            double tempF = raw / 10.0;

                            //strData = bytesRead > 0 ? Buffer[0].ToString() : "";
                            strData = tempF.ToString("F1"); // Format the temperature to one decimal place
                            ReadingOutput = strData;
                            if (_device is not null) _onReading?.Invoke(_device, strData);
                        }

                        //socket.Close();
                        accepted.Close();
                }
                    catch (Exception ex)
                    {
                        /*Ignore exception*/
                        if (ConnectionTaskComplete == true) break;
                    }
                }

                if (ConnectionTaskComplete == true)
                {
                    //taskComplete.SetResult(true);
                    //Action action2 = () => tempReading.Text = "OFF";  <---ANY actions need to be taken on close?  Put it here.
                    socket?.Close();
                    accepted?.Close();
                    //Dispatcher.UIThread.Post(action2); <--ANY UI.Thread actions need to be taken on close?  Put it here.
                    taskComplete.SetResult(true);
                }

            }).Start();
        }
    }  
}
