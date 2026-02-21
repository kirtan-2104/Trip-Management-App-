using CommunityToolkit.Mvvm.ComponentModel;
using TripMate.Models;

namespace TripMate.ViewModels;

/// <summary>
/// Display item for a trip in list views.
/// </summary>
public partial class TripListItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _destination = string.Empty;

    [ObservableProperty]
    private DateTime _startDate;

    [ObservableProperty]
    private DateTime _endDate;

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    private string _dateRangeText = string.Empty;

    public static TripListItemViewModel FromTrip(Trip trip)
    {
        return new TripListItemViewModel
        {
            Id = trip.Id,
            Name = trip.Name,
            Destination = trip.Destination,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate,
            StatusText = trip.Status.ToString(),
            DateRangeText = $"{trip.StartDate:MMM d, yyyy} - {trip.EndDate:MMM d, yyyy}"
        };
    }
}
