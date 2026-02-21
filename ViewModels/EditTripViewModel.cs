using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripMate.Models;
using TripMate.Services;

namespace TripMate.ViewModels;

/// <summary>
/// ViewModel for editing an existing trip.
/// </summary>
public partial class EditTripViewModel : BaseViewModel
{
    private readonly ITripService _tripService;

    [ObservableProperty]
    private int _tripId;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _destination = string.Empty;

    [ObservableProperty]
    private DateTime _startDate;

    [ObservableProperty]
    private DateTime _endDate;

    [ObservableProperty]
    private TripStatus _selectedStatus;

    public EditTripViewModel(ITripService tripService)
    {
        _tripService = tripService;
    }

    public IReadOnlyList<TripStatus> StatusOptions { get; } = [TripStatus.Upcoming, TripStatus.Ongoing, TripStatus.Completed];

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (TripId <= 0) return;

        IsBusy = true;
        try
        {
            var trip = await _tripService.GetByIdAsync(TripId);
            if (trip == null)
            {
                await Shell.Current.GoToAsync("..");
                return;
            }
            Name = trip.Name;
            Destination = trip.Destination;
            StartDate = trip.StartDate;
            EndDate = trip.EndDate;
            SelectedStatus = trip.Status;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            StatusMessage = "Trip name is required.";
            HasError = true;
            return;
        }
        if (string.IsNullOrWhiteSpace(Destination))
        {
            StatusMessage = "Destination is required.";
            HasError = true;
            return;
        }
        if (EndDate < StartDate)
        {
            StatusMessage = "End date must be after start date.";
            HasError = true;
            return;
        }

        IsBusy = true;
        HasError = false;

        try
        {
            await _tripService.UpdateAsync(TripId, Name.Trim(), Destination.Trim(), StartDate, EndDate, SelectedStatus);
            await Shell.Current.GoToAsync("..");
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
    private async Task DeleteAsync()
    {
        IsBusy = true;
        try
        {
            await _tripService.DeleteAsync(TripId);
            await Shell.Current.GoToAsync("//TripList");
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
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

}
