using Disk.Entities;
using Disk.Properties.Langs.MapNamePicker;
using Disk.Repository.Interface;
using Disk.Sessions;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class MapNamePickerViewModel(IMapRepository mapRepository, ModalNavigationStore modalNavigationStore) : PopupViewModel
    {
        public ICommand SaveMapCommand => new AsyncCommand(SaveMap);

        private string _mapName = string.Empty;
        public string MapName { get => _mapName; set => SetProperty(ref _mapName, value); }

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
                CoordinatesJson = JsonConvert.SerializeObject(MapSession.Map),
                CreatedAtDateTime = DateTime.Now.ToString(),
                CreatedBy = AppSession.Doctor.Id
            };

            try
            {
                await mapRepository.AddAsync(map);
                modalNavigationStore.Close();
            }
            catch (DbUpdateException ex)
            {
                Log.Error(ex.Message);
                await ShowPopup(MapNamePickerLocalization.SavingError, MapNamePickerLocalization.NameDuplication);
            }
        }
    }
}