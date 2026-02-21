using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripMate.Services;

namespace TripMate.ViewModels;

/// <summary>
/// ViewModel for the Login page.
/// </summary>
public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            StatusMessage = "Please enter email and password.";
            HasError = true;
            return;
        }

        IsBusy = true;
        HasError = false;
        StatusMessage = string.Empty;

        try
        {
            var (user, message) = await _authService.LoginAsync(Email, Password);
            if (user != null)
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
    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync("Register");
    }
}
