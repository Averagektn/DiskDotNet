using Disk.Db.Context;
using Disk.Repository.Implementation;
using Disk.Repository.Interface;
using Disk.Service.Implementation;
using Disk.Service.Interface;
using Disk.Stores;
using Disk.ViewModel;
using Disk.ViewModel.Common.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Windows;

namespace Disk;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    private App()
    {
        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Disk.Properties.Config.Config.Default.Language);
        System.Windows.Media.RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("logs/app.log")
            .CreateLogger();

        var services = new ServiceCollection();
        _ = services.AddDbContext<DiskContext>();

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

        _ = services.AddSingleton<IAppointmentRepository, AppointmentRepository>();
        _ = services.AddSingleton<IMapRepository, MapRepository>();
        _ = services.AddSingleton<IPathInTargetRepository, PathInTargetRepository>();
        _ = services.AddSingleton<IPathToTargetRepository, PathToTargetRepository>();
        _ = services.AddSingleton<ISessionRepository, SessionRepository>();
        _ = services.AddSingleton<ISessionResultRepository, SessionResultRepository>();

        _ = services.AddSingleton<IPatientService, PatientService>();
        _ = services.AddSingleton<IExcelFiller, ExcelFiller>();

        // Common state
        _ = services.AddSingleton<MainViewModel>();
        _ = services.AddSingleton<MapCreatorViewModel>();
        _ = services.AddSingleton<PatientsViewModel>();
        _ = services.AddSingleton<SettingsViewModel>();
        _ = services.AddSingleton<AppointmentsListViewModel>();
        _ = services.AddSingleton<AppointmentViewModel>();
        _ = services.AddSingleton<StartSessionViewModel>();
        _ = services.AddSingleton<SessionResultViewModel>();
        _ = services.AddSingleton<CalibrationViewModel>();

        // New state
        _ = services.AddTransient<MapNamePickerViewModel>();
        _ = services.AddTransient<AddPatientViewModel>();
        _ = services.AddTransient<QuestionViewModel>();
        _ = services.AddTransient<PaintViewModel>();
        _ = services.AddTransient<EditPatientViewModel>();
        
        // Rework
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

        var db = _serviceProvider.GetService<DiskContext>();
        db?.EnsureDatabaseExists();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        Log.Information("App start");

        MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }
}
