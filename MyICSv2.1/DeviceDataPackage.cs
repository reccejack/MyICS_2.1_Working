using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyICSv2._1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyICSv2._1
{
    internal class DeviceDataPackage
    {
        //inbound microcontroller string
        string DeviceID { get; set; } //DDPList index 0
        string PhysReading { get; set; } //DDPList index 1   
        string IPv4Address { get; set; } //DDPList index 2
        string IPv6Address { get; set; } //DDPList index 3
        string MACAddress { get; set; } //DDPList index 4
        DateTime Stamp { get; set; }  //DateTime stamp on receipt of DDP from microcontroller
        List<string> DDPList = new List<string>();

        public DeviceDataPackage()
        {
            //DDP Constructor w/ properties
        }

        public async void DDPHandler(string incomingMessage)
        {
            //INSERT I/O code here            
            //[] DeviceID.Is this device recognized? Y / N(build in fault handler)

            DDPList = incomingMessage.Split('|').ToList();
            DeviceID = DDPList[0];
            PhysReading = DDPList[1];           
            IPv4Address = DDPList[2];
            IPv6Address = DDPList[3];
            MACAddress = DDPList[4];
            Stamp = DateTime.Now;
        }

        //THIS CODE NEEDS TO BE IMPLEMENTED IN THE MAINVIEWMODEL CLASS, NOT HERE.  THIS IS JUST A PLACEHOLDER FOR NOW.
        //public async string DDPOutput(List<string> ddpList)
        //{
        //    ddpList = DDPList;
        //    if (ddpList[0] == "DeviceID")  // --------DeviceID flagged by specfic 'SelectedProcessDevice'
        //    {
        //        return ddpList; //< --device reading index
        //    }
        //}
    }
}
