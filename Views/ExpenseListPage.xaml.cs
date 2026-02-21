using TripMate.Helpers;
using TripMate.ViewModels;

namespace TripMate.Views;

public partial class ExpenseListPage : ContentPage, IQueryAttributable
{
    public ExpenseListPage() : this(ServiceHelper.GetRequiredService<ExpenseListViewModel>()) { }

    public ExpenseListPage(ExpenseListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (BindingContext is ExpenseListViewModel vm && query.TryGetValue("tripId", out var v) && int.TryParse(v?.ToString(), out var id))
            vm.TripId = id;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ExpenseListViewModel vm && vm.TripId > 0)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
