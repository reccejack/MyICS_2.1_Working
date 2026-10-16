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
using CommunityToolkit.Mvvm.ComponentModel;

namespace MyICSv2._1.Models
{
    public class Device : ObservableObject
    {
        private string _readingType = string.Empty;

        [Key]
        public int DeviceId { get; set; }

        [Required]
        [MaxLength(200)]
        public string DeviceName { get; set; } = string.Empty;
        //Same as DeviceMake on GUI

        [MaxLength(12)]
        public string SubProcess { get; set; } = string.Empty;

        [MaxLength(5)]        
        //public string ReadingType { get; set; } = string.Empty;

        public string ReadingType
        {
            get => _readingType;
            set => SetProperty(ref _readingType, value);
        }


        [MaxLength(5)]
        public string DeviceAverage { get; set; } = string.Empty;

        [MaxLength(15)]
        public string DeviceAddressIPv4 { get; set; } = string.Empty;
        
        [MaxLength(39)]
        public string DeviceAddressIPv6 { get; set; } = string.Empty;
        
        public bool? IsActive { get; set; } = null;

        //public string DeviceStatus => IsActive.HasValue ? (IsActive.Value ? "Active" : "Inactive") : "Unknown";
        public string DeviceStatus { get; set; } = "Unknown";

        public string DeviceSecurity { get; set; } = "AES256";
        public DateTime DateTime { get; set; } = DateTime.Now;
        public Device() { DeviceStatus = "Unknown"; }

        public Device(string name)
        {
            DeviceName = name;
            DeviceStatus = "Unknown";
        }

        // Navigation property for many-to-many relationship with Process
        public virtual ICollection<IndustryProcess> Processes { get; set; } = new List<IndustryProcess>();        
    }
}
