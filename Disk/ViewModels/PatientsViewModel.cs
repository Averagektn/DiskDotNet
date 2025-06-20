﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using Disk.Db.Context;
using Disk.Entities;
using Disk.Navigators;
using Disk.Properties.Langs.Patients;
using Disk.Stores;
using Disk.ViewModels.Common.Commands.Async;
using Disk.ViewModels.Common.Commands.Sync;
using Disk.ViewModels.Common.ViewModels;

using Microsoft.EntityFrameworkCore;

using Serilog;

namespace Disk.ViewModels;

public class PatientsViewModel : ObserverViewModel
{
    private const int PatientsPerPage = 15;

    public Patient? SelectedPatient { get; set; }

    private int TotalPages
    {
        get
        {
            int patientsCount = _database.Patients.Count();

            return (int)Math.Ceiling((double)patientsCount / PatientsPerPage);
        }
    }

    private bool _isNextEnabled = false;
    public bool IsNextEnabled { get => _isNextEnabled; set => SetProperty(ref _isNextEnabled, value); }

    private bool _isPrevEnabled = false;
    public bool IsPrevEnabled { get => _isPrevEnabled; set => SetProperty(ref _isPrevEnabled, value); }

    private int _pageNum = 1;
    public int PageNum
    {
        get => _pageNum;
        set
        {
            _ = SetProperty(ref _pageNum, value);
            _ = Application.Current.Dispatcher.InvokeAsync(GetPagedPatientsAsync).Task.ContinueWith(e =>
            {
                if (e.Exception is not null)
                {
                    Log.Error($"{e.Exception.Message} \n {e.Exception.StackTrace}");
                }
            });
        }
    }

    private string _searchText = string.Empty;
    public string SearchText { get => _searchText; set => SetProperty(ref _searchText, value); }

    private ObservableCollection<Patient> _sortedPatients = [];
    public ObservableCollection<Patient> SortedPatients { get => _sortedPatients; set => SetProperty(ref _sortedPatients, value); }

    private readonly NavigationStore _navigationStore;
    private readonly ModalNavigationStore _modalNavigationStore;
    private readonly DiskContext _database;
    public PatientsViewModel(NavigationStore navigationStore, ModalNavigationStore modalNavigationStore, DiskContext database)
    {
        _navigationStore = navigationStore;
        _modalNavigationStore = modalNavigationStore;
        _database = database;

        GetPagedPatientsAsync().Wait();
        IsNextEnabled = TotalPages > 1;
    }

    public ICommand SearchCommand => new AsyncCommand(async _ =>
    {
        if (SearchText != string.Empty)
        {
            string[] nsp = SearchText.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            IQueryable<Patient> query = _database.Patients.AsQueryable();

            foreach (string word in nsp)
            {
                query = query.Where(p =>
                    EF.Functions.Like(p.Name.ToLower(), $"%{word.ToLower()}%") ||
                    EF.Functions.Like(p.Surname.ToLower(), $"%{word.ToLower()}%") ||
                    (p.Patronymic != null && EF.Functions.Like(p.Patronymic.ToLower(), $"%{word.ToLower()}%")));
            }

            List<Patient> patients = await query.OrderByDescending(p => p.Id).ToListAsync();
            SortedPatients = [.. patients];
            Log.Information($"Patient search by: {string.Join(", ", nsp)}");
        }
        else
        {
            await GetPagedPatientsAsync();
        }
    });

    public ICommand SelectPatientCommand => new Command(_ =>
    {
        if (SelectedPatient is not null)
        {
            SessionsListNavigator.NavigateWithBar(this, _navigationStore, SelectedPatient);
        }
    });

    public ICommand AddPatientCommand => new Command(_ => AddPatientNavigator.Navigate(this, _modalNavigationStore));

    public ICommand DeletePatientCommand => new Command(_ =>
    {
        if (SelectedPatient is null)
        {
            return;
        }

        string question =
            $"{PatientsLocalization.DeletePatientQuestion} " +
            $"{SelectedPatient.Surname} " +
            $"{SelectedPatient.Name} " +
            $"{SelectedPatient.Patronymic}";
        QuestionNavigator.Navigate(this, _modalNavigationStore, question, beforeConfirm: async () =>
        {
            try
            {
                _ = _database.Patients.Remove(SelectedPatient);
                _ = await _database.SaveChangesAsync();
                _ = SortedPatients.Remove(SelectedPatient);
                SelectedPatient = null;

                if (SortedPatients.Count == 0 && PageNum > 1)
                {
                    PageNum--;
                }
                else
                {
                    await GetPagedPatientsAsync();
                }

                IsPrevEnabled = PageNum > 1;
                IsNextEnabled = PageNum < TotalPages;

                SearchText = string.Empty;
            }
            catch (Exception ex)
            {
                Log.Fatal($"Async exception: {ex.Message} \n {ex.StackTrace}");
            }
        });
    });

    public ICommand UpdatePatientCommand => new Command(_ =>
    {
        if (SelectedPatient is null)
        {
            return;
        }

        EditPatientNavigator.Navigate(this, _modalNavigationStore, SelectedPatient);
    });

    public ICommand NextPageCommand => new AsyncCommand(async _ =>
    {
        SearchText = string.Empty;
        PageNum++;
        await GetPagedPatientsAsync();
    });

    public ICommand PrevPageCommand => new AsyncCommand(async _ =>
    {
        SearchText = string.Empty;
        PageNum--;
        await GetPagedPatientsAsync();
    });

    private async Task GetPagedPatientsAsync()
    {
        SortedPatients =
        [..
            await _database.Patients
                .OrderByDescending(p => p.Id)
                .Skip(PatientsPerPage * (PageNum - 1))
                .Take(PatientsPerPage)
                .ToListAsync()
        ];
        IsPrevEnabled = PageNum > 1;
        IsNextEnabled = PageNum < TotalPages;
    }

    public override void Refresh()
    {
        base.Refresh();

        if (SearchText != string.Empty)
        {
            SearchCommand.Execute(SearchText);
        }
        else
        {
            _ = Application.Current.Dispatcher.InvokeAsync(GetPagedPatientsAsync).Task.ContinueWith(e =>
            {
                if (e.Exception is not null)
                {
                    Log.Error($"{e.Exception.Message} \n {e.Exception.StackTrace}");
                }
            });
        }
    }
}
