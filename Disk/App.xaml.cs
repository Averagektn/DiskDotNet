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

namespace Disk
{
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
                .WriteTo
                .File("logs/app.log")
                .CreateLogger();

            var services = new ServiceCollection();
            _ = services.AddDbContext<DiskContext>();

            _ = services.AddSingleton<Func<Type, ObserverViewModel>>(provider =>
                type => (ObserverViewModel)provider.GetRequiredService(type)
            );
            _ = services.AddSingleton<NavigationStore>();
            _ = services.AddSingleton<ModalNavigationStore>();

            _ = services.AddSingleton<IAppointmentRepository, AppointmentRepository>();
            _ = services.AddSingleton<IMapRepository, MapRepository>();
            _ = services.AddSingleton<IPathInTargetRepository, PathInTargetRepository>();
            _ = services.AddSingleton<IPathToTargetRepository, PathToTargetRepository>();
            _ = services.AddSingleton<IPatientRepository, PatientRepository>();
            _ = services.AddSingleton<ISessionRepository, SessionRepository>();
            _ = services.AddSingleton<ISessionResultRepository, SessionResultRepository>();

            _ = services.AddSingleton<IPatientService, PatientService>();
            _ = services.AddSingleton<IExcelFiller, ExcelFiller>();

            _ = services.AddTransient<MainViewModel>();
            _ = services.AddTransient<MapCreatorViewModel>();
            _ = services.AddTransient<MapNamePickerViewModel>();
            _ = services.AddTransient<CalibrationViewModel>();
            _ = services.AddTransient<SettingsViewModel>();
            _ = services.AddTransient<PatientsViewModel>();
            _ = services.AddTransient<AddPatientViewModel>();
            _ = services.AddTransient<AppointmentsListViewModel>();
            _ = services.AddTransient<AppointmentViewModel>();
            _ = services.AddTransient<StartSessionViewModel>();
            _ = services.AddTransient<PaintViewModel>();
            _ = services.AddTransient<NavigationBarLayoutViewModel>();
            _ = services.AddTransient<EditPatientViewModel>();
            _ = services.AddTransient<SessionResultViewModel>();

            _ = services.AddSingleton<MainWindow>(provider =>
            {
                provider
                .GetRequiredService<NavigationStore>()
                .SetViewModel<NavigationBarLayoutViewModel>(vm => vm.CurrentViewModel = provider.GetRequiredService<PatientsViewModel>());

                return new()
                {
                    DataContext = provider.GetRequiredService<MainViewModel>()
                };
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
