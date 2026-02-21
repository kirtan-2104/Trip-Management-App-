using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripMate.Services;

namespace TripMate.ViewModels;

/// <summary>
/// ViewModel for the Register page.
/// </summary>
public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            StatusMessage = "Name is required.";
            HasError = true;
            return;
        }
        if (string.IsNullOrWhiteSpace(Email))
        {
            StatusMessage = "Email is required.";
            HasError = true;
            return;
        }
        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
        {
            StatusMessage = "Password must be at least 6 characters.";
            HasError = true;
            return;
        }
        if (Password != ConfirmPassword)
        {
            StatusMessage = "Passwords do not match.";
            HasError = true;
            return;
        }

        IsBusy = true;
        HasError = false;
        StatusMessage = string.Empty;

        try
        {
            var (success, message) = await _authService.RegisterAsync(Name, Email, Password);
            if (success)
            {
                await Shell.Current.GoToAsync("//TripList");
            }
            else
            {
                StatusMessage = message;
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
