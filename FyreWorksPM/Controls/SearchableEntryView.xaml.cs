using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;

namespace FyreWorksPM.Controls;

public partial class SearchableEntryView : ContentView
{
    public SearchableEntryView()
    {
        InitializeComponent();
        FilteredSuggestions = new ObservableCollection<object>();
        SuggestionsView.IsVisible = false;
    }

    // Suggestions source (i.e. your full list)
    public static readonly BindableProperty SuggestionsProperty =
        BindableProperty.Create(nameof(Suggestions), typeof(IEnumerable), typeof(SearchableEntryView), propertyChanged: OnSuggestionsChanged);

    public IEnumerable Suggestions
    {
        get => (IEnumerable)GetValue(SuggestionsProperty);
        set => SetValue(SuggestionsProperty, value);
    }

    // What the user types
    public static readonly BindableProperty SearchTextProperty =
        BindableProperty.Create(nameof(SearchText), typeof(string), typeof(SearchableEntryView), defaultBindingMode: BindingMode.TwoWay);

    public string SearchText
    {
        get => (string)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    // Selected item from suggestions
    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SearchableEntryView), defaultBindingMode: BindingMode.TwoWay);

    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    // Placeholder for the entry field
    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(SearchableEntryView), "Search...");

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    // Optional: property to display (i.e. "Name" on a complex object)
    public static readonly BindableProperty DisplayMemberPathProperty =
        BindableProperty.Create(nameof(DisplayMemberPath), typeof(string), typeof(SearchableEntryView), "");

    public string DisplayMemberPath
    {
        get => (string)GetValue(DisplayMemberPathProperty);
        set => SetValue(DisplayMemberPathProperty, value);
    }

    // Internal filtered list (bound to your ListView or CollectionView)
    public ObservableCollection<object> FilteredSuggestions { get; private set; }

    // === Event Handlers ===

    private static void OnSuggestionsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SearchableEntryView view)
        {
            view.ApplyFilter();
        }
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        SearchText = e.NewTextValue;
        ApplyFilter();
    }

    private void OnSuggestionSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is object selected)
        {
            SelectedItem = selected;

            // Use DisplayMemberPath if provided
            if (!string.IsNullOrWhiteSpace(DisplayMemberPath))
            {
                var prop = selected.GetType().GetProperty(DisplayMemberPath);
                SearchText = prop?.GetValue(selected)?.ToString() ?? "";
            }
            else
            {
                SearchText = selected.ToString();
            }

            SuggestionsView.IsVisible = false;
        }
    }

    private void ApplyFilter()
    {
        var query = SearchText?.ToLowerInvariant() ?? "";
        var items = new List<object>();

        foreach (var item in Suggestions ?? Enumerable.Empty<object>())
        {
            string value = item.ToString();

            if (!string.IsNullOrWhiteSpace(DisplayMemberPath))
            {
                var prop = item.GetType().GetProperty(DisplayMemberPath);
                value = prop?.GetValue(item)?.ToString() ?? "";
            }

            if (!string.IsNullOrWhiteSpace(value) && value.ToLowerInvariant().Contains(query))
            {
                items.Add(item);
            }
        }

        FilteredSuggestions.Clear();
        foreach (var item in items)
            FilteredSuggestions.Add(item);

        SuggestionsView.IsVisible = FilteredSuggestions.Any();
    }
}
