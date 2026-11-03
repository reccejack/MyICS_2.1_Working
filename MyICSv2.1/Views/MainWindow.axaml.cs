using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using MyICSv2._1.Models;
using MyICSv2._1.Data;
using MyICSv2._1.ViewModels;
using MyICSv2._1.Migrations;

namespace MyICSv2._1.Views;

public partial class MainWindow : Window
{
    private IDbContextFactory<AppDbContext> _dbContextFactory;
    private MainViewModel ViewModel => (MainViewModel)DataContext;
    public MainWindow()
    {        
        InitializeComponent();

        Loaded += async (_, _) =>
        {
            InputListBox1.ItemsSource = ViewModel.Processes;

            _dbContextFactory = ViewModel._dbFactory;
            await ViewModel.LoadItemsAsync();
        };
    }    

    //Add new process EventHandler
    private async void OnClick_AddProcess(object? sender, RoutedEventArgs e)
    {
        var dialog = new AddProcessWindow();
        var result = await dialog.ShowDialog<IndustryProcess?>(this);

        if (result is not null)
            await ViewModel.AddProcessAsync(result);
    }

    //Remove process EventHandler
    private async void OnClick_RemoveProcess(object? sender, RoutedEventArgs e)
    {
        if (InputListBox1.SelectedItem is not IndustryProcess selectedItem)
            return;

        await ViewModel.RemoveProcessAsync(selectedItem);
    }

    //Add device to a process EventHandler
    private async void OnClick_AddDevice(object? sender, RoutedEventArgs e)
    {
        //if (ViewModel.SelectedProcess is null) { return; }

        var dialog = new AddDeviceWindow();
        var result = await dialog.ShowDialog<Device?>(this);

        if (result is not null && dialog.SelectedProcessId is not null)
        {
            await ViewModel.AddDeviceAsync(result, dialog.SelectedProcessId);
        }
    }

    //Remove a device from a process EventHandler
    private async void OnClick_RemoveDevice(object? sender, RoutedEventArgs e)
    {
        if (InputListBoxDevices.SelectedItem is not Device selectedItem)
            return;

        await ViewModel.RemoveDeviceAsync(selectedItem);
    }

    private async void OnClick_SelectProcess(object? sender, RoutedEventArgs e)
    {
        if (InputListBox1.SelectedItem is not IndustryProcess selectedProcess)
            return;

        // Load devices for the selected process
        await ViewModel.LoadSelectedProcessDevicesAsync(selectedProcess);
    }

    //EXPERIMENTATION (16 OCT 2026)
    private async void OnClickConnect(object? sender, RoutedEventArgs e)
    {

        //This code is needs to be refactored to accomplish the following:
        /*
         1.  Target the SelectedProcessDevice
         2.  Ingest the I/O data: Temp reading and display it in lieu of generic 'ReadingType' property output
         */

        //foreach (Device d in ViewModel.SelectedProcessDevices)
        //{          
        //    System.Diagnostics.Debug.WriteLine($"Selected: {d.DeviceName}");
        //    System.Diagnostics.Debug.WriteLine($"Selected: {d.DeviceAddressIPv4}");            
        //    var connectionHandler = new ConnectionHandler(d);
        //    connectionHandler.TcpConnection(d.DeviceAddressIPv4.ToString(), Int32.Parse(d.SubProcess));
        //    if (connectionHandler != null)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Selected: {connectionHandler.ReadingOutput}");                
        //        //Thread.Sleep(1000); // Sleep for 1 second to simulate a delay between iterations                
        //    } else
        //    {
        //        System.Diagnostics.Debug.WriteLine($"ConnectionHandler is null");
        //    }
        //}

        //EXPERIMENTATION (03 NOV 2026)
        var vm = ViewModel;
        ConnectionHandler connectionHandler1 = new ConnectionHandler(ViewModel.SelectedProcessDevices[0], (dev, text) => _ = vm.UpdateDevice(dev, text));
        ConnectionHandler connectionHandler2 = new ConnectionHandler(ViewModel.SelectedProcessDevices[1], (dev, text) => _ = vm.UpdateDevice(dev, text));

        connectionHandler1.TcpConnection(ViewModel.SelectedProcessDevices[0].DeviceAddressIPv4.ToString(), Int32.Parse(ViewModel.SelectedProcessDevices[0].SubProcess));
        connectionHandler2.TcpConnection(ViewModel.SelectedProcessDevices[1].DeviceAddressIPv4.ToString(), Int32.Parse(ViewModel.SelectedProcessDevices[1].SubProcess));

    }
}