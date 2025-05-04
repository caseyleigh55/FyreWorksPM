using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.Services.Item;
using FyreWorksPM.Pages.PopUps;
using FyreWorksPM.ViewModels.Solitary;
using FyreWorksPM.DataAccess.DTO;
using Microsoft.Maui.ApplicationModel;
using System.Xml.Linq;

namespace FyreWorksPM.ViewModels.Creation;

/// <summary>
/// ViewModel for creating, displaying, filtering, and managing items.
/// Now fully API-powered using DTOs.
/// </summary>
public partial class CreateItemsViewModel : INotifyPropertyChanged
{
    private readonly IItemService _itemService;
    private readonly IItemTypeService _itemTypeService;

    public Action<ItemDto>? ItemSelectedCallback { get; set; }

    public CreateItemsViewModel(IItemService itemService, IItemTypeService itemTypeService)
    {
        _itemService = itemService;
        _itemTypeService = itemTypeService;

        _ = LoadItemTypesAsync();
        _ = LoadItemsAsync();
    }

    #region ========== Bindable Properties ==========

    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public string Description { get => _description; set => SetProperty(ref _description, value); }
    public string SelectedItemType { get => _selectedItemType; set { SetProperty(ref _selectedItemType, value); FilterItems(); } }
    public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); FilterItems(); } }

    private ItemDto? _selectedItem;
    public ItemDto? SelectedItem
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

    public ObservableCollection<ItemDto> Items { get; set; } = new();
    public ObservableCollection<ItemDto> FilteredItems { get; set; } = new();
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

    private void FilterItems()
    {
        var filtered = Items
            .Where(i =>
                (string.IsNullOrWhiteSpace(SearchText) || i.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(SelectedItemType) || i.ItemTypeName == SelectedItemType))
            .ToList();

        FilteredItems.Clear();
        foreach (var item in filtered)
            FilteredItems.Add(item);
    }

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

    private async Task LoadItemsAsync()
    {
        var items = await _itemService.GetAllItemsAsync();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Items.Clear();
            foreach (var item in items)
                Items.Add(item);

            FilterItems();
        });
    }

    #endregion

    #region ========== Add / Remove / Edit Items ==========

    private async Task AddItemAsync()
    {
        if (string.IsNullOrWhiteSpace(Name) ||
            string.IsNullOrWhiteSpace(Description) ||
            string.IsNullOrWhiteSpace(SelectedItemType))
        {
            return;
        }

        var dto = new CreateItemDto
        {
            Name = Name,
            Description = Description,
            ItemTypeName = SelectedItemType
        };

        await _itemService.AddItemAsync(dto);

        await LoadItemsAsync();
        await LoadItemTypesAsync();

        Name = string.Empty;
        Description = string.Empty;
        SelectedItemType = string.Empty;

        FilterItemTypeSuggestions(string.Empty);
    }

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

    [RelayCommand]
    private async Task EditSelectedItemAsync()
    {
        if (SelectedItem == null)
            return;

        var popup = new ManageItemPopup(
            SelectedItem,
            async () =>
            {
                await LoadItemsAsync();
                await LoadItemTypesAsync();
                FilterItemTypeSuggestions(SearchText);
                FilterItems();
            },
            _itemService); // 👈 this is the missing 3rd parameter!

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
