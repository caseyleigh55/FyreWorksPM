using System.Collections.ObjectModel;

namespace FyreWorksPM.Controls;

public partial class SearchableEntryView : ContentView
{
    public SearchableEntryView()
    {
        InitializeComponent();
        FilteredSuggestions = new ObservableCollection<string>();
        ShowSuggestions = false;
    }

    public static readonly BindableProperty SuggestionsProperty =
        BindableProperty.Create(nameof(Suggestions), typeof(IEnumerable<string>), typeof(SearchableEntryView), propertyChanged: OnSuggestionsChanged);

    public IEnumerable<string> Suggestions
    {
        get => (IEnumerable<string>)GetValue(SuggestionsProperty);
        set => SetValue(SuggestionsProperty, value);
    }

    public static readonly BindableProperty SearchTextProperty =
        BindableProperty.Create(nameof(SearchText), typeof(string), typeof(SearchableEntryView), defaultBindingMode: BindingMode.TwoWay);

    public string SearchText
    {
        get => (string)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(string), typeof(SearchableEntryView), defaultBindingMode: BindingMode.TwoWay);

    public string SelectedItem
    {
        get => (string)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(SearchableEntryView), "Search...");

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public ObservableCollection<string> FilteredSuggestions { get; set; }

    public bool ShowSuggestions { get; set; }

    private static void OnSuggestionsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SearchableEntryView view)
        {
            view.UpdateFilter();
        }
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        SearchText = e.NewTextValue;
        UpdateFilter();
    }

    private void UpdateFilter()
    {
        var query = SearchText?.ToLowerInvariant() ?? "";
        var items = Suggestions?.Where(x => x != null && x.ToLowerInvariant().Contains(query)).Distinct().ToList() ?? new();

        FilteredSuggestions.Clear();
        foreach (var item in items)
        {
            FilteredSuggestions.Add(item);
        }

        ShowSuggestions = FilteredSuggestions.Count > 0;
        SuggestionsView.IsVisible = ShowSuggestions;
    }

    private void OnSuggestionSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is string selected)
        {
            SelectedItem = selected;
            SearchText = selected;
            SuggestionsView.IsVisible = false;
        }
    }
}
