using CommunityToolkit.Mvvm.ComponentModel;
using TripMate.Models;

namespace TripMate.ViewModels;

/// <summary>
/// Item for member picker (dropdown).
/// </summary>
public partial class MemberPickerItem : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _name = string.Empty;

    public static MemberPickerItem FromUser(User u) => new() { Id = u.Id, Name = u.Name };
}
