using System.ComponentModel;
using System.Runtime.CompilerServices;

using Disk.Stores.Interfaces;

namespace Disk.ViewModels.Common.ViewModels;

public class ObserverViewModel : INotifyPropertyChanged, IDisposable
{
    public required INavigationStore IniNavigationStore;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        if (!Equals(field, newValue))
        {
            field = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        return false;
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public virtual void Refresh() { }
    public virtual void BeforeNavigation() { }
    public virtual void AfterNavigation() { }
}
