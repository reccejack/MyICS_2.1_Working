using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyICSv2._1.Models;
using MyICSv2._1.Data;
using MyICSv2._1.Views;
using MyICSv2._1.Migrations;
using System.Dynamic;
using Avalonia.Controls;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Net.Sockets;

namespace MyICSv2._1.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public ObservableCollection<Device> Devices { get; } = new ObservableCollection<Device>();
    public ObservableCollection<IndustryProcess> Processes { get; } = new ObservableCollection<IndustryProcess>();
    public ObservableCollection<Device> SelectedProcessDevices { get; } = new ObservableCollection<Device>();

    public IDbContextFactory<AppDbContext> _dbFactory;

    //Process Repository specific code BEGIN
    private readonly ProcessRepository _processRepository = new ProcessRepository(App.Services.GetRequiredService<AppDbContext>());

    [ObservableProperty]
    public IndustryProcess? selectedProcess;
    //Process Repository specific code END

    public MainViewModel()
    {
        _dbFactory = App.Services.GetRequiredService<IDbContextFactory<AppDbContext>>();
        _ = LoadProcessesAsync();

    }

    public async Task LoadItemsAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var DeviceItems = await db.Devices.AsNoTracking().OrderBy(d => d.DeviceName).ToListAsync();
        var ProcessItems = await db.Processes.AsNoTracking().OrderBy(p => p.ProcessId).ToListAsync();

        Devices.Clear();
        foreach (var item in DeviceItems)
            Devices.Add(item);

        Processes.Clear();
        foreach (var item in ProcessItems)
            Processes.Add(item);
    }

    public async Task AddDeviceAsync(Device device, string processId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var process = await db.Processes.FirstAsync(p => p.ProcessId == processId);
        process.Devices.Add(device);
        await db.SaveChangesAsync();

        //db.Devices.Add(device);
        //await db.SaveChangesAsync();
        Devices.Add(device);

        if (SelectedProcess?.ProcessId == processId)
        {
            SelectedProcessDevices.Add(device);
        }
    }

    public async Task<bool> RemoveDeviceAsync(Device device)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var rows = await db.Devices.Where(d => d.DeviceId == device.DeviceId).ExecuteDeleteAsync();
        if (rows > 0)
        {
            Devices.Remove(device);
            return true;
        }

        return false;
    }
    
    public async Task AddProcessAsync(IndustryProcess process)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.Processes.Add(process);
        await db.SaveChangesAsync();
        Processes.Add(process);
    }

    public async Task<bool> RemoveProcessAsync(IndustryProcess process)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var rows = await db.Processes.Where(p => p.ProcessId == process.ProcessId).ExecuteDeleteAsync();
        if (rows > 0)
        {
            Processes.Remove(process);
            return true;
        }
        return false;
    }

    private async Task LoadProcessesAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var processes = await db.Processes.AsNoTracking().OrderBy(p => p.ProcessId).ToListAsync();

        // Update the ObservableCollection on the UI thread so bindings refresh correctly
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            Processes.Clear();
            foreach (var process in processes)
            {
                Processes.Add(process);
            }
        });
    }

    partial void OnSelectedProcessChanged(IndustryProcess? value)
    {
        System.Diagnostics.Debug.WriteLine($"Selected: {value?.ProcessId ?? "null"}");
        _ = LoadSelectedProcessDevicesAsync(value);
    }

    public async Task LoadSelectedProcessDevicesAsync(IndustryProcess? process)
    {
        if (process == null)
        {
            
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => SelectedProcessDevices.Clear());
            return;
        }

        await using var db = await _dbFactory.CreateDbContextAsync();
        var devices = await db.Processes
                              .Where(p => p.ProcessId == process.ProcessId)
                              .SelectMany(p => p.Devices)
                              .AsNoTracking()
                              .ToListAsync();

        System.Diagnostics.Debug.WriteLine($"Devices found: {devices.Count}");        
        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            SelectedProcessDevices.Clear();
            foreach (var d in devices)
                SelectedProcessDevices.Add(d);
            // Update status text with selected process and device count and DB path
            var dbPath = MyICSv2._1.App.DbPath ?? "(unknown db path)";                                   
        });
    }

    //public async Task UpdateDevice(Device device)
    //{
    //    //POTENTIAL FUTURE 'HISTORIAN' IMPLEMENTATION
    //    //await using var db = await _dbFactory.CreateDbContextAsync();
    //    //var existingDevice = await db.Devices.FirstOrDefaultAsync(d => d.DeviceId == device.DeviceId);
    //    //if (existingDevice != null)
    //    //{
    //    //    existingDevice.ReadingType = device.ReadingType;
    //    //    existingDevice.DeviceAverage = device.DeviceAverage;
    //    //    existingDevice.DateTime = device.DateTime;
    //    //    await db.SaveChangesAsync();
    //    //}

    //    for (int i = 0; i < SelectedProcessDevices.Count; i++)
    //        if (SelectedProcessDevices[i].DeviceId == device.DeviceId)
    //        {

    //            //PIPE IN I/O DATA HERE
    //            string updatedReadingType = "testString";

    //            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
    //            {
    //                SelectedProcessDevices[i].ReadingType = updatedReadingType; // Notify UI of the change
    //            });

    //            break;
    //        }        
    //}
}
