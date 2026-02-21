using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripMate.Services;

namespace TripMate.ViewModels;

/// <summary>
/// ViewModel for creating a new trip.
/// </summary>
public partial class CreateTripViewModel : BaseViewModel
{
    private readonly ITripService _tripService;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _destination = string.Empty;

    [ObservableProperty]
    private DateTime _startDate = DateTime.Today.AddDays(7);

    [ObservableProperty]
    private DateTime _endDate = DateTime.Today.AddDays(14);

    public CreateTripViewModel(ITripService tripService)
    {
        _tripService = tripService;
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
        StatusMessage = string.Empty;

        try
        {
            await _tripService.CreateAsync(Name.Trim(), Destination.Trim(), StartDate, EndDate);
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
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
