using Disk.Data.Impl;
using Disk.Db.Context;
using Disk.Entities;
using Disk.Properties.Langs.MapNamePicker;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System.Globalization;
using System.Windows.Input;

namespace Disk.ViewModel;

public class MapNamePickerViewModel(DiskContext database) : PopupViewModel
{
    public required List<Point2D<float>> Map { get; set; }
    public string MapName { get; set; } = string.Empty;

    public ICommand CancelCommand => new Command(_ => IniNavigationStore.Close());

    public ICommand SaveMapCommand => new AsyncCommand(async _ =>
    {
        if (MapName.Trim().Length == 0)
        {
            await ShowPopup(MapNamePickerLocalization.SavingError, MapNamePickerLocalization.EmptyName);
            return;
        }

        var map = new Map()
        {
            Name = MapName,
            CoordinatesJson = JsonConvert.SerializeObject(Map),
            CreatedAtDateTime = DateTime.Now.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)
        };

        try
        {
            _ = await database.Maps.AddAsync(map);
            _ = await database.SaveChangesAsync();
            IniNavigationStore.Close();
        }
        catch (DbUpdateException ex)
        {
            Log.Error(ex.Message);
            await ShowPopup(MapNamePickerLocalization.SavingError, MapNamePickerLocalization.NameDuplication);
        }
    });
}