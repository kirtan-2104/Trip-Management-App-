using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripMate.Models;
using TripMate.Services;

namespace TripMate.ViewModels;

/// <summary>
/// ViewModel for managing trip members.
/// </summary>
public partial class TripMembersViewModel : BaseViewModel
{
    private readonly ITripService _tripService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private int _tripId;

    [ObservableProperty]
    private string _tripName = string.Empty;

    public ObservableCollection<Models.User> Members { get; } = new();

    public TripMembersViewModel(ITripService tripService, IAuthService authService)
    {
        _tripService = tripService;
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (TripId <= 0) return;

        IsBusy = true;
        try
        {
            var trip = await _tripService.GetByIdAsync(TripId);
            if (trip != null)
            {
                TripName = trip.Name;
                var members = await _tripService.GetMembersAsync(TripId);
                Members.Clear();
                foreach (var m in members)
                    Members.Add(m);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadAsync();
    }

    [RelayCommand]
    private async Task AddMemberAsync()
    {
        StatusMessage = "Add member: Use same app to register; then add via Edit Trip. (Full flow can be extended.)";
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

}
