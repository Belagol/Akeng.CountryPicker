using AkengCountryPicker.Models;
using AkengCountryPicker.Services;

namespace AkengCountryPicker.Controls;

public partial class CountryPickerView : ContentView
{
    private readonly ICountryService _countryService;
    private IReadOnlyList<CountryInfo> _allCountries = Array.Empty<CountryInfo>();
    private CancellationTokenSource? _searchCancellationTokenSource;
    private bool _isLoaded;

    public event EventHandler<CountryInfo>? CountrySelected;

    #region Bindable properties
    public static readonly BindableProperty SelectedCountryProperty =
        BindableProperty.Create(
            nameof(SelectedCountry),
            typeof(CountryInfo),
            typeof(CountryPickerView),
            null,
            BindingMode.TwoWay);

    public CountryInfo? SelectedCountry
    {
        get => (CountryInfo?)GetValue(SelectedCountryProperty);
        set => SetValue(SelectedCountryProperty, value);
    }

    public static readonly BindableProperty ShowDialCodeProperty =
        BindableProperty.Create(
            nameof(ShowDialCode),
            typeof(bool),
            typeof(CountryPickerView),
            true);

    public bool ShowDialCode
    {
        get => (bool)GetValue(ShowDialCodeProperty);
        set => SetValue(ShowDialCodeProperty, value);
    }

    public static readonly BindableProperty ShowSearchBarProperty =
        BindableProperty.Create(
            nameof(ShowSearchBar),
            typeof(bool),
            typeof(CountryPickerView),
            true);

    public bool ShowSearchBar
    {
        get => (bool)GetValue(ShowSearchBarProperty);
        set => SetValue(ShowSearchBarProperty, value);
    }

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(
            nameof(Placeholder),
            typeof(string),
            typeof(CountryPickerView),
            "Search country...");

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty SearchTextProperty =
        BindableProperty.Create(
            nameof(SearchText),
            typeof(string),
            typeof(CountryPickerView),
            string.Empty,
            BindingMode.TwoWay);

    public string SearchText
    {
        get => (string)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    public static readonly BindableProperty EmptyMessageProperty =
        BindableProperty.Create(
            nameof(EmptyMessageProperty),
            typeof(string),
            typeof(CountryPickerView),
            "No country found");

    public string EmptyMessage
    {
        get => (string)GetValue(EmptyMessageProperty);
        set => SetValue(EmptyMessageProperty, value);
    }

    public static readonly BindableProperty ShowEmojiProperty =
    BindableProperty.Create(
        nameof(ShowEmoji),
        typeof(bool),
        typeof(CountryPickerView),
        true);

    public bool ShowEmoji
    {
        get => (bool)GetValue(ShowEmojiProperty);
        set => SetValue(ShowEmojiProperty, value);
    }

    public static readonly BindableProperty ShowNativeNameProperty =
    BindableProperty.Create(
        nameof(ShowNativeName),
        typeof(bool),
        typeof(CountryPickerView),
        true);

    public bool ShowNativeName
    {
        get => (bool)GetValue(ShowNativeNameProperty);
        set => SetValue(ShowNativeNameProperty, value);
    }

    public static readonly BindableProperty ShowIso2Property =
    BindableProperty.Create(
        nameof(ShowIso2),
        typeof(bool),
        typeof(CountryPickerView),
        false);

    public bool ShowIso2
    {
        get => (bool)GetValue(ShowIso2Property);
        set => SetValue(ShowIso2Property, value);
    }

    public static readonly BindableProperty ShowIso3Property =
        BindableProperty.Create(
            nameof(ShowIso3),
            typeof(bool),
            typeof(CountryPickerView),
            false);

    public bool ShowIso3
    {
        get => (bool)GetValue(ShowIso3Property);
        set => SetValue(ShowIso3Property, value);
    }
    #endregion

    public CountryPickerView()
	{
		InitializeComponent();

        _countryService = new CountryService();

        Loaded += OnLoaded;
        Loaded += OnUnloaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        if (_isLoaded)
            return;

        _isLoaded = true;

        try
        {
            await LoadCountriesAsync();
        }
        catch(Exception ex)
        {
            ApplyCountries(Array.Empty<CountryInfo>());
        }
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        _searchCancellationTokenSource?.Cancel();
        _searchCancellationTokenSource?.Dispose();
        _searchCancellationTokenSource = null;
    }

    private async Task LoadCountriesAsync()
    {
        var countries = await _countryService.GetCountriesAsync();

        _allCountries = countries.OrderBy(c => c.Name).ToArray();

        ApplyCountries(_allCountries);
    }

    private void ApplyCountries(IEnumerable<CountryInfo> countries)
    {
        var result = countries.ToArray();
        CountriesCollectionView.ItemsSource = result;

        EmptyMessageLabel.IsVisible = result.Length == 0;
    }

    private async void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        _searchCancellationTokenSource?.Cancel();
        _searchCancellationTokenSource?.Dispose();

        _searchCancellationTokenSource = new CancellationTokenSource();

        try
        {
            await Task.Delay(TimeSpan.FromMilliseconds(250), _searchCancellationTokenSource.Token);

            var searchText = e.NewTextValue?.Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                ApplyCountries(_allCountries);
                return;
            }

            var result = _allCountries.Where(country =>
                Contains(country.Name, searchText) ||
                Contains(country.NativeName, searchText) ||
                Contains(country.Iso2, searchText) ||
                Contains(country.Iso3, searchText) ||
                Contains(country.DialCode, searchText));

            ApplyCountries(result);
        }
        catch (OperationCanceledException)
        {
            // Une nouvelle saisie a remplacé la précédente.
        }
    }

    private static bool Contains(string? value, string searchText)
    {
        return !string.IsNullOrEmpty(value) && value.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }

    private void OnCountrySelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not CountryInfo country)
            return;

        SelectedCountry = country;
        CountrySelected?.Invoke(this, country);
    }
}