using TripMate.Helpers;
using TripMate.ViewModels;

namespace TripMate.Views;

public partial class CreateTripPage : ContentPage
{
    public CreateTripPage() : this(ServiceHelper.GetRequiredService<CreateTripViewModel>()) { }

    public CreateTripPage(CreateTripViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
