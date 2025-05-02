using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.Services.Item;
using FyreWorksPM.Pages.PopUps;
using FyreWorksPM.ViewModels.Solitary;

namespace FyreWorksPM.ViewModels.Creation;

/// <summary>
/// ViewModel for creating, displaying, filtering, and managing items.
/// Interacts with IItemService and IItemTypeService instead of direct database access.
/// </summary>
public partial class CreateItemsViewModel : INotifyPropertyChanged
{
    private readonly IItemService _itemService;
    private readonly IItemTypeService _itemTypeService;

    /// <summary>
    /// Callback to notify parent page when an item is selected.
    /// </summary>
    public Action<ItemModel>? ItemSelectedCallback { get; set; }

    public CreateItemsViewModel(IItemService itemService, IItemTypeService itemTypeService)
    {
        _itemService = itemService;
        _itemTypeService = itemTypeService;

        // Load initial data
        _ = LoadItemTypesAsync();
        _ = LoadItemsAsync();
    }

    #region ========== Bindable Properties ==========

    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public string Description { get => _description; set => SetProperty(ref _description, value); }
    public string SelectedItemType { get => _selectedItemType; set { SetProperty(ref _selectedItemType, value); FilterItems(); } }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); FilterItems(); } }

    private ItemModel? _selectedItem;
    public ItemModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
            {
                (RemoveSelectedItemCommand as Command)?.ChangeCanExecute();
            }
        }
    }

    public ObservableCollection<ItemModel> Items { get; set; } = new();
    public ObservableCollection<ItemModel> FilteredItems { get; set; } = new();
    public ObservableCollection<string> ItemTypes { get; set; } = new();
    public ObservableCollection<string> FilteredItemTypes { get; set; } = new();

    private bool _areSuggestionsVisible;
    public bool AreSuggestionsVisible
    {
        get => _areSuggestionsVisible;
        set => SetProperty(ref _areSuggestionsVisible, value);
    }

    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _selectedItemType = string.Empty;
    private string _searchText = string.Empty;

    #endregion

    #region ========== Commands ==========

    public ICommand AddItemCommand => new Command(async () => await AddItemAsync());
    public ICommand RemoveSelectedItemCommand => new Command(async () => await RemoveSelectedItemAsync());
    public ICommand SelectItemTypeCommand => new Command<string>(SelectItemType);

    #endregion

    #region ========== Filtering Logic ==========

    /// <summary>
    /// Filters the item type suggestions as the user types.
    /// </summary>
    public void FilterItemTypeSuggestions(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            FilteredItemTypes.Clear();
            AreSuggestionsVisible = false;
            return;
        }

        var filtered = ItemTypes
            .Where(t => t?.ToLowerInvariant().StartsWith(input.ToLowerInvariant()) == true)
            .ToList();

        FilteredItemTypes.Clear();
        foreach (var item in filtered)
            FilteredItemTypes.Add(item);

        AreSuggestionsVisible = FilteredItemTypes.Any();
    }

    /// <summary>
    /// Filters the displayed list of items based on search and selected type.
    /// </summary>
    private void FilterItems()
    {
        var filtered = Items
            .Where(i =>
                (string.IsNullOrWhiteSpace(SearchText) || i.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(SelectedItemType) || (i.ItemType != null && i.ItemType.Name == SelectedItemType)))
            .ToList();

        FilteredItems.Clear();
        foreach (var item in filtered)
            FilteredItems.Add(item);
    }

    /// <summary>
    /// Selects an item type from the suggestion list.
    /// </summary>
    private void SelectItemType(string type)
    {
        SelectedItemType = type;
        AreSuggestionsVisible = false;

        var matchedItem = Items.FirstOrDefault(i => i.Name.Equals(type, StringComparison.OrdinalIgnoreCase));
        if (matchedItem != null)
        {
            ItemSelectedCallback?.Invoke(matchedItem);
        }
    }

    #endregion

    #region ========== Data Loading ==========

    /// <summary>
    /// Loads available item types.
    /// </summary>
    private async Task LoadItemTypesAsync()
    {
        var types = await _itemTypeService.GetAllItemTypeNamesAsync();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            ItemTypes.Clear();
            foreach (var type in types)
                ItemTypes.Add(type);

            SelectedItemType ??= ItemTypes.FirstOrDefault();
        });
    }

    /// <summary>
    /// Loads all items into the Items collection.
    /// </summary>
    private async Task LoadItemsAsync()
    {
        var items = await _itemService.GetAllItemsAsync();

        Items = new ObservableCollection<ItemModel>(items);
        FilterItems();
    }

    #endregion

    #region ========== Add / Remove / Edit Items ==========

    /// <summary>
    /// Adds a new item using the service layer.
    /// </summary>
    private async Task AddItemAsync()
    {
        if (string.IsNullOrWhiteSpace(Name) ||
            string.IsNullOrWhiteSpace(Description) ||
            string.IsNullOrWhiteSpace(SelectedItemType))
        {
            return;
        }

        await _itemService.AddItemAsync(new ItemModel
        {
            Name = Name,
            Description = Description,
            ItemType = new ItemTypeModel
            {
                Name = SelectedItemType,
                Items = new List<ItemModel>() // Required property, even if it's empty
            }
        });

        await LoadItemsAsync();
        await LoadItemTypesAsync();

        Name = string.Empty;
        Description = string.Empty;
        SelectedItemType = string.Empty;

        FilterItemTypeSuggestions(string.Empty);

        // Focus back on name field if available
        if (Application.Current.MainPage is Shell shell &&
            shell.CurrentPage is not null &&
            shell.CurrentPage.BindingContext is CreateItemsViewModel vm)
        {
            // You can optionally implement a Focus method on your page
        }
    }

    /// <summary>
    /// Removes the currently selected item.
    /// </summary>
    private async Task RemoveSelectedItemAsync()
    {
        if (SelectedItem == null)
            return;

        await _itemService.DeleteItemAsync(SelectedItem.Id);

        Items.Remove(SelectedItem);
        FilterItems();
        SelectedItem = null;

        await LoadItemTypesAsync();
    }

    /// <summary>
    /// Edits the currently selected item via popup.
    /// </summary>
    [RelayCommand]
    private async Task EditSelectedItemAsync()
    {
        if (SelectedItem == null)
            return;

        var popup = new ManageItemPopup(new ManageItemPopupViewModel(
            SelectedItem,
            async () =>
            {
                await LoadItemsAsync();
                await LoadItemTypesAsync();
                FilterItemTypeSuggestions(SearchText);
                FilterItems();
            }));

        await Shell.Current.Navigation.PushAsync(popup);
    }

    #endregion

    #region ========== INotifyPropertyChanged Boilerplate ==========

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string name = "")
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
        storage = value;
        OnPropertyChanged(name);
        return true;
    }

    #endregion
}
