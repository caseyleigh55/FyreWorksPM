using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.Services.Item;
using FyreWorksPM.ViewModels.Solitary;

namespace FyreWorksPM.Pages.PopUps;

public partial class ManageItemTypesPopup : ContentPage
{
    private readonly IItemTypeService _service;

    public ManageItemTypesPopup(IItemTypeService service)
    {
        InitializeComponent();
        _service = service;
        BindingContext = new ManageItemTypesPopupViewModel(service);
    }
}
