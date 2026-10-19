using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MyICSv2._1.Models;
using MyICSv2._1.ViewModels;
using MyICSv2._1.Views;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace MyICSv2._1
{
    public class ConnectionHandler
    {
        static Socket socket;
        static Socket accepted;
        static string? strData = null;
        public static byte[]? Buffer { get; set; }
        public static bool? ConnectionState = null;
        string ConnectionType { get; set; } //TCP, UDP, HTTP, HTTPS, MQTT, etc.
        IPAddress ipAddress { get; set; }
        IPEndPoint localEndPoint { get; set; }
        List<string> DataPackageList { get; set; } = new List<string>();
        public string TempReadingOutput { get; set; }

        public ConnectionHandler(string connectionType)
        {
            //ConnectionHandler Constructor
            ConnectionType = connectionType;
        }

        /*Currently need to add a means in which the ConnectionHandler is able to preemptively connect to a pre-determined IP address or Device ID, and 
         then utilize these propterties to both authenticate inbound data, and to establish a I/O stream from which to port incoming device data*/

        public void TcpConnection(string deviceIPAddress)
        {
            //TCP Connection Handler//Acquiring the IP Address of the host machine
            //IPAddress and IPEndPoint information for Server (DCS or SCADA System)        
            
            ipAddress = IPAddress.Any;
            
            //This IPAddress must be bound to the 'Server' IP
            if (ipAddress == IPAddress.Parse(deviceIPAddress))    //<--TargetIPAddress == 'SelectedProcessDevices<Device> device'
            {
                localEndPoint = new IPEndPoint(ipAddress, 8888);
                ConnectionState = false;
            }

            new Thread(() =>
            {
                TaskCompletionSource<bool> taskComplete = new TaskCompletionSource<bool>();
                while (ConnectionState == false)
                {
                    try
                    {
                        //Instantiate the socket
                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                        //Bind the new socket to the local endpoint
                        socket.Bind(localEndPoint);

                        //Begin listening on the socket and accept the inbound data
                        socket.Listen(100);
                        accepted = socket.Accept();

                        //Instantiate an empty Buffer        <--Buffer will require more bandwidth to accomodate new string data format "|"
                        //Buffer = new byte[2];
                        Buffer = new byte[93];

                        //Quantify the amount of data read from the socket
                        int bytesRead = accepted.Receive(Buffer);


                        //Instantiate a new byte array and append each new received byte from the Buffer (via bytesRead) per the length of the bytesRead quantity 

                        //Encode the new data on the formatted byte array to a string and update the UI with the new reading                    
                        strData = Encoding.ASCII.GetString(Buffer);

                        DataPackageList = strData.Split('|').ToList();
                        TempReadingOutput = DataPackageList[1];


                        socket.Close();
                        accepted.Close();
                }
                    catch (Exception ex)
                    {
                        /*Ignore exception*/
                    }
                }

                if (ConnectionState == true)
                {
                    //taskComplete.SetResult(true);
                    //Action action2 = () => tempReading.Text = "OFF";  <---ANY actions need to be taken on close?  Put it here.
                    socket.Close();
                    accepted.Close();
                    //Dispatcher.UIThread.Post(action2); <--ANY UI.Thread actions need to be taken on close?  Put it here.
                    taskComplete.SetResult(true);
                }

            }).Start();
        }
    }  
}
