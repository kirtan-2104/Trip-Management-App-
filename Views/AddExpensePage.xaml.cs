using TripMate.Helpers;
using TripMate.ViewModels;

namespace TripMate.Views;

public partial class AddExpensePage : ContentPage, IQueryAttributable
{
    public AddExpensePage() : this(ServiceHelper.GetRequiredService<AddExpenseViewModel>()) { }

    public AddExpensePage(AddExpenseViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (BindingContext is AddExpenseViewModel vm && query.TryGetValue("tripId", out var v) && int.TryParse(v?.ToString(), out var id))
            vm.TripId = id;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AddExpenseViewModel vm && vm.TripId > 0)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
