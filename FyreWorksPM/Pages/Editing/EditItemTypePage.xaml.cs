using FyreWorksPM.DataAccess.Data;
using FyreWorksPM.Services.Item;
using FyreWorksPM.ViewModels.Solitary;

namespace FyreWorksPM.Pages.Editing;

public partial class EditItemTypePage : ContentPage
{
    private readonly IItemTypeService _service;

    public EditItemTypePage(IItemTypeService service)
    {
        InitializeComponent();
        _service = service;
        BindingContext = new EditItemTypeViewModel(service);
    }
}
