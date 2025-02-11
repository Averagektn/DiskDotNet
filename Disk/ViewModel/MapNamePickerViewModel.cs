using Disk.Data.Impl;
using Disk.Entities;
using Disk.Properties.Langs.MapNamePicker;
using Disk.Repository.Interface;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System.Globalization;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class MapNamePickerViewModel(IMapRepository mapRepository) : PopupViewModel
    {
        public ICommand SaveMapCommand => new AsyncCommand(SaveMap);

        public required List<Point2D<float>> Map { get; set; } 
        public string MapName { get; set; } = string.Empty;

        private async Task SaveMap(object? arg)
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
                await mapRepository.AddAsync(map);
                IniNavigationStore.Close();
            }
            catch (DbUpdateException ex)
            {
                Log.Error(ex.Message);
                await ShowPopup(MapNamePickerLocalization.SavingError, MapNamePickerLocalization.NameDuplication);
            }
        }
    }
}