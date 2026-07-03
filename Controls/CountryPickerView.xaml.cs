using AkengCountryPicker.Models;
using AkengCountryPicker.Services;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;

namespace AkengCountryPicker.Controls;

public partial class CountryPickerView : ContentView
{
    private readonly ICountryService _countryService;
    private readonly ObservableCollection<CountryInfo> _countries = new();
    public event EventHandler<CountryInfo>? CountrySelected;

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

    public CountryPickerView()
	{
		InitializeComponent();

        _countryService = new CountryService();

        CountriesCollectionView.ItemsSource = _countries;

        Loaded += async (_, _) => await LoadCountriesAsync();
    }

    private async Task LoadCountriesAsync()
    {
        _countries.Clear();

        var countries = await _countryService.GetCountriesAsync();

        foreach (var country in countries)
            _countries.Add(country);

        EmptyMessageLabel.IsVisible = _countries.Count == 0;
    }

    private async void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        _countries.Clear();

        var countries = await _countryService.SearchAsync(e.NewTextValue);

        foreach (var country in countries)
            _countries.Add(country);

        EmptyMessageLabel.IsVisible = _countries.Count == 0;
    }

    private void OnCountrySelected(object? sender, SelectionChangedEventArgs e)
    {
        var country = e.CurrentSelection.FirstOrDefault() as CountryInfo;

        if (country == null)
            return;

        SelectedCountry = country;
        CountrySelected?.Invoke(this, country);
    }
}