using System.Collections.ObjectModel;
using System.Windows.Input;
using FyreWorksPM.DataAccess.DTO; // Adjust if BidDto lives in DataAccess
using FyreWorksPM.Services.Bid;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FyreWorksPM.ViewModels.Foundation
{
    public partial class BidsPageViewModel : ObservableObject
    {
        private readonly IBidService _bidService;

        public BidsPageViewModel(IBidService bidService)
        {
            _bidService = bidService;

            // Initialize command bindings
            CreateNewBidCommand = new RelayCommand(OnCreateNewBid);
            EditSelectedBidCommand = new RelayCommand(OnEditSelectedBid, CanEditBid);
            ConvertToProjectCommand = new RelayCommand(OnConvertToProject, CanConvertBid);

            // Load the bids from API
            LoadBidsCommand = new AsyncRelayCommand(LoadBidsAsync);
        }

        // Full list of all bids from the API
        private List<BidDto> AllBids = new();

        // Observable collections for UI binding
        [ObservableProperty]
        private ObservableCollection<BidDto> activeBids = new();

        [ObservableProperty]
        private ObservableCollection<BidDto> inactiveBids = new();

        // Bid selected by user in either section
        [ObservableProperty]
        private BidDto? selectedBid;

        // Commands for button binding
        public ICommand CreateNewBidCommand { get; }
        public ICommand EditSelectedBidCommand { get; }
        public ICommand ConvertToProjectCommand { get; }
        public ICommand LoadBidsCommand { get; }

        // Async load from API and split into Active/Inactive
        private async Task LoadBidsAsync()
        {
            try
            {
                var bids = await _bidService.GetAllBidsAsync();
                AllBids = bids.ToList();

                // Separate into active/inactive
                ActiveBids = new ObservableCollection<BidDto>(
                    AllBids.Where(b => b.BidDtoIsActive)); // Assuming IsActive exists

                InactiveBids = new ObservableCollection<BidDto>(
                    AllBids.Where(b => !b.BidDtoIsActive));
            }
            catch (Exception ex)
            {
                // TODO: Show alert/logging as needed
                Console.WriteLine($"Failed to load bids: {ex.Message}");
            }
        }

        private void OnCreateNewBid()
        {
            // TODO: Navigate to CreateBidPage
            Console.WriteLine("Navigating to CreateBidPage...");
        }

        private void OnEditSelectedBid()
        {
            if (SelectedBid == null) return;

            // TODO: Navigate to EditBidPage and pass SelectedBid
            Console.WriteLine($"Editing bid {SelectedBid.BidDtoBidNumber}");
        }

        private void OnConvertToProject()
        {
            if (SelectedBid == null) return;

            // TODO: Later: transform this bid into a project
            Console.WriteLine($"Converting bid {SelectedBid.BidDtoBidNumber} to project...");
        }

        private bool CanEditBid() => SelectedBid != null;
        private bool CanConvertBid() => SelectedBid != null;

        partial void OnSelectedBidChanged(BidDto? value)
        {
            // Notify the UI that command states may have changed
            (EditSelectedBidCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (ConvertToProjectCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }
    }
}
