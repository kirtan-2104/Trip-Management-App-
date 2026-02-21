using TripMate.Helpers;
using TripMate.ViewModels;

namespace TripMate.Views;

public partial class EditTripPage : ContentPage, IQueryAttributable
{
    public EditTripPage() : this(ServiceHelper.GetRequiredService<EditTripViewModel>()) { }

    public EditTripPage(EditTripViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (BindingContext is EditTripViewModel vm && query.TryGetValue("tripId", out var v) && int.TryParse(v?.ToString(), out var id))
            vm.TripId = id;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is EditTripViewModel vm && vm.TripId > 0)
            await vm.LoadCommand.ExecuteAsync(null);
    }
}
