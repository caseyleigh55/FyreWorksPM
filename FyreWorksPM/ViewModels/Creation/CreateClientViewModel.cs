using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.DataAccess.DTO; // <-- Use DTOs, not EF Models
using FyreWorksPM.Services.Client;

namespace FyreWorksPM.ViewModels.Creation
{
    /// <summary>
    /// ViewModel for creating a new client.
    /// Handles validation, database saving, and callback on success.
    /// </summary>
    public partial class CreateClientViewModel : ViewModelBase
    {
        // ============================================
        // ============ Private Members ===============
        // ============================================

        private readonly IClientService _clientService;

        // ============================================
        // ============ Public Properties =============
        // ============================================

        /// <summary>
        /// Optional callback to notify when a new client is successfully added.
        /// </summary>
        public Func<ClientDto, Task> ClientAddedCallback { get; set; }

        /// <summary>
        /// Name of the client.
        /// </summary>
        public string ClientName { get => Get<string>(); set => Set(value); }

        /// <summary>
        /// Name of the primary contact for the client.
        /// </summary>
        public string ContactName { get => Get<string>(); set => Set(value); }

        /// <summary>
        /// Email address of the client.
        /// </summary>
        public string Email { get => Get<string>(); set => Set(value); }

        /// <summary>
        /// Phone number of the client.
        /// </summary>
        public string Phone { get => Get<string>(); set => Set(value); }

        // ============================================
        // ============ Constructor ===================
        // ============================================

        public CreateClientViewModel(IClientService clientService)
        {
            _clientService = clientService;
            // ClientAddedCallback will be assigned manually later!
        }

        // ============================================
        // ============ Commands ======================
        // ============================================

        /// <summary>
        /// Saves the new client to the database after validation.
        /// </summary>
        [RelayCommand]
        private async Task SaveClientAsync()
        {
            // Validate input fields
            if (string.IsNullOrWhiteSpace(ClientName) ||
                string.IsNullOrWhiteSpace(ContactName) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Phone))
            {
                await Shell.Current.DisplayAlert("Validation Error", "Please fill out all fields.", "OK");
                return;
            }

            // Create the new client DTO
            var newClientDto = new CreateClientDto
            {
                Name = ClientName,
                Contact = ContactName,
                Email = Email,
                Phone = Phone
            };

            // Save client via API
            await _clientService.AddClientAsync(newClientDto);

            // Optional: You could fetch the newly added client if your API supports returning it
            // For now, just fire the callback if set
            if (ClientAddedCallback != null)
            {
                var clientList = await _clientService.GetAllClientsAsync();
                var addedClient = clientList.LastOrDefault(c =>
                    c.Name == ClientName &&
                    c.Contact == ContactName &&
                    c.Email == Email &&
                    c.Phone == Phone);

                if (addedClient != null)
                    await ClientAddedCallback.Invoke(addedClient);
            }

            // Show success confirmation
            await Shell.Current.DisplayAlert("Success", "Client added successfully!", "OK");

            // Close the CreateClientPage
            await Shell.Current.GoToAsync("..");
        }
    }
}
