using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripMate.Models;
using TripMate.Services;

namespace TripMate.ViewModels;

/// <summary>
/// ViewModel for the main trip list (dashboard home).
/// </summary>
public partial class TripListViewModel : BaseViewModel
{
    private readonly ITripService _tripService;
    private readonly IAuthService _authService;

    public ObservableCollection<TripListItemViewModel> Trips { get; } = new();

    [ObservableProperty]
    private string _userName = string.Empty;

    public TripListViewModel(ITripService tripService, IAuthService authService)
    {
        _tripService = tripService;
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoadTripsAsync()
    {
        if (!_authService.IsLoggedIn)
        {
            await Shell.Current.GoToAsync("Login");
            return;
        }

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            UserName = _authService.CurrentUser!.Name;
            var trips = await _tripService.GetMyTripsAsync();
            Trips.Clear();
            foreach (var t in trips)
                Trips.Add(TripListItemViewModel.FromTrip(t));
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
    private async Task RefreshAsync()
    {
        await LoadTripsAsync();
    }

    [RelayCommand]
    private async Task CreateTripAsync()
    {
        await Shell.Current.GoToAsync("CreateTrip");
    }

    [RelayCommand]
    private async Task SelectTripAsync(TripListItemViewModel? item)
    {
        if (item == null) return;
        await Shell.Current.GoToAsync($"TripDetail?tripId={item.Id}");
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        _authService.Logout();
        await Shell.Current.GoToAsync("//Login");
    }
}
