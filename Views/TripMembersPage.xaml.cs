using TripMate.Helpers;
using TripMate.ViewModels;

namespace TripMate.Views;

public partial class TripMembersPage : ContentPage, IQueryAttributable
{
    public TripMembersPage() : this(ServiceHelper.GetRequiredService<TripMembersViewModel>()) { }

    public TripMembersPage(TripMembersViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (BindingContext is TripMembersViewModel vm && query.TryGetValue("tripId", out var v) && int.TryParse(v?.ToString(), out var id))
            vm.TripId = id;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is TripMembersViewModel vm && vm.TripId > 0)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
