using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripMate.Models;
using TripMate.Services;

namespace TripMate.ViewModels;

/// <summary>
/// ViewModel for adding an expense.
/// </summary>
public partial class AddExpenseViewModel : BaseViewModel
{
    private readonly IExpenseService _expenseService;
    private readonly ITripService _tripService;
    private readonly IExpenseCalculationService _calculationService;

    [ObservableProperty]
    private int _tripId;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _amountText = string.Empty;

    [ObservableProperty]
    private string _note = string.Empty;

    [ObservableProperty]
    private MemberPickerItem? _selectedPaidBy;

    [ObservableProperty]
    private ExpenseSplitType _selectedSplitType;

    [ObservableProperty]
    private string _perPersonAmountText = string.Empty;

    public ObservableCollection<MemberPickerItem> Members { get; } = new();

    public AddExpenseViewModel(IExpenseService expenseService, ITripService tripService, IExpenseCalculationService calculationService)
    {
        _expenseService = expenseService;
        _tripService = tripService;
        _calculationService = calculationService;
    }

    public IReadOnlyList<ExpenseSplitType> SplitTypes { get; } = [ExpenseSplitType.Equal, ExpenseSplitType.Custom];

    partial void OnAmountTextChanged(string value)
    {
        UpdatePerPersonAmount();
    }

    partial void OnSelectedSplitTypeChanged(ExpenseSplitType value)
    {
        UpdatePerPersonAmount();
    }

    private void UpdatePerPersonAmount()
    {
        if (!decimal.TryParse(AmountText, out var amount) || Members.Count == 0 || SelectedSplitType != ExpenseSplitType.Equal)
        {
            PerPersonAmountText = string.Empty;
            return;
        }
        var perPerson = _calculationService.GetPerPersonAmountEqual(amount, Members.Count);
        PerPersonAmountText = $"₹{perPerson:N2} per person";
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (TripId <= 0) return;

        IsBusy = true;
        try
        {
            var members = await _tripService.GetMembersAsync(TripId);
            Members.Clear();
            foreach (var m in members)
            {
                var item = MemberPickerItem.FromUser(m);
                Members.Add(item);
                if (SelectedPaidBy == null)
                    SelectedPaidBy = item;
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            StatusMessage = "Expense title is required.";
            HasError = true;
            return;
        }
        if (!decimal.TryParse(AmountText, out var amount) || amount <= 0)
        {
            StatusMessage = "Please enter a valid amount.";
            HasError = true;
            return;
        }
        if (SelectedPaidBy == null)
        {
            StatusMessage = "Please select who paid.";
            HasError = true;
            return;
        }

        IsBusy = true;
        HasError = false;

        try
        {
            await _expenseService.AddAsync(TripId, Title.Trim(), amount, SelectedPaidBy.Id, SelectedSplitType, string.IsNullOrWhiteSpace(Note) ? null : Note.Trim(), null);
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
