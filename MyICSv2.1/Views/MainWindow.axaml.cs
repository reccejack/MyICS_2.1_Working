using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MyICSv2._1.Models;
using MyICSv2._1.Data;
using MyICSv2._1.ViewModels;
using MyICSv2._1.Migrations;

namespace MyICSv2._1.Views;

public partial class MainWindow : Window
{
    //public ObservableCollection<Device> Devices { get; } = new ObservableCollection<Device>();
    //public ObservableCollection<Process> Processes { get; } = new ObservableCollection<Process>();

    private IDbContextFactory<AppDbContext> _dbContextFactory;


    private MainViewModel ViewModel => (MainViewModel)DataContext;
    public MainWindow()
    {        
        InitializeComponent();

        Loaded += async (_, _) =>
        {
            InputListBox1.ItemsSource = ViewModel.Processes;

            _dbContextFactory = ViewModel._dbFactory;
            //_dbContextFactory = App.Services.GetRequiredService<IDbContextFactory<AppDbContext>>();
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
        if (ViewModel.SelectedProcess is null) { return; }

        var dialog = new AddDeviceWindow();
        var result = await dialog.ShowDialog<Device?>(this);

        if (result is not null && dialog.SelectedProcessId is not null)
        {
            //await ViewModel.AddDeviceAsync(result, ViewModel.SelectedProcess.ProcessId);
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
        //ExampleBlock.Text = $"Process ID: {selectedProcess.ProcessId} Process Name: {selectedProcess.IsAlarm}";
    }

    //EXPERIMENTATION (16 OCT 2026)
    private async void OnClickConnect(object? sender, RoutedEventArgs e)
    {
        //await ViewModel.UpdateDevice(ViewModel.Devices[0].ReadingType);
        for(int i = 0; i < ViewModel.SelectedProcessDevices.Count; i++)
        {
            await ViewModel.UpdateDevice(ViewModel.SelectedProcessDevices[i], "0000");
            System.Diagnostics.Debug.WriteLine($"Selected: {ViewModel.SelectedProcessDevices[i].ReadingType}");
        }        
    }
}