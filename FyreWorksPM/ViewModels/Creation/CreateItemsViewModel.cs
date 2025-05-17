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
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.ApplicationModel;
using static Azure.Core.HttpHeader;
using FyreWorksPM.Services.Navigation;

namespace FyreWorksPM.ViewModels.Creation;

/// <summary>
/// ViewModel for creating, displaying, filtering, and managing items.
/// Fully refactored to use ObservableProperty and RelayCommand.
/// </summary>
public partial class CreateItemsViewModel : ObservableObject
{
    private readonly IItemService _itemService;
    public IItemService ItemService => _itemService;
    private readonly IItemTypeService _itemTypeService;
    private readonly INavigationServices _navigationService;
    public Func<Task> RequestEditItemPopup { get; set; }


    public Action<ItemDto>? ItemSelectedCallback { get; set; }

    public CreateItemsViewModel(IItemService itemService, IItemTypeService itemTypeService,INavigationServices navigationService)
    {
        
        _itemService = itemService;
        _itemTypeService = itemTypeService;
        _navigationService = navigationService;

        _ = LoadItemTypesAsync();
        _ = LoadItemsAsync();
    }

    // Observable properties for form input and filtering
    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string selectedItemType = string.Empty;
    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private bool areSuggestionsVisible;
    [ObservableProperty] private ItemDto? selectedItem;

    public ObservableCollection<ItemDto> Items { get; } = new();
    public ObservableCollection<ItemDto> FilteredItems { get; } = new();
    public ObservableCollection<string> ItemTypes { get; } = new();
    public ObservableCollection<string> FilteredItemTypes { get; } = new();

    partial void OnSelectedItemChanged(ItemDto? value)
    {
        RemoveSelectedItemCommand.NotifyCanExecuteChanged();
    }

    partial void OnSearchTextChanged(string value) => FilterItems();
    partial void OnSelectedItemTypeChanged(string value) => FilterItems();

    [RelayCommand(CanExecute = nameof(CanRemoveSelectedItem))]
    private async Task RemoveSelectedItemAsync()
    {
        if (SelectedItem == null) return;

        await _itemService.DeleteItemAsync(SelectedItem.Id);

        Items.Remove(SelectedItem);
        FilterItems();
        SelectedItem = null;

        await LoadItemTypesAsync();
    }

    private bool CanRemoveSelectedItem() => SelectedItem != null;

    [RelayCommand]
    private async Task AddItemAsync()
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description) || string.IsNullOrWhiteSpace(SelectedItemType))
            return;

        var dto = new CreateItemDto { Name = Name, Description = Description, ItemTypeName = SelectedItemType };
        await _itemService.AddItemAsync(dto);

        await LoadItemsAsync();
        await LoadItemTypesAsync();

        Name = string.Empty;
        Description = string.Empty;
        SelectedItemType = string.Empty;
        FilterItemTypeSuggestions(string.Empty);
    }

    [RelayCommand]
    private async Task EditSelectedItemAsync()
    {
        if (SelectedItem == null) return;
        if (RequestEditItemPopup != null)
            await RequestEditItemPopup.Invoke();
    }

    [RelayCommand]
    private void SelectItemType(string type)
    {
        SelectedItemType = type;
        AreSuggestionsVisible = false;

        var matchedItem = Items.FirstOrDefault(i => i.Name.Equals(type, StringComparison.OrdinalIgnoreCase));
        if (matchedItem != null)
            ItemSelectedCallback?.Invoke(matchedItem);
    }

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

    public void FilterItems()
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

    public async Task LoadItemTypesAsync()
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

    public async Task LoadItemsAsync()
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
}

