using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripMate.Services;

namespace TripMate.ViewModels;

/// <summary>
/// ViewModel for expense list and reports.
/// </summary>
public partial class ExpenseListViewModel : BaseViewModel
{
    private readonly IExpenseService _expenseService;
    private readonly ITripService _tripService;

    [ObservableProperty]
    private int _tripId;

    [ObservableProperty]
    private MemberPickerItem? _selectedFilterMember;

    public ObservableCollection<ExpenseListItemViewModel> Expenses { get; } = new();
    public ObservableCollection<MemberPickerItem> Members { get; } = new();

    public ExpenseListViewModel(IExpenseService expenseService, ITripService tripService)
    {
        _expenseService = expenseService;
        _tripService = tripService;
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
            Members.Add(new MemberPickerItem { Id = 0, Name = "All members" });
            foreach (var m in members)
                Members.Add(MemberPickerItem.FromUser(m));

            if (SelectedFilterMember == null && Members.Count > 0)
                SelectedFilterMember = Members[0];

            await LoadExpensesAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadExpensesAsync()
    {
        var paidBy = SelectedFilterMember?.Id == 0 ? null : (int?)SelectedFilterMember?.Id;
        var list = await _expenseService.GetByTripIdAsync(TripId, paidBy);
        Expenses.Clear();
        foreach (var e in list)
            Expenses.Add(ExpenseListItemViewModel.FromExpense(e));
    }

    partial void OnSelectedFilterMemberChanged(MemberPickerItem? value)
    {
        if (TripId > 0)
            _ = LoadExpensesAsync();
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
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

}
