#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.System;
#endif

using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.Pages.PopUps;
using FyreWorksPM.ViewModels.Creation;

namespace FyreWorksPM.Pages.Creation;

public partial class CreateItemsPage : ContentPage
{
    private int _suggestionIndex = -1;
    private readonly Action<ItemDto>? _onItemSelected;
    private readonly CreateItemsViewModel _viewModel;


    /// <summary>
    /// Initializes the ItemsPage with a ViewModel and an optional item-selected callback.
    /// </summary>
    /// <param name="vm">The ItemsViewModel for this page.</param>
    /// <param name="onItemSelected">Optional callback to fire when an item is selected from the list.</param>
    public CreateItemsPage(CreateItemsViewModel vm, Action<ItemDto>? onItemSelected = null)
    {
        InitializeComponent();
        BindingContext = vm;
        _viewModel = vm;
        _onItemSelected = onItemSelected;

        // Reset suggestion navigation on focus/text change
        ItemTypeEntry.Focused += (s, e) => ResetSuggestionNavigation();
        ItemTypeEntry.TextChanged += (s, e) => ResetSuggestionNavigation();

#if WINDOWS
        // Handle arrow keys on Windows for suggestion nav
        this.HandlerChanged += (s, e) =>
        {
            var handler = ItemTypeEntry?.Handler?.PlatformView as FrameworkElement;
            if (handler != null)
                handler.KeyDown += OnPlatformKeyDown;
        };
#endif

        // Register callback from ViewModel (when item is tapped)
        vm.ItemSelectedCallback = OnItemSelectedInternal;

        vm.RequestEditItemPopup = async () =>
        {
            var selectedItem = vm.SelectedItem;
            if (selectedItem == null)
                return;

            var popup = new ManageItemPopup(
                selectedItem,
                async () =>
                {
                    await vm.LoadItemsAsync();
                    await vm.LoadItemTypesAsync();
                    vm.FilterItemTypeSuggestions(vm.SearchText);
                    vm.FilterItems();
                },
                vm.ItemService  // Or however you access the item service from the view model
            );

            await Shell.Current.Navigation.PushModalAsync(popup);
        };

    }

    /// <summary>
    /// Internal method triggered when an item is selected.
    /// Fires callback and optionally closes the page.
    /// </summary>
    /// <param name="item">The item selected by the user.</param>
    private async void OnItemSelectedInternal(ItemDto item)
    {
        _onItemSelected?.Invoke(item);
        await Shell.Current.Navigation.PopAsync();
    }


    public void FocusItemName()
    {
        ItemNameEntry.Focus();
    }

    private void ResetSuggestionNavigation()
    {
        _suggestionIndex = -1;
    }

    private void OnEntryCompleted(object sender, EventArgs e)
    {
        if (BindingContext is CreateItemsViewModel vm && vm.AddItemCommand.CanExecute(null))
        {
            vm.AddItemCommand.Execute(null);
        }
    }

    private void OnItemTypeTextChanged(object sender, TextChangedEventArgs e)
    {
        if (BindingContext is CreateItemsViewModel vm)
        {
            vm.FilterItemTypeSuggestions(e.NewTextValue);
        }
    }

#if WINDOWS
    private void OnPlatformKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Down)
        {
            if (BindingContext is CreateItemsViewModel vm && vm.FilteredItemTypes.Any())
            {
                _suggestionIndex = Math.Min(_suggestionIndex + 1, vm.FilteredItemTypes.Count - 1);
                vm.SelectedItemType = vm.FilteredItemTypes[_suggestionIndex];
            }

            e.Handled = true;
        }
        else if (e.Key == VirtualKey.Up)
        {
            if (BindingContext is CreateItemsViewModel vm && vm.FilteredItemTypes.Any())
            {
                _suggestionIndex = Math.Max(_suggestionIndex - 1, 0);
                vm.SelectedItemType = vm.FilteredItemTypes[_suggestionIndex];
            }

            e.Handled = true;
        }
    }
#endif
}
