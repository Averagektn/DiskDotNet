using Disk.Db.Context;
using Disk.Entities;
using Disk.Navigators;
using Disk.Service.Implementation;
using Disk.Service.Interface;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Localization = Disk.Properties.Langs.Appointment.AppointmentLocalization;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModel;

public class AppointmentViewModel(DiskContext database, IExcelFiller excelFiller, NavigationStore navigationStore,
    ModalNavigationStore modalNavigationStore) : PopupViewModel
{
    public required Patient Patient { get; set; }
    public Session? SelectedSession { get; set; } = null;

    private Appointment _appointment = null!;

    public static string MaxXAngle => $"{Settings.XMaxAngle:f2}";
    public static string MaxYAngle => $"{Settings.YMaxAngle:f2}";
    public static string Ip => Settings.IP;
    public static string CursorImageName => Path.GetFileName(CursorImagePath);
    public static string CursorImagePath => Settings.CursorFilePath;
    public static string TargetImageName => Path.GetFileName(TargetImagePath);
    public static string TargetImagePath => Settings.TargetFilePath;
    public static int ShotFrequency => 1000 / Settings.ShotTime;
    public static int MoveFrequency => 1000 / Settings.MoveTime;
    public static int TargetLifespan => 1000 * Settings.TargetHp / ShotFrequency;
    private static Settings Settings => Settings.Default;

    public required Appointment Appointment
    {
        get => _appointment;
        set
        {
            _ = SetProperty(ref _appointment, value);
            _ = Application.Current.Dispatcher.InvokeAsync(UpdateAsync);
        }
    }

    private ObservableCollection<Session> _sessions = [];
    public ObservableCollection<Session> Sessions
    {
        get => _sessions;
        set => SetProperty(ref _sessions, value);
    }

    private ObservableCollection<MergedPaths> _paths = [];
    public ObservableCollection<MergedPaths> Paths
    {
        get => _paths;
        set
        {
            SetProperty(ref _paths, value);
            OnPropertyChanged(nameof(Paths));
        }
    }

    public ICommand StartSessionCommand => new Command(_ =>
        QuestionNavigator.Navigate(this, modalNavigationStore,
            message:
                $"""
                    {Localization.Angles}: {MaxXAngle};{MaxYAngle}
                    {Localization.Ip}: {Ip}
                    {Localization.CursorImagePath}: {CursorImageName}
                    {Localization.TargetImagePath}: {TargetImageName}
                    {Localization.ShotTime}: {ShotFrequency}
                    {Localization.MoveTime}: {MoveFrequency}
                    {Localization.TargetHp}: {TargetLifespan}
                """,
            afterConfirm: () =>
            {
                var now = DateTime.Now;
                var logPath = $"{Settings.MainDirPath}{Path.DirectorySeparatorChar}" +
                              $"{Patient.Surname} {Patient.Name}{Path.DirectorySeparatorChar}" +
                              $"{now:dd.MM.yyyy HH-mm-ss}";
                _ = Directory.CreateDirectory(logPath);

                var session = new Session()
                {
                    Appointment = Appointment.Id,
                    CursorRadius = Settings.IniUserRadius,
                    DateTime = now.ToString("dd.MM.yyyy HH:mm"),
                    MaxXAngle = Settings.XMaxAngle,
                    MaxYAngle = Settings.YMaxAngle,
                    TargetRadius = Settings.IniUserRadius,
                    LogFilePath = logPath,
                };

                _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    _ = await database.AddAsync(session);
                    _ = await database.SaveChangesAsync();
                    PaintNavigator.Navigate(this, navigationStore, session.Id);
                });
            }));

    public ICommand SessionSelectedCommand => new Command(SessionSelected);

    public ICommand ExportToExcelCommand => new Command(async _ =>
    {
        try
        {
            excelFiller.ExportToExcel(Appointment, [.. Sessions],  Patient, Appointment.MapNavigation);
        }
        catch
        {
            await ShowPopup(header: Localization.SaveFailed, message: "");
        }
    });

    public ICommand ShowSessionCommand => new AsyncCommand(async _ =>
    {
        if (SelectedSession is null)
        {
            return;
        }

        try
        {
            SessionResultNavigator.Navigate(this, navigationStore, SelectedSession.Id);
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }
        catch
        {
            await ShowPopup(header: Localization.ErrorCaption, message: Localization.NoContentError);
        }
    });

    public ICommand DeleteSessionCommand => new AsyncCommand(async _ =>
    {
        _ = database.Sessions.Remove(SelectedSession!);
        _ = await database.SaveChangesAsync();
        _ = Sessions.Remove(SelectedSession!);
        OnPropertyChanged(nameof(Sessions));
        Paths.Clear();

        SelectedSession = null;
    });

    private void SessionSelected(object? obj)
    {
        if (SelectedSession is null)
        {
            return;
        }

        Paths = [.. SelectedSession.PathToTargets.Zip(SelectedSession.PathInTargets, (ptt, pit) => new MergedPaths(ptt, pit))];
    }

    private async Task UpdateAsync()
    {
        Sessions =
        [..
            await database.Sessions
                .Where(s => s.Appointment == Appointment.Id)
                .OrderByDescending(s => s.Id)
                .Include(s => s.SessionResult)
                .Include(s => s.PathInTargets)
                .Include(s => s.PathToTargets)
                .ToListAsync()
        ];
    }

    public override void Refresh()
    {
        base.Refresh();

        _ = Application.Current.Dispatcher.InvokeAsync(UpdateAsync);
    }

    public class MergedPaths(PathToTarget pathToTarget, PathInTarget pathInTarget)
    {
        public PathToTarget PathToTarget { get; set; } = pathToTarget;
        public PathInTarget PathInTarget { get; set; } = pathInTarget;
    }
}
