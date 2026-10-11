using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyICSv2._1.ViewModels;
using MyICSv2._1.Views;
using MyICSv2._1.Data;
using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace MyICSv2._1;

public partial class App : Application
{
    public static IServiceProvider? Services { get; private set; } = null;
    public static IConfiguration? Configuration { get; private set; } = null;
    public static string? DbPath { get; private set; } = null;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Load config from output folder
        Configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), optional: false)
            .Build();

        // Use the Data folder located under the runtime output (bin) directory
        // e.g. <project>/bin/<configuration>/<tfm>/Data
        var dbFolder = Path.Combine(AppContext.BaseDirectory, "Data");
        Directory.CreateDirectory(dbFolder);

        // DB PATH is myicsdb.db under the runtime output Data folder
        var dbPath = Path.Combine(dbFolder, "myicsdb.db");
        DbPath = dbPath;

        var services = new ServiceCollection();

        // FORCE EF CORE TO USE THE DATA FOLDER DB  
        services.AddDbContextFactory<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        Services = services.BuildServiceProvider();

        // Apply migrations
        using (var context = Services.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext())
        {
            context.Database.Migrate();
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }


}