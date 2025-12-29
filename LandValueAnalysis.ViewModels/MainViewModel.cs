using LandValueAnalysis.Common;
using LandValueAnalysis.Models.EventArgs;
using LandValueAnalysis.Services;

namespace LandValueAnalysis.ViewModels;

public sealed class MainViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;

    private BaseViewModel _currentViewModel;

    public BaseViewModel CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel = value;
            OnPropertyChanged();
        }
    }

    public MainViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;
        _navigationService.OnPageChanged += OnViewModelUpdate;
        //Set default view as MapViewModel
        _navigationService.Navigate<MapViewModel>();
    }

    private void OnViewModelUpdate(object sender, NavigationEventArgs e)
    {
        if (CurrentViewModel != null)
        {
            CurrentViewModel.Dispose();
        }
        CurrentViewModel = e.ViewModel;
    }

    public override void Dispose()
    {
        if (CurrentViewModel != null)
        {
            CurrentViewModel.Dispose();
        }
        _navigationService.OnPageChanged -= OnViewModelUpdate;
        GC.SuppressFinalize(this);
        base.Dispose();
    }
}
