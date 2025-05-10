using Disk.Data.Impl;
using Disk.Db.Context;
using Disk.Entities;
using Disk.Properties.Langs.MapNamePicker;
using Disk.ViewModels.Common.Commands.Async;
using Disk.ViewModels.Common.Commands.Sync;
using Disk.ViewModels.Common.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System.Globalization;
using System.Windows.Input;

namespace Disk.ViewModels;

public class MapNamePickerViewModel(DiskContext database) : PopupViewModel
{
    public required List<Point2D<float>> Map { get; set; }
    public Map MapEntity { get; set; } = new()
    {
        Description = string.Empty,
        Name = string.Empty,
        CoordinatesJson = string.Empty,
        Sessions = [],
        CreatedAtDateTime = string.Empty,
        Id = 0,
    };
    public ICommand CancelCommand => new Command(_ => IniNavigationStore.Close());

    public ICommand SaveMapCommand => new AsyncCommand(async _ =>
    {
        if (MapEntity.Name.Trim().Length == 0)
        {
            await ShowPopup(MapNamePickerLocalization.SavingError, MapNamePickerLocalization.EmptyName);
            return;
        }

        MapEntity.CoordinatesJson = JsonConvert.SerializeObject(Map);
        MapEntity.CreatedAtDateTime = DateTime.Now.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);

        try
        {
            _ = await database.Maps.AddAsync(MapEntity);
            _ = await database.SaveChangesAsync();
            IniNavigationStore.Close();
        }
        catch (DbUpdateException ex)
        {
            Log.Error($"Mapp adding error {ex.Message} {ex.StackTrace}");
            await ShowPopup(MapNamePickerLocalization.SavingError, MapNamePickerLocalization.NameDuplication);
        }
    });
}