using Disk.Stores;

using Serilog;

namespace Disk.ViewModels.Common.ViewModels;

public class MainViewModel : ObserverViewModel
{
    private readonly NavigationStore _navigationStore;
    private readonly ModalNavigationStore _modalNavigationStore;

    public ObserverViewModel? CurrentViewModel => _navigationStore.CurrentViewModel;
    public ObserverViewModel? CurrentModalViewModel => _modalNavigationStore.CurrentViewModel;
    public bool IsModalOpen => _modalNavigationStore.IsOpen;

    public MainViewModel(NavigationStore navigationStore, ModalNavigationStore modalNavigationStore)
    {
        _navigationStore = navigationStore;
        _modalNavigationStore = modalNavigationStore;

        _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        _modalNavigationStore.CurrentViewModelChanged += OnCurrentModalViewModelChanged;
    }

    public void CloseModal()
    {
        _modalNavigationStore.Close();
    }

    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);

        while (_navigationStore.CanClose)
        {
            _navigationStore.Close();
        }
        _navigationStore.Close();
        while (_modalNavigationStore.CanClose)
        {
            _modalNavigationStore.Close();
        }
    }

    private void OnCurrentModalViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentModalViewModel));
        OnPropertyChanged(nameof(IsModalOpen));
    }

    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }

    public void Close()
    {
        if (_modalNavigationStore.CanClose)
        {
            Log.Information("Closing modal");
            _modalNavigationStore.Close();
        }
        else if (_navigationStore.CanClose)
        {
            Log.Information("Closing");
            _navigationStore.Close();
        }
    }
}