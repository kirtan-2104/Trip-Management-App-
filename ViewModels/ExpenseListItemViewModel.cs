using CommunityToolkit.Mvvm.ComponentModel;
using TripMate.Models;

namespace TripMate.ViewModels;

/// <summary>
/// Display item for an expense in list views.
/// </summary>
public partial class ExpenseListItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    private string _paidByName = string.Empty;

    [ObservableProperty]
    private string _createdDateText = string.Empty;

    public static ExpenseListItemViewModel FromExpense(Expense e)
    {
        return new ExpenseListItemViewModel
        {
            Id = e.Id,
            Title = e.Title,
            Amount = e.Amount,
            PaidByName = e.PaidByUser?.Name ?? "Unknown",
            CreatedDateText = e.CreatedDate.ToString("MMM d, yyyy")
        };
    }
}
