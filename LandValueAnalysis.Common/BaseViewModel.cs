using System.ComponentModel;
using System.Runtime.CompilerServices;

//Base view model for ez liskov principle usage. In common to expose viewmodel for use as a dependency
namespace LandValueAnalysis.Common;

public abstract class BaseViewModel : INotifyPropertyChanged, IDisposable
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public virtual void Dispose()
    {
    }
}
