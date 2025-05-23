using Disk.Calculations.Implementations;
using Disk.Db.Context;
using Disk.Entities;
using Disk.Navigators;
using Disk.Services.Interfaces;
using Disk.Stores;
using Disk.ViewModels.Common.Commands.Async;
using Disk.ViewModels.Common.Commands.Sync;
using Disk.ViewModels.Common.ViewModels;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Localization = Disk.Properties.Langs.Session.SessionLocalization;
using Settings = Disk.Properties.Config.Config;

namespace Disk.ViewModels;

public class SessionViewModel(DiskContext database, IExcelFiller excelFiller, NavigationStore navigationStore,
    ModalNavigationStore modalNavigationStore) : PopupViewModel
{
    public required Patient Patient { get; set; }
    public Attempt? SelectedAttempt { get; set; } = null;

    private Session _session = null!;

    public static string MaxXAngle => $"{Settings.XMaxAngle:f2}";
    public static string MaxYAngle => $"{Settings.YMaxAngle:f2}";
    public static string Ip => Settings.IP;
    public static string CursorImageName => Path.GetFileName(CursorImagePath);
    public static string CursorImagePath => Settings.CursorFilePath;
    public static string TargetImageName => Path.GetFileName(TargetImagePath);
    public static string TargetImagePath => Settings.TargetFilePath;
    public static int ShotFrequency => 1000 / Settings.ShotTime;
    public static int TargetLifespan => 1000 * Settings.TargetHp / ShotFrequency;
    private static Settings Settings => Settings.Default;

    public required Session Session
    {
        get => _session;
        set
        {
            _ = SetProperty(ref _session, value);
            UpdateAsync().Wait();
        }
    }

    private Visibility _pathToTargetGridVisibility = Visibility.Visible;
    public Visibility PathToTargetGridVisibility { get => _pathToTargetGridVisibility; set => SetProperty(ref _pathToTargetGridVisibility, value); }

    private Visibility _pathInTargetGridVisibility = Visibility.Hidden;
    public Visibility PathInTargetGridVisibility { get => _pathInTargetGridVisibility; set => SetProperty(ref _pathInTargetGridVisibility, value); }

    private ObservableCollection<Attempt> _attempts = [];
    public ObservableCollection<Attempt> Attempts
    {
        get => _attempts;
        set => SetProperty(ref _attempts, value);
    }

    public ICommand StartAttemptCommand => new Command(_ =>
        QuestionNavigator.Navigate(this, modalNavigationStore,
            message:
                $"""
                    {Localization.Angles}: {MaxXAngle};{MaxYAngle}
                    {Localization.Ip}: {Ip}
                    {Localization.CursorImagePath}: {CursorImageName}
                    {Localization.TargetImagePath}: {TargetImageName}
                    {Localization.ShotTime}: {Calculator.RoundToNearest(ShotFrequency, nearest: 5)}
                    {Localization.TargetHp}: {Calculator.RoundToNearest(TargetLifespan, nearest: 100)}
                """,
            afterConfirm: () =>
            {
                var now = DateTime.Now;
                var logPath = $"{Settings.MainDirPath}{Path.DirectorySeparatorChar}" +
                              $"{Patient.Surname} {Patient.Name}{Path.DirectorySeparatorChar}" +
                              $"{now:dd.MM.yyyy HH-mm-ss}";
                _ = Directory.CreateDirectory(logPath);

                var attempt = new Attempt()
                {
                    Session = Session.Id,
                    CursorRadius = Settings.IniCursorRadius,
                    DateTime = now.ToString("dd.MM.yyyy HH:mm"),
                    MaxXAngle = Settings.XMaxAngle,
                    MaxYAngle = Settings.YMaxAngle,
                    TargetRadius = Settings.IniCursorRadius,
                    LogFilePath = logPath,
                    SamplingInterval = Settings.Default.ShotTime,
                };

                _ = Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    _ = await database.AddAsync(attempt);
                    _ = await database.SaveChangesAsync();

                    // Do not change the order of these lines. It causes a bug in the PaintView. Target is displayed in center
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
                    PaintNavigator.Navigate(this, navigationStore, attempt.Id);
                    //

                    Log.Information("Created session");
                }).Task.ContinueWith(e =>
                {
                    if (e.Exception is not null)
                    {
                        Log.Error("Failed to create session");
                        Log.Error($"{e.Exception.Message} \n {e.Exception.StackTrace}");
                    }
                });
            }));

    public void UpdateAttemptResult(AttemptResult attemptResult)
    {
        _ = database.Update(attemptResult);
        _ = database.SaveChanges();
    }

    public ICommand PathToTargetSelectedCommand => new Command(_ =>
    {
        PathToTargetGridVisibility = Visibility.Visible;
        PathInTargetGridVisibility = Visibility.Hidden;
    });

    public ICommand PathInTargetSelectedCommand => new Command(_ =>
    {
        PathInTargetGridVisibility = Visibility.Visible;
        PathToTargetGridVisibility = Visibility.Hidden;
    });

    public ICommand AttemptSelectedCommand => new Command(_ =>
    {
        if (SelectedAttempt is null)
        {
            return;
        }

        OnPropertyChanged(nameof(SelectedAttempt));
    });

    public ICommand ExportToExcelCommand => new Command(async _ =>
    {
        try
        {
            excelFiller.ExportToExcel(Session, [.. Attempts], Patient, Session.MapNavigation);
        }
        catch (Exception ex)
        {
            Log.Error($"Exception error {ex.Message} {ex.StackTrace}");
            await ShowPopup(header: Localization.SaveFailed, message: "");
        }
    });

    public ICommand ShowAttemptCommand => new AsyncCommand(async _ =>
    {
        if (SelectedAttempt is null)
        {
            return;
        }

        try
        {
            AttemptResultNavigator.Navigate(this, navigationStore, SelectedAttempt.Id);
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }
        catch
        {
            await ShowPopup(header: Localization.ErrorCaption, message: Localization.NoContentError);
        }
    });

    public ICommand DeleteAttemptCommand => new AsyncCommand(async _ =>
    {
        if (SelectedAttempt is null)
        {
            return;
        }

        _ = database.Attempts.Remove(SelectedAttempt);
        _ = await database.SaveChangesAsync();
        _ = Attempts.Remove(SelectedAttempt);

        OnPropertyChanged(nameof(Attempts));
        OnPropertyChanged(nameof(SelectedAttempt));

        SelectedAttempt = null;
    });

    private async Task UpdateAsync()
    {
        Attempts =
        [..
            await database.Attempts
                .Where(s => s.Session == Session.Id)
                .OrderByDescending(s => s.Id)
                .Include(s => s.AttemptResult)
                .Include(s => s.PathInTargets)
                .Include(s => s.PathToTargets)
                .ToListAsync()
        ];
    }

    public override void Refresh()
    {
        base.Refresh();

        _ = Application.Current.Dispatcher.InvokeAsync(UpdateAsync)
            .Task.ContinueWith(e =>
            {
                if (e.Exception is not null)
                {
                    Log.Error($"{e.Exception.Message} \n {e.Exception.StackTrace}");
                }
            });
    }

    public class MergedPaths(PathToTarget pathToTarget, PathInTarget pathInTarget)
    {
        public PathToTarget PathToTarget { get; set; } = pathToTarget;
        public PathInTarget PathInTarget { get; set; } = pathInTarget;
    }
}
