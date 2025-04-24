using Disk.Db.Context;
using Disk.Properties.Config;
using Disk.Service.Implementation;
using Disk.Service.Interface;
using Disk.Stores;
using Disk.ViewModel;
using Disk.ViewModel.Common.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Globalization;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Disk;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    private App()
    {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(Config.Default.Language);
        RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File($"./logs/{DateTime.Now:dd.MM.yyyy}/{DateTime.Now:dd.MM.yyyy HH-mm-ss}.log")
            .CreateLogger();

        var services = new ServiceCollection();
        _ = services.AddDbContext<DiskContext>(options => options.UseSqlite(AppConfig.DbConnectionString),
            contextLifetime: ServiceLifetime.Transient);

        _ = services.AddSingleton<Func<Type, ObserverViewModel>>(provider =>
            type =>
            {
                var vm = (ObserverViewModel)provider.GetRequiredService(type);
                Log.Information($"Provider returns {vm.GetType()}");
                return vm;
            }
        );
        _ = services.AddSingleton<NavigationStore>();
        _ = services.AddSingleton<ModalNavigationStore>();

        _ = services.AddTransient<IPatientService, PatientService>();
        _ = services.AddTransient<IExcelFiller, ExcelFiller>();

        _ = services.AddTransient<MainViewModel>();
        _ = services.AddTransient<SettingsViewModel>();
        _ = services.AddTransient<CalibrationViewModel>();
        _ = services.AddTransient<MapCreatorViewModel>();
        _ = services.AddTransient<PatientsViewModel>();
        _ = services.AddTransient<SessionsListViewModel>();
        _ = services.AddTransient<ConfigureSessionViewModel>();
        _ = services.AddTransient<AttemptResultViewModel>();
        _ = services.AddTransient<SessionViewModel>();
        _ = services.AddTransient<MapNamePickerViewModel>();
        _ = services.AddTransient<AddPatientViewModel>();
        _ = services.AddTransient<QuestionViewModel>();
        _ = services.AddTransient<PaintViewModel>();
        _ = services.AddTransient<EditPatientViewModel>();
        _ = services.AddTransient<NavigationBarLayoutViewModel>();

        _ = services.AddSingleton<MainWindow>(provider =>
        {
            provider
                .GetRequiredService<NavigationStore>()
                .SetViewModel<NavigationBarLayoutViewModel>(vm =>
                    vm.CurrentViewModel = provider.GetRequiredService<PatientsViewModel>());

            return new()
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            };
        });

        _serviceProvider = services.BuildServiceProvider();

        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            Log.Fatal("------------------------------------------------------");
            Log.Fatal($"Thread {Environment.CurrentManagedThreadId}");
            Log.Fatal(args.Exception.Message);
            Log.Fatal(args.Exception.StackTrace ?? "Empty stack trace");
            Log.Fatal(args.Exception.TargetSite?.ToString() ?? "No method found");
            Log.Fatal("------------------------------------------------------");

            var db = _serviceProvider.GetService<DiskContext>();
            _ = (db?.SaveChanges());
            Log.Information("Unhandled exception DB save");
        };

        var db = _serviceProvider.GetService<DiskContext>();
        db?.EnsureDatabaseExists();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        Log.Information("------------------------------------------------------");
        Log.Information("App start");

        MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        MainWindow.Show();
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        Exit += App_Exit;

        base.OnStartup(e);
    }

    private void App_Exit(object sender, ExitEventArgs e)
    {
        var db = _serviceProvider.GetService<DiskContext>();
        _ = (db?.SaveChanges());
        Log.Information("App exit DB save");
        Log.Information("------------------------------------------------------");
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Fatal("------------------------------------------------------");
        Log.Fatal(e.Exception.Message);
        Log.Fatal(e.Exception.StackTrace ?? "Empty stack trace");
        Log.Fatal(e.Exception.TargetSite?.ToString() ?? "No method found");
        Log.Fatal("------------------------------------------------------");

        var db = _serviceProvider.GetService<DiskContext>();
        _ = (db?.SaveChanges());
        Log.Information("Unhandled exception DB save");
    }
}
