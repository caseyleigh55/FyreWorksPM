using System.Collections.ObjectModel;

namespace FyreWorksPM.Controls;

public partial class SearchableEntryView : ContentView
{
    public SearchableEntryView()
    {
        InitializeComponent();
        FilteredSuggestions = new ObservableCollection<string>();
    }

    public static readonly BindableProperty SuggestionsProperty =
        BindableProperty.Create(nameof(Suggestions), typeof(IEnumerable<string>), typeof(SearchableEntryView), propertyChanged: OnSuggestionsChanged);

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(string), typeof(SearchableEntryView), defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty SearchTextProperty =
        BindableProperty.Create(nameof(SearchText), typeof(string), typeof(SearchableEntryView), string.Empty, BindingMode.TwoWay);

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(SearchableEntryView), "Search...");

    public IEnumerable<string> Suggestions
    {
        get => (IEnumerable<string>)GetValue(SuggestionsProperty);
        set => SetValue(SuggestionsProperty, value);
    }

    public string SelectedItem
    {
        get => (string)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public string SearchText
    {
        get => (string)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public ObservableCollection<string> FilteredSuggestions { get; private set; }

    public bool ShowSuggestions => FilteredSuggestions.Any();

    private static void OnSuggestionsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SearchableEntryView control && newValue is IEnumerable<string> newSuggestions)
        {
            control.UpdateFilteredSuggestions(control.SearchText);
        }
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateFilteredSuggestions(e.NewTextValue);
    }

    private void UpdateFilteredSuggestions(string input)
    {
        FilteredSuggestions.Clear();

        if (!string.IsNullOrWhiteSpace(input) && Suggestions != null)
        {
            var filtered = Suggestions
                .Where(s => s.Contains(input, StringComparison.OrdinalIgnoreCase))
                .Distinct();

            foreach (var suggestion in filtered)
                FilteredSuggestions.Add(suggestion);
        }

        SuggestionsView.IsVisible = FilteredSuggestions.Any();
    }

    private void OnSuggestionSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is string selected)
        {
            SearchText = selected;
            SelectedItem = selected;
            SuggestionsView.IsVisible = false;
            SuggestionsView.SelectedItem = null;
        }
    }
}
