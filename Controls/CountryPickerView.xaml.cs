using AkengCountryPicker.Models;
using AkengCountryPicker.Ressources.Themes;
using AkengCountryPicker.Services;
using System.Collections.ObjectModel;

namespace AkengCountryPicker.Controls;

public partial class CountryPickerView : ContentView
{
    private readonly ICountryService _countryService;
    private readonly ICountryDetectionService _countryDetectionService;
    private readonly ICountryHistoryService _countryHistoryService;
    private ResourceDictionary? _activeThemeDictionary;
    private AppTheme _lastSystemTheme;
    private IReadOnlyList<CountryInfo> _allCountries = Array.Empty<CountryInfo>();
    private IReadOnlyDictionary<string, CountryInfo> _countriesByIso2 = new Dictionary<string, CountryInfo>(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _favoriteIso2Codes = [];
    private readonly List<string> _recentIso2Codes = [];
    private CancellationTokenSource? _searchCancellationTokenSource;
    private bool _isLoaded;
    private bool _isHandlingSelection;

    public event EventHandler<CountryInfo>? CountrySelected;

    private readonly ObservableCollection<CountryGroup> _countryGroups = [];

    private CountryGroup? _favoritesGroup;
    private CountryGroup? _recentGroup;
    private CountryGroup? _allCountriesGroup;

    private bool _isUpdatingFavorite;

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

    public static readonly BindableProperty DetectionModeProperty =
        BindableProperty.Create(
            nameof(DetectionMode),
            typeof(CountryDetectionMode),
            typeof(CountryPickerView),
            CountryDetectionMode.None);

    public CountryDetectionMode DetectionMode
    {
        get => (CountryDetectionMode)GetValue(DetectionModeProperty);
        set => SetValue(DetectionModeProperty, value);
    }

    public static readonly BindableProperty ShowFavoritesProperty =
    BindableProperty.Create(
        nameof(ShowFavorites),
        typeof(bool),
        typeof(CountryPickerView),
        true);

    public bool ShowFavorites
    {
        get => (bool)GetValue(ShowFavoritesProperty);
        set => SetValue(ShowFavoritesProperty, value);
    }

    public static readonly BindableProperty ShowRecentCountriesProperty =
        BindableProperty.Create(
            nameof(ShowRecentCountries),
            typeof(bool),
            typeof(CountryPickerView),
            true);

    public bool ShowRecentCountries
    {
        get => (bool)GetValue(ShowRecentCountriesProperty);
        set => SetValue(ShowRecentCountriesProperty, value);
    }

    public static readonly BindableProperty MaxRecentCountriesProperty =
        BindableProperty.Create(
            nameof(MaxRecentCountries),
            typeof(int),
            typeof(CountryPickerView),
            5);

    public int MaxRecentCountries
    {
        get => (int)GetValue(MaxRecentCountriesProperty);
        set => SetValue(MaxRecentCountriesProperty, value);
    }

    public static readonly BindableProperty AllowFavoriteSelectionProperty =
        BindableProperty.Create(
            nameof(AllowFavoriteSelection),
            typeof(bool),
            typeof(CountryPickerView),
            true);

    public bool AllowFavoriteSelection
    {
        get => (bool)GetValue(AllowFavoriteSelectionProperty);
        set => SetValue(AllowFavoriteSelectionProperty, value);
    }

    public static readonly BindableProperty ThemeProperty =
        BindableProperty.Create(
            nameof(Theme),
            typeof(CountryPickerTheme),
            typeof(CountryPickerView),
            CountryPickerTheme.System,
            propertyChanged: OnThemeChanged);

    public CountryPickerTheme Theme
    {
        get => (CountryPickerTheme)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    public static readonly BindableProperty TemplateModeProperty =
    BindableProperty.Create(
        nameof(TemplateMode),
        typeof(CountryItemTemplateMode),
        typeof(CountryPickerView),
        CountryItemTemplateMode.Default,
        propertyChanged: OnTemplateModeChanged);

    public CountryItemTemplateMode TemplateMode
    {
        get => (CountryItemTemplateMode)GetValue(TemplateModeProperty);
        set => SetValue(TemplateModeProperty, value);
    }

    
    #endregion

    public CountryPickerView()
	{
		InitializeComponent();

        _countryService = new CountryService();
        _countryDetectionService = new CountryDetectionService();
        _countryHistoryService = new CountryHistoryService();

        CountriesCollectionView.ItemsSource = _countryGroups;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        ApplyTheme();
        ApplyCountryTemplate();
    }

    #region Methods
    private static void OnTemplateModeChanged(BindableObject bindable, object oldValue,  object newValue)
    {
        if (bindable is CountryPickerView picker)
        {
            picker.ApplyCountryTemplate();
        }
    }

    private void ApplyCountryTemplate()
    {
        if (CountriesCollectionView is null)
            return;

        var resourceKey = TemplateMode switch
        {
            CountryItemTemplateMode.Compact =>
                "CompactCountryTemplate",

            CountryItemTemplateMode.Phone =>
                "PhoneCountryTemplate",

            CountryItemTemplateMode.Iso =>
                "IsoCountryTemplate",

            CountryItemTemplateMode.NativeName =>
                "NativeNameCountryTemplate",

            CountryItemTemplateMode.FlagOnly =>
                "FlagOnlyCountryTemplate",

            _ =>
                "DefaultCountryTemplate"
        };

        if (Resources.TryGetValue(resourceKey, out var resource) &&
            resource is DataTemplate template)
        {
            CountriesCollectionView.ItemTemplate = template;
        }
    }

    private void ApplyTheme()
    {
        var effectiveTheme = Theme switch
        {
            CountryPickerTheme.Light => AppTheme.Light,
            CountryPickerTheme.Dark => AppTheme.Dark,
            _ => GetCurrentSystemTheme()
        };

        _lastSystemTheme = effectiveTheme;

        ResourceDictionary dictionary = effectiveTheme switch
        {
            AppTheme.Dark => new CountryPickerDarkTheme(),
            _ => new CountryPickerLightTheme()
        };

        if (_activeThemeDictionary is not null)
        {
            Resources.MergedDictionaries.Remove(_activeThemeDictionary);
        }

        _activeThemeDictionary = dictionary;

        Resources.MergedDictionaries.Add(_activeThemeDictionary);
    }

    private static AppTheme GetCurrentSystemTheme()
    {
        if (Application.Current is null)
            return AppTheme.Light;

        return Application.Current.RequestedTheme switch
        {
            AppTheme.Dark => AppTheme.Dark,
            _ => AppTheme.Light
        };
    }

    private static void OnThemeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CountryPickerView picker)
        {
            picker.ApplyTheme();
        }
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        SubscribeToThemeChanges();
        ApplyTheme();

        if (_isLoaded)
            return;

        _isLoaded = true;

        try
        {
            await LoadCountriesAsync();
            await DetectAndSelectCountryAsync();
        }
        catch(Exception ex)
        {
            ApplyCountries(Array.Empty<CountryInfo>());
        }
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        UnsubscribeFromThemeChanges();

        _searchCancellationTokenSource?.Cancel();
        _searchCancellationTokenSource?.Dispose();
        _searchCancellationTokenSource = null;
    }

    private void SubscribeToThemeChanges()
    {
        if (Application.Current is not null)
        {
            Application.Current.RequestedThemeChanged -=  OnRequestedThemeChanged;

            Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
        }
    }

    private void UnsubscribeFromThemeChanges()
    {
        if (Application.Current is not null)
        {
            Application.Current.RequestedThemeChanged -= OnRequestedThemeChanged;
        }
    }

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        if (Theme != CountryPickerTheme.System)
            return;

        ApplyTheme();
    }

    private async Task LoadCountriesAsync()
    {
        var countries = await _countryService.GetCountriesAsync();

        _allCountries = countries.Where(country => !string.IsNullOrWhiteSpace(country.Iso2))
            .GroupBy(country => country.Iso2.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(country => country.Name)
            .ToArray();

        _countriesByIso2 = _allCountries.ToDictionary(country => country.Iso2.Trim(), country => country, StringComparer.OrdinalIgnoreCase);

        LoadStoredCountryData();

        ApplyCountries(_allCountries);
    }

    private void LoadStoredCountryData()
    {
        _favoriteIso2Codes.Clear();
        _favoriteIso2Codes.AddRange(_countryHistoryService.GetFavoriteIso2Codes());

        _recentIso2Codes.Clear();
        _recentIso2Codes.AddRange(_countryHistoryService.GetRecentIso2Codes());

        var favoriteSet = _favoriteIso2Codes.ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var country in _allCountries)
        {
            country.IsFavorite = favoriteSet.Contains(country.Iso2);
        }
    }

    private void ApplyCountries(IEnumerable<CountryInfo> countries)
    {
        var filteredCountries = countries.ToArray();
        var groups = BuildGroups(filteredCountries);

        _countryGroups.Clear();

        foreach (var group in groups)
        {
            _countryGroups.Add(group);
        }

        RefreshGroupReferences();

        EmptyMessageLabel.IsVisible =
            _countryGroups.Sum(group => group.Count) == 0;

        //var filteredCountries = countries.ToArray();

        //var groups = BuildGroups(filteredCountries);

        //CountriesCollectionView.ItemsSource = groups;

        //EmptyMessageLabel.IsVisible = groups.Sum(group => group.Count) == 0;
    }
    private void RefreshGroupReferences()
    {
        _favoritesGroup = _countryGroups.FirstOrDefault(
            group => group.Key == "favorites");

        _recentGroup = _countryGroups.FirstOrDefault(
            group => group.Key == "recent");

        _allCountriesGroup = _countryGroups.FirstOrDefault(
            group => group.Key == "all");
    }

    private IReadOnlyList<CountryGroup> BuildGroups(IReadOnlyCollection<CountryInfo> filteredCountries)
    {
        var groups = new List<CountryGroup>();

        var filteredIso2Codes = filteredCountries.Select(country => country.Iso2)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var alreadyDisplayed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (ShowFavorites)
        {
            var favorites = GetCountriesInStoredOrder(_favoriteIso2Codes, filteredIso2Codes);

            if (favorites.Count > 0)
            {
                groups.Add(new CountryGroup("favorites", "Favorites", favorites));

                foreach (var country in favorites)
                    alreadyDisplayed.Add(country.Iso2);
            }
        }

        if (ShowRecentCountries)
        {
            var recents = GetCountriesInStoredOrder(_recentIso2Codes, filteredIso2Codes)
                .Where(country => !alreadyDisplayed.Contains(country.Iso2))
                .ToArray();

            if (recents.Length > 0)
            {
                groups.Add(new CountryGroup("recent", "Recent countries", recents));

                foreach (var country in recents)
                    alreadyDisplayed.Add(country.Iso2);
            }
        }

        var remainingCountries = filteredCountries.Where(country => !alreadyDisplayed.Contains(country.Iso2))
            .OrderBy(country => country.Name)
            .ToArray();

        if (remainingCountries.Length > 0)
        {
            groups.Add(new CountryGroup("all", "All countries", remainingCountries));
        }

        return groups;
    }

    private IReadOnlyList<CountryInfo> GetCountriesInStoredOrder(IEnumerable<string> storedIso2Codes, IReadOnlySet<string> filteredIso2Codes)
    {
        return storedIso2Codes.Where(iso2 => !string.IsNullOrWhiteSpace(iso2))
            .Select(iso2 => iso2.Trim())
            .Where(filteredIso2Codes.Contains)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(iso2 => _countriesByIso2.TryGetValue(iso2, out var country) ? country : null)
            .Where(country => country is not null)
            .Cast<CountryInfo>()
            .ToArray();
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

            var result = FilterCountries(searchText);

            ApplyCountries(result);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private static bool Contains(string? value, string searchText)
    {
        return !string.IsNullOrWhiteSpace(value) && value.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }

    private void OnCountrySelected(object? sender, SelectionChangedEventArgs e)
    {
        if (_isHandlingSelection)
            return;

        if (e.CurrentSelection.FirstOrDefault() is not CountryInfo country)
            return;

        if (string.Equals(SelectedCountry?.Iso2, country.Iso2, StringComparison.OrdinalIgnoreCase))
            return;

        try
        {
            _isHandlingSelection = true;

            SelectedCountry = country;
            
            AddToRecentCountries(country);

            CountrySelected?.Invoke(this, country);
        }
        finally
        {
            _isHandlingSelection = false;
        }
    }

    private void AddToRecentCountries(CountryInfo country)
    {
        var normalizedIso2 = country.Iso2
       .Trim()
       .ToUpperInvariant();

        _recentIso2Codes.RemoveAll(iso2 =>
            string.Equals(
                iso2,
                normalizedIso2,
                StringComparison.OrdinalIgnoreCase));

        _recentIso2Codes.Insert(0, normalizedIso2);

        var maximum = Math.Max(0, MaxRecentCountries);

        if (_recentIso2Codes.Count > maximum)
        {
            _recentIso2Codes.RemoveRange(
                maximum,
                _recentIso2Codes.Count - maximum);
        }

        _countryHistoryService.SetRecentIso2Codes(
            _recentIso2Codes);

        RefreshRecentGroup(country);

        //_recentIso2Codes.RemoveAll(iso2 => string.Equals(iso2, country.Iso2, StringComparison.OrdinalIgnoreCase));

        //_recentIso2Codes.Insert(0, country.Iso2.ToUpperInvariant());

        //var maximum = Math.Max(0, MaxRecentCountries);

        //if (_recentIso2Codes.Count > maximum)
        //{
        //    _recentIso2Codes.RemoveRange(maximum, _recentIso2Codes.Count - maximum);
        //}

        //_countryHistoryService.SetRecentIso2Codes(_recentIso2Codes);
    }

    private void RefreshRecentGroup(CountryInfo country)
    {
        if (!ShowRecentCountries || MaxRecentCountries <= 0)
            return;

        // Le pays ne doit être présent que dans un seul groupe.
        RemoveCountryFromGroup(_recentGroup, country.Iso2);

        if (country.IsFavorite)
        {
            // Les favoris restent uniquement dans Favorites.
            return;
        }

        RemoveCountryFromGroup(_allCountriesGroup, country.Iso2);

        EnsureRecentGroup();

        _recentGroup!.Insert(0, country);

        while (_recentGroup.Count > MaxRecentCountries)
        {
            var removedCountry =
                _recentGroup[_recentGroup.Count - 1];

            _recentGroup.RemoveAt(
                _recentGroup.Count - 1);

            if (!removedCountry.IsFavorite)
            {
                InsertCountryAlphabetically(
                    _allCountriesGroup,
                    removedCountry);
            }
        }
    }

    private void EnsureRecentGroup()
    {
        if (_recentGroup is not null)
            return;

        _recentGroup = new CountryGroup(
            "recent",
            "Recent countries",
            Array.Empty<CountryInfo>());

        var insertionIndex =
            _favoritesGroup is not null ? 1 : 0;

        _countryGroups.Insert(
            Math.Min(insertionIndex, _countryGroups.Count),
            _recentGroup);
    }

    private void OnFavoriteClicked(object? sender, EventArgs e)
    {
        if (_isUpdatingFavorite)
            return;

        if (sender is not Button
            {
                BindingContext: CountryInfo country
            })
        {
            return;
        }

        try
        {
            _isUpdatingFavorite = true;

            country.IsFavorite = !country.IsFavorite;

            UpdateFavoriteStorage(country);
            RefreshFavoriteGroups(country);
        }
        finally
        {
            _isUpdatingFavorite = false;
        }

        //if (sender is not Button { BindingContext: CountryInfo country })
        //    return;

        //country.IsFavorite = !country.IsFavorite;

        //_favoriteIso2Codes.RemoveAll(iso2 => string.Equals(iso2, country.Iso2, StringComparison.OrdinalIgnoreCase));

        //if (country.IsFavorite)
        //{
        //    _favoriteIso2Codes.Insert(0, country.Iso2.ToUpperInvariant());
        //}

        //_countryHistoryService.SetFavoriteIso2Codes(_favoriteIso2Codes);
    }
    private void UpdateFavoriteStorage(
    CountryInfo country)
    {
        var normalizedIso2 = country.Iso2
            .Trim()
            .ToUpperInvariant();

        _favoriteIso2Codes.RemoveAll(iso2 =>
            string.Equals(
                iso2,
                normalizedIso2,
                StringComparison.OrdinalIgnoreCase));

        if (country.IsFavorite)
        {
            _favoriteIso2Codes.Insert(
                0,
                normalizedIso2);
        }

        _countryHistoryService.SetFavoriteIso2Codes(
            _favoriteIso2Codes);
    }
    private void RefreshFavoriteGroups(
    CountryInfo country)
    {
        RemoveCountryFromGroup(
            _favoritesGroup,
            country.Iso2);

        RemoveCountryFromGroup(
            _recentGroup,
            country.Iso2);

        RemoveCountryFromGroup(
            _allCountriesGroup,
            country.Iso2);

        if (country.IsFavorite)
        {
            EnsureFavoritesGroup();

            _favoritesGroup!.Insert(0, country);

            RemoveEmptyGroups();
            return;
        }

        var isRecent = _recentIso2Codes.Any(iso2 =>
            string.Equals(
                iso2,
                country.Iso2,
                StringComparison.OrdinalIgnoreCase));

        if (ShowRecentCountries && isRecent)
        {
            EnsureRecentGroup();
            InsertCountryInRecentOrder(country);
        }
        else
        {
            EnsureAllCountriesGroup();

            InsertCountryAlphabetically(
                _allCountriesGroup,
                country);
        }

        RemoveEmptyGroups();
    }

    private void EnsureAllCountriesGroup()
    {
        if (_allCountriesGroup is not null)
            return;

        _allCountriesGroup = new CountryGroup(
            "all",
            "All countries",
            Array.Empty<CountryInfo>());

        _countryGroups.Add(_allCountriesGroup);
    }

    private void InsertCountryInRecentOrder(
    CountryInfo country)
    {
        if (_recentGroup is null)
            return;

        RemoveCountryFromGroup(
            _recentGroup,
            country.Iso2);

        var countryPosition =
            _recentIso2Codes.FindIndex(iso2 =>
                string.Equals(
                    iso2,
                    country.Iso2,
                    StringComparison.OrdinalIgnoreCase));

        if (countryPosition < 0)
        {
            _recentGroup.Add(country);
            return;
        }

        var insertionIndex = 0;

        foreach (var existingCountry in _recentGroup)
        {
            var existingPosition =
                _recentIso2Codes.FindIndex(iso2 =>
                    string.Equals(
                        iso2,
                        existingCountry.Iso2,
                        StringComparison.OrdinalIgnoreCase));

            if (existingPosition < 0 ||
                existingPosition > countryPosition)
            {
                break;
            }

            insertionIndex++;
        }

        _recentGroup.Insert(
            Math.Min(insertionIndex, _recentGroup.Count),
            country);
    }

    private void EnsureFavoritesGroup()
    {
        if (_favoritesGroup is not null)
            return;

        _favoritesGroup = new CountryGroup(
            "favorites",
            "Favorites",
            Array.Empty<CountryInfo>());

        _countryGroups.Insert(0, _favoritesGroup);
    }

    private static void RemoveCountryFromGroup(
    CountryGroup? group,
    string iso2)
    {
        if (group is null)
            return;

        var country = group.FirstOrDefault(item =>
            string.Equals(
                item.Iso2,
                iso2,
                StringComparison.OrdinalIgnoreCase));

        if (country is not null)
        {
            group.Remove(country);
        }
    }

    private static void InsertCountryAlphabetically(
    CountryGroup? group,
    CountryInfo country)
    {
        if (group is null)
            return;

        RemoveCountryFromGroup(
            group,
            country.Iso2);

        var index = 0;

        while (index < group.Count &&
               string.Compare(
                   group[index].Name,
                   country.Name,
                   StringComparison.CurrentCultureIgnoreCase) < 0)
        {
            index++;
        }

        group.Insert(index, country);
    }

    private void RemoveEmptyGroups()
    {
        if (_favoritesGroup is { Count: 0 })
        {
            _countryGroups.Remove(_favoritesGroup);
            _favoritesGroup = null;
        }

        if (_recentGroup is { Count: 0 })
        {
            _countryGroups.Remove(_recentGroup);
            _recentGroup = null;
        }

        if (_allCountriesGroup is { Count: 0 })
        {
            _countryGroups.Remove(_allCountriesGroup);
            _allCountriesGroup = null;
        }
    }

    private IEnumerable<CountryInfo> FilterCountries( string searchText)
    {
        return _allCountries.Where(country =>
            Contains(country.Name, searchText) ||
            Contains(country.NativeName, searchText) ||
            Contains(country.Iso2, searchText) ||
            Contains(country.Iso3, searchText) ||
            Contains(country.DialCode, searchText));
    }    

    private async Task DetectAndSelectCountryAsync()
    {
        if (DetectionMode == CountryDetectionMode.None)
            return;

        var iso2 = await _countryDetectionService.DetectCountryIso2Async(DetectionMode);

        if (string.IsNullOrWhiteSpace(iso2))
            return;

        var country = _allCountries.FirstOrDefault(item => string.Equals(item.Iso2, iso2, StringComparison.OrdinalIgnoreCase));

        if (country is null)
            return;

        SelectedCountry = country;
        CountriesCollectionView.SelectedItem = country;

        CountrySelected?.Invoke(this, country);

        CountriesCollectionView.ScrollTo(country, position: ScrollToPosition.Center, animate: false);
    }
    #endregion
}