using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.Services.Client;
using FyreWorksPM.ViewModels.Editing;

namespace FyreWorksPM.Pages.Editing;



    public partial class EditClientPage : ContentPage
    {
    //===============================================\\
    //================= Constructor =================\\
    //===============================================\\
    public EditClientPage(ClientDto client, Func<Task> onSaved, IClientService service)
        {
            InitializeComponent();
            BindingContext = new EditClientViewModel(client, onSaved, service);
        }
    private bool isClosingInternally = false;

    public void MarkClosingInternally()
    {
        isClosingInternally = true;
    }

    /// <summary>
    /// Handles the cancel button click by closing the popup view.
    /// </summary>
    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
