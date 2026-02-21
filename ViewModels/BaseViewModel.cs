using CommunityToolkit.Mvvm.ComponentModel;

namespace TripMate.ViewModels;

/// <summary>
/// Base view model with common properties for loading and validation.
/// </summary>
public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;
}
