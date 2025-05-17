using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.Services.Client;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace FyreWorksPM.ViewModels.Creation;

/// <summary>
/// ViewModel for creating a new client.
/// Handles validation, saving via service, and invokes a callback on success.
/// </summary>
public partial class CreateClientViewModel : ObservableObject
{
    private readonly IClientService _clientService;

    /// <summary>
    /// Optional callback to notify when a new client is successfully added.
    /// </summary>
    public Func<ClientDto, Task>? ClientAddedCallback { get; set; }

    [ObservableProperty] private string clientName = string.Empty;
    [ObservableProperty] private string contactName = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string phone = string.Empty;

    public CreateClientViewModel(IClientService clientService)
    {
        _clientService = clientService;
    }

    /// <summary>
    /// Saves the new client to the database after validation.
    /// </summary>
    [RelayCommand]
    private async Task SaveClientAsync()
    {
        if (string.IsNullOrWhiteSpace(ClientName) ||
            string.IsNullOrWhiteSpace(ContactName) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Phone))
        {
            await Shell.Current.DisplayAlert("Validation Error", "Please fill out all fields.", "OK");
            return;
        }

        var newClientDto = new CreateClientDto
        {
            CreateClientDtoName = ClientName,
            CreateClientDtoContact = ContactName,
            CreateClientDtoEmail = Email,
            CreateClientDtoPhone = Phone
        };

        var createdClient = await _clientService.AddClientAsync(newClientDto);

        if (ClientAddedCallback != null)
        {
            await ClientAddedCallback.Invoke(createdClient);

            // Optional double-check if API doesn't return the full object
            var clientList = await _clientService.GetAllClientsAsync();
            var addedClient = clientList.LastOrDefault(c =>
                c.ClientDtoName == ClientName &&
                c.ClientDtoContact == ContactName &&
                c.ClientDtoEmail == Email &&
                c.ClientDtoPhone == Phone);

            if (addedClient != null)
                await ClientAddedCallback.Invoke(addedClient);
        }

        await Shell.Current.DisplayAlert("Success", "Client added successfully!", "OK");
        await Shell.Current.GoToAsync("..");
    }
}
