using Disk.Entities;
using Disk.Repository.Interface;
using Disk.Sessions;
using Disk.Stores;
using Disk.ViewModel.Common.Commands.Async;
using Disk.ViewModel.Common.Commands.Sync;
using Disk.ViewModel.Common.ViewModels;
using System.Windows.Input;

namespace Disk.ViewModel
{
    public class AddNoteViewModel(ModalNavigationStore modalNavigationStore, INoteRepository noteRepository) : ObserverViewModel
    {
        public event Action<Note>? OnAdd;

        public Note Note { get; set; } = new()
        {
            Patient = AppointmentSession.Patient.Id,
            Doctor = AppSession.Doctor.Id,
        };

        public ICommand CancelCommand => new Command(_ => modalNavigationStore.Close());
        public ICommand AddNoteCommand => new AsyncCommand(AddNote);

        private async Task AddNote(object? arg)
        {
            await noteRepository.AddAsync(Note);
            OnAdd?.Invoke(Note);
            modalNavigationStore.Close();
        }
    }
}