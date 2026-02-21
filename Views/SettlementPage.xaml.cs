using TripMate.Helpers;
using TripMate.ViewModels;

namespace TripMate.Views;

public partial class SettlementPage : ContentPage, IQueryAttributable
{
    public SettlementPage() : this(ServiceHelper.GetRequiredService<SettlementViewModel>()) { }

    public SettlementPage(SettlementViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (BindingContext is SettlementViewModel vm && query.TryGetValue("tripId", out var v) && int.TryParse(v?.ToString(), out var id))
            vm.TripId = id;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is SettlementViewModel vm && vm.TripId > 0)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
