using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripMate.Models;
using TripMate.Services;

namespace TripMate.ViewModels;

/// <summary>
/// ViewModel for trip detail and dashboard.
/// </summary>
public partial class TripDetailViewModel : BaseViewModel
{
    private readonly ITripService _tripService;
    private readonly IDashboardService _dashboardService;
    private readonly ISettlementService _settlementService;

    [ObservableProperty]
    private int _tripId;

    [ObservableProperty]
    private string _tripName = string.Empty;

    [ObservableProperty]
    private string _destination = string.Empty;

    [ObservableProperty]
    private string _dateRange = string.Empty;

    [ObservableProperty]
    private string _status = string.Empty;

    [ObservableProperty]
    private decimal _totalTripCost;

    [ObservableProperty]
    private decimal _yourTotalPaid;

    [ObservableProperty]
    private decimal _yourBalance;

    [ObservableProperty]
    private int _expenseCount;

    [ObservableProperty]
    private string _settlementSummary = string.Empty;

    public TripDetailViewModel(ITripService tripService, IDashboardService dashboardService, ISettlementService settlementService)
    {
        _tripService = tripService;
        _dashboardService = dashboardService;
        _settlementService = settlementService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (TripId <= 0) return;

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            var trip = await _tripService.GetByIdAsync(TripId);
            if (trip == null)
            {
                await Shell.Current.GoToAsync("..");
                return;
            }

            TripName = trip.Name;
            Destination = trip.Destination;
            DateRange = $"{trip.StartDate:MMM d, yyyy} - {trip.EndDate:MMM d, yyyy}";
            Status = trip.Status.ToString();

            var summary = await _dashboardService.GetTripSummaryAsync(TripId);
            TotalTripCost = summary.TotalTripCost;
            YourTotalPaid = summary.YourTotalPaid;
            YourBalance = summary.YourBalance;
            ExpenseCount = summary.ExpenseCount;

            var settlements = await _settlementService.GetSettlementsAsync(TripId);
            SettlementSummary = settlements.Count == 0
                ? "All settled!"
                : string.Join("\n", settlements.Select(s => $"{s.PayerName} pays {s.ReceiverName} ₹{s.Amount:N2}"));
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
        await LoadAsync();
    }

    [RelayCommand]
    private async Task AddExpenseAsync()
    {
        await Shell.Current.GoToAsync($"AddExpense?tripId={TripId}");
    }

    [RelayCommand]
    private async Task ViewExpensesAsync()
    {
        await Shell.Current.GoToAsync($"ExpenseList?tripId={TripId}");
    }

    [RelayCommand]
    private async Task ViewSettlementsAsync()
    {
        await Shell.Current.GoToAsync($"Settlement?tripId={TripId}");
    }

    [RelayCommand]
    private async Task EditTripAsync()
    {
        await Shell.Current.GoToAsync($"EditTrip?tripId={TripId}");
    }

    [RelayCommand]
    private async Task ManageMembersAsync()
    {
        await Shell.Current.GoToAsync($"TripMembers?tripId={TripId}");
    }

}
