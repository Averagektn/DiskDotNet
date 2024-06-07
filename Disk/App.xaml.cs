﻿using Disk.Db.Context;
using Disk.Repository.Implementation;
using Disk.Repository.Interface;
using Disk.Service.Interface;
using Disk.Service.Implementation;
using Disk.Stores;
using Disk.ViewModel;
using Disk.ViewModel.Common.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Microsoft.Extensions.Logging;
using Serilog;

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


            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/app.log")
                .CreateLogger();

            var services = new ServiceCollection();
            _ = services.AddDbContext<DiskContext>();

            _ = services.AddSingleton<Func<Type, ObserverViewModel>>(provider =>
                type => (ObserverViewModel)provider.GetRequiredService(type));
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

            _ = services.AddSingleton<IAuthenticationService, AuthenticationService>();

            _ = services.AddTransient<MainViewModel>();
            _ = services.AddTransient<MenuViewModel>();
            _ = services.AddTransient<AuthenticationViewModel>();

            _ = services.AddSingleton<MainWindow>(provider =>
            {
                provider.GetRequiredService<NavigationStore>().SetViewModel<AuthenticationViewModel>();
                return new MainWindow()
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
