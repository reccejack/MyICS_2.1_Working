using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore.Design;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace MyICSv2._1.Models
{
    public class IndustryProcess
    {
        // Navigation property for many-to-many relationship with Device
        public virtual ICollection<Device> Devices { get; set; } = new List<Device>();


        [Key]
        public string ProcessId { get; set; }

        [MaxLength(200)]
        public string ProcessStatus { get; set; } = string.Empty;

        [MaxLength(12)]
        public string Connection { get; set; } = string.Empty;        

        public bool? IsAlarm { get; set; } = null;

        public string ProcessReport { get; set; } = "active";
            
        public IndustryProcess() { ProcessStatus = "Unknown"; }

        public IndustryProcess(string name)
        {
            ProcessId = name;
            ProcessStatus = "Unknown";
        }

        // Adds Device object to the Process navigation collection
        public void AddDevice(Device device)
        {
            if (device == null) return;
            if (!Devices.Contains(device)) Devices.Add(device);
            // Note: inverse navigation (device.Processes) will be managed by EF when tracking entities
        }
    }
}

