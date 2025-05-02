using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.ViewModels.Solitary;

namespace FyreWorksPM.Pages.PopUps;

public partial class ManageItemTypesPopup : ContentPage
{
    public ManageItemTypesPopup()
    {
        InitializeComponent();

        // Manually wire up the service
        var db = new ApplicationDbContextFactory().CreateDbContext(Array.Empty<string>());
        var service = new ItemTypeService(db);

        BindingContext = new ManageItemTypesPopupViewModel(service);
    }
}
