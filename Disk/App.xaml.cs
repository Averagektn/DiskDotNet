using Disk.Db.Context;
using Disk.Repository.Implementation;
using Disk.Repository.Interface;
using Disk.Stores;
using Disk.View;
using Disk.ViewModel;
using Disk.ViewModel.Common.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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
            Thread.CurrentThread.CurrentUICulture =
                new System.Globalization.CultureInfo(Disk.Properties.Config.Config.Default.LANGUAGE);

            var services = new ServiceCollection();
            _ = services.AddDbContext<DiskContext>();

            _ = services.AddSingleton<Func<Type, ObserverViewModel>>(s => type => (ObserverViewModel)s.GetRequiredService(type));
            _ = services.AddSingleton<NavigationStore>();

            _ = services.AddSingleton<IAppointmentRepository, AppointmentRepository>();
            _ = services.AddSingleton<IDoctorRepository, DoctorRepository>();
            _ = services.AddSingleton<IMapRepository, MapRepository>();
            _ = services.AddSingleton<INoteRepository, NoteRepository>();
            _ = services.AddSingleton<IPathInTargetRepository, PathInTargetRepository>();
            _ = services.AddSingleton<IPathToTargetRepository, PathToTargetRepository>();
            _ = services.AddSingleton<IPatientRepository, PatientRepository>();
            _ = services.AddSingleton<ISessionRepository, SessionRepository>();
            _ = services.AddSingleton<ISesssionResultRepository, SessionResultRepository>();

            _ = services.AddTransient<MenuViewModel>();

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var navigationStore = _serviceProvider.GetRequiredService<NavigationStore>();
            var doctorRepository = _serviceProvider.GetRequiredService<IDoctorRepository>();
            navigationStore.CurrentViewModel = new AuthenticationViewModel(doctorRepository, navigationStore);

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(navigationStore)
            };
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
