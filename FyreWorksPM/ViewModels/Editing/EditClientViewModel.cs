using System.Windows.Input;
using FyreWorksPM.DataAccess.DTO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.Services.Client;
using FyreWorksPM.Pages.Editing;

namespace FyreWorksPM.ViewModels.Editing;

public partial class EditClientViewModel : ObservableObject
{   
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string contact;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string phone;

    private readonly ClientDto _originalClient;
    private readonly Func<Task> _onSaved;
    private readonly IClientService _clientService;

    public EditClientViewModel(ClientDto client, Func<Task> onSaved, IClientService clientService)
    {
        _originalClient = client;
        _onSaved = onSaved;
        _clientService = clientService;

        // Initialize editable values from the original DTO        
        name = client.Name;
        contact = client.Contact;
        email = client.Email;
        phone = client.Phone;

        
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var view = Shell.Current.CurrentPage as EditClientPage;
        view?.MarkClosingInternally();
        _originalClient.Name = Name;
        _originalClient.Contact = Contact;
        _originalClient.Email = Email;
        _originalClient.Phone = Phone;

       var dto = new UpdateClientDto
       {
           Name = _originalClient.Name,
           Contact = _originalClient.Contact,
           Email = _originalClient.Email,
           Phone = _originalClient.Phone
       };


        await _clientService.UpdateClientAsync(_originalClient.Id, dto);

        // Notify parent to refresh the list
        if (_onSaved != null)
            await _onSaved.Invoke();

        // Proceed with save logic
        await Shell.Current.GoToAsync("..");
    }   
}
