using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.Services.Client;
using System.Collections.ObjectModel;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace FyreWorksPM.ViewModels.Creation;

/// <summary>
/// ViewModel for creating a new client.
/// Handles validation, saving via service, and invokes a callback on success.
/// </summary>
public partial class CreateClientViewModel : ObservableObject
{
    private readonly IClientService _clientService;
    public IClientService ClientService => _clientService;

    //===============================================\\
    //================= Constructor =================\\
    //===============================================\\

    public CreateClientViewModel(IClientService clientService)
    {
        _clientService = clientService;
        _=LoadClientsAsync();
    }
    /// <summary>
    /// Optional callback to notify when a new client is successfully added.
    /// </summary>
    public Func<ClientDto, Task>? ClientAddedCallback { get; set; }
    public Func<ClientDto, Task>? ClientSelectedCallback { get; set; }
    public Func<Task>? RequestEditClientPopup { get; set; }

    [ObservableProperty] private string clientName = string.Empty;
    [ObservableProperty] private string contactName = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string phone = string.Empty;
    [ObservableProperty] private ClientDto? selectedClient;
    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private string selectedClientName = string.Empty;

    public ObservableCollection<ClientDto> ClientList { get; } = new();
    public ObservableCollection<ClientDto> FilteredClients { get; } = new();
    public ObservableCollection<string> FilteredClientNames { get; } = new();
    public ObservableCollection<string> ClientNames { get; } = new();

    partial void OnSearchTextChanged(string value) => FilterClients();
    partial void OnSelectedClientNameChanged(string value) => FilterClients();
    partial void OnSelectedClientChanged(ClientDto? value)
    {
        RemoveSelectedClientCommand.NotifyCanExecuteChanged();
    }
    private bool CanRemoveSelectedClient() => SelectedClient != null;

    [RelayCommand(CanExecute = nameof(CanRemoveSelectedClient))]
    private async Task RemoveSelectedClientAsync()
    {
        if (SelectedClient == null) return;

        await _clientService.DeleteClientAsync(SelectedClient.Id);

        ClientList.Remove(SelectedClient);
        FilterClients();
        SelectedClient = null;

        await LoadClientsAsync();
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
            Name = ClientName,
            Contact = ContactName,
            Email = Email,
            Phone = Phone
        };

        var createdClient = await _clientService.AddClientAsync(newClientDto);

        if (ClientAddedCallback != null)
        {
            await ClientAddedCallback.Invoke(createdClient);

            // Optional double-check if API doesn't return the full object
            var clientList = await _clientService.GetAllClientsAsync();
            var addedClient = clientList.LastOrDefault(c =>
                c.Name == ClientName &&
                c.Contact == ContactName &&
                c.Email == Email &&
                c.Phone == Phone);

            if (addedClient != null)
                await ClientAddedCallback.Invoke(addedClient);
        }

        await Shell.Current.DisplayAlert("Success", "Client added successfully!", "OK");
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task EditSelectedClientAsync()
    {
        if (SelectedClient == null) return;
        if (RequestEditClientPopup != null)
            await RequestEditClientPopup.Invoke();
    }

    public async Task LoadClientsAsync()
    {
        var clients = await _clientService.GetAllClientsAsync();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ClientList.Clear();
            foreach (var client in clients)
                ClientList.Add(client);

            FilterClients();
        });
    }
    public void FilterClients()
    {
        var filtered = ClientList
            .Where(i =>
                (string.IsNullOrWhiteSpace(SearchText) || i.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        FilteredClients.Clear();
        foreach (var client in filtered)
            FilteredClients.Add(client);
    }
}
