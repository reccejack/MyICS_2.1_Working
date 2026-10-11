using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MyICSv2._1.Migrations;
using MyICSv2._1.Models;
using MyICSv2._1.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Threading;

namespace MyICSv2._1.Views
{
    public partial class AddDeviceWindow : Window
    {

        public string? SelectedProcessId { get; private set; }
        
        //guard field bool _closing
        private bool _closing;
        public AddDeviceWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            ProcessComboBox.ItemsSource = ((MainViewModel)DataContext).Processes;
        }

        private void OnClick_Save(object? sender, RoutedEventArgs e)
        {
            if (_closing) return;

            var name = DeviceNameField.Text?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {              
                ShowError("Device name cannot be empty.");
                return;
            }

            // Return the new Device as the dialog result
            //Close(new Device(name.Trim()));

            if(ProcessComboBox.SelectedItem is not IndustryProcess selectedProcess)
            {
                ShowError("Please select a process.");
                return;
            }

            var ipv4 = IPv4Field.Text?.Trim() ?? string.Empty;
            if (ipv4.Length > 0 && !System.Net.IPAddress.TryParse(ipv4, out _))
            {
                ShowError("Invalid IPv4 address.");
                return;
            }

            var device = new Device(name)
            {
                SubProcess = SubProcessField.Text?.Trim() ?? string.Empty,
                ReadingType = ReadingTypeField.Text?.Trim() ?? string.Empty,
                DeviceAverage = DeviceAverageField.Text?.Trim() ?? string.Empty,
                DeviceAddressIPv4 = ipv4,
                DeviceAddressIPv6 = IPv6Field.Text?.Trim() ?? string.Empty,
                DeviceStatus = string.IsNullOrWhiteSpace(StatusField.Text) ? "Unknown" : StatusField.Text.Trim(),
                DeviceSecurity = (SecurityComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "AES256",
                IsActive = IsActiveCheck.IsChecked,
                DateTime = DateTime.Now
            };

            SelectedProcessId = selectedProcess.ProcessId;
            _closing = true;
            //Dispatcher.UIThread.Post(()=> Close(device));
            Dispatcher.UIThread.Post(() => Close(device), DispatcherPriority.Background);

        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.IsVisible = true;
        }

        private void OnClick_Cancel(object? sender, RoutedEventArgs e)
        {
            if (_closing) return;
            _closing = true;
            Dispatcher.UIThread.Post(() => Close(null));
        }        
    }
}