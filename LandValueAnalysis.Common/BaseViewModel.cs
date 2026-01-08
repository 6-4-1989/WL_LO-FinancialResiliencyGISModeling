using System.ComponentModel;
using System.Runtime.CompilerServices;

//Base view model for ez liskov principle usage. In common to expose viewmodel for use as a dependency
namespace LandValueAnalysis.Common;

public abstract class BaseViewModel : BaseNotificationClass, IDisposable
{
    public virtual void Dispose()
    {
    }
}
