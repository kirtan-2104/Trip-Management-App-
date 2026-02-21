using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripMate.Services;

namespace TripMate.ViewModels;

/// <summary>
/// ViewModel for settlement (who owes whom) display.
/// </summary>
public partial class SettlementViewModel : BaseViewModel
{
    private readonly ISettlementService _settlementService;

    [ObservableProperty]
    private int _tripId;

    [ObservableProperty]
    private string _tripName = string.Empty;

    [ObservableProperty]
    private string _settlementText = string.Empty;

    public SettlementViewModel(ISettlementService settlementService)
    {
        _settlementService = settlementService;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (TripId <= 0) return;

        IsBusy = true;
        try
        {
            var settlements = await _settlementService.GetSettlementsAsync(TripId);
            SettlementText = settlements.Count == 0
                ? "All settled! No one owes anyone."
                : string.Join("\n\n", settlements.Select(s => $"• {s.PayerName} pays {s.ReceiverName} ₹{s.Amount:N2}"));
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
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
