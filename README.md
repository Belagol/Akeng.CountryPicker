# Akeng.CountryPicker

A lightweight and customizable **.NET MAUI Country Picker** for Android, iOS, Windows and MacCatalyst.

## ✨ Features

- 🌍 240+ countries
- 📞 International dialing codes
- 🏳️ Country emoji / flag support
- 🔍 Fast built-in search
- ⭐ Favorite countries
- 🕘 Recent countries
- 🌐 Device region detection
- 📍 Optional geolocation-based country detection
- 🎨 Light, Dark and System themes
- 🧩 Built-in item templates
- 🎛️ Fully custom `DataTemplate`
- 🔄 Two-way selected country binding
- ⚡ MVVM command support
- 🧩 `ICountryService` for fully custom UIs
- 📱 Android, iOS, Windows and MacCatalyst

---

# Installation

```bash
dotnet add package Akeng.CountryPicker
```

Register the package in `MauiProgram.cs`:

```csharp
using Akeng.CountryPicker.Extensions;

builder
    .UseMauiApp<App>()
    .UseAkengCountryPicker();
```

---

# Usage

## Option 1 — Use `ICountryService`

```csharp
public class HomeViewModel
{
    private readonly ICountryService _countryService;

    public HomeViewModel(ICountryService countryService)
    {
        _countryService = countryService;
    }

    public async Task LoadAsync()
    {
        var countries = await _countryService.GetCountriesAsync();
    }
}
```

Available methods:

```csharp
Task<List<CountryInfo>> GetCountriesAsync();
Task<List<CountryInfo>> SearchAsync(string text);
Task<CountryInfo?> GetByIso2Async(string iso2);
Task<CountryInfo?> GetByIso3Async(string iso3);
Task<CountryInfo?> GetByDialCodeAsync(string dialCode);
Task<CountryInfo?> GetCurrentCountryAsync();
```

---

## Option 2 — Use `CountryPickerView`

```xml
xmlns:countryPicker="clr-namespace:AkengCountryPicker.Controls;assembly=AkengCountryPicker"

<countryPicker:CountryPickerView
    SelectedCountry="{Binding SelectedCountry}" />
```

ViewModel:

```csharp
public CountryInfo? SelectedCountry
{
    get => _selectedCountry;
    set => SetProperty(ref _selectedCountry, value);
}
```

---

# Customization

| Property | Default | Description |
|---|---:|---|
| `ShowEmoji` | `True` | Show country emoji |
| `ShowDialCode` | `True` | Show dialing code |
| `ShowSearchBar` | `True` | Show search bar |
| `ShowNativeName` | `True` | Show native country name |
| `ShowIso2` | `False` | Show ISO2 |
| `ShowIso3` | `False` | Show ISO3 |
| `ShowFavorites` | `True` | Show favorites group |
| `ShowRecentCountries` | `True` | Show recent countries |
| `AllowFavoriteSelection` | `True` | Allow favorite toggle |
| `MaxRecentCountries` | `5` | Maximum recent countries |
| `DetectionMode` | `None` | Country detection mode |
| `Theme` | `System` | Picker theme |
| `TemplateMode` | `Default` | Built-in item layout |
| `Placeholder` | `Search country...` | Search placeholder |
| `EmptyMessage` | `No country found` | Empty result message |

---

# Country Selection

Event-based:

```xml
<countryPicker:CountryPickerView
    CountrySelected="OnCountrySelected" />
```

```csharp
private void OnCountrySelected(object sender, CountryInfo country)
{
    Debug.WriteLine(country.Name);
}
```

MVVM command:

```xml
<countryPicker:CountryPickerView
    SelectedCountry="{Binding SelectedCountry}"
    CountrySelectedCommand="{Binding CountrySelectedCommand}" />
```

---

# 🌍 Automatic Country Detection

```xml
<countryPicker:CountryPickerView
    DetectionMode="Geolocation" />
```

Available modes:

```csharp
None
DeviceRegion
Geolocation
```

## Android permissions

Add to `Platforms/Android/AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```

## iOS / MacCatalyst

Add to `Info.plist`:

```xml
<key>NSLocationWhenInUseUsageDescription</key>
<string>This application uses your location to automatically select your current country.</string>
```

> Geolocation is optional. No location permission is required unless `DetectionMode="Geolocation"` is used.

---

# 🎨 Themes

Available themes:

```csharp
System
Light
Dark
```

```xml
<countryPicker:CountryPickerView
    Theme="Dark" />
```

Change at runtime:

```csharp
countryPicker.Theme = CountryPickerTheme.Dark;
countryPicker.Theme = CountryPickerTheme.Light;
countryPicker.Theme = CountryPickerTheme.System;
```

The picker theme only affects `CountryPickerView`, not the entire application.

---

# 🧩 Built-in Templates

Available template modes:

- `Default`
- `Compact`
- `Phone`
- `Iso`
- `NativeName`
- `FlagOnly`

```xml
<countryPicker:CountryPickerView
    TemplateMode="Phone" />
```

Change at runtime:

```csharp
countryPicker.TemplateMode = CountryItemTemplateMode.Compact;
```

---

# 🎛️ Custom Country Template

You can completely replace the built-in item layout.

```xml
xmlns:countryModels="clr-namespace:AkengCountryPicker.Models;assembly=AkengCountryPicker"

<countryPicker:CountryPickerView>

    <countryPicker:CountryPickerView.CountryTemplate>

        <DataTemplate x:DataType="countryModels:CountryInfo">

            <Grid
                Padding="12"
                ColumnDefinitions="Auto,*,Auto">

                <Label
                    FontSize="26"
                    Text="{Binding FlagEmoji}" />

                <VerticalStackLayout
                    Grid.Column="1"
                    Margin="12,0">

                    <Label
                        FontAttributes="Bold"
                        Text="{Binding Name}" />

                    <Label
                        FontSize="12"
                        Text="{Binding DialCode}" />

                </VerticalStackLayout>

            </Grid>

        </DataTemplate>

    </countryPicker:CountryPickerView.CountryTemplate>

</countryPicker:CountryPickerView>
```

A custom `CountryTemplate` takes priority over `TemplateMode`.

The picker still keeps its built-in search, selection, recent countries and favorites logic.

---

# ⭐ Favorites & Recent Countries

The picker automatically:

- persists favorite countries
- remembers recently selected countries
- avoids duplicates
- limits recent countries with `MaxRecentCountries`
- updates the groups in real time

Favorites and recent countries are stored locally using country ISO2 codes.

---

# Country Model

```csharp
public class CountryInfo
{
    public string Name { get; set; } = string.Empty;
    public string NativeName { get; set; } = string.Empty;
    public string Iso2 { get; set; } = string.Empty;
    public string Iso3 { get; set; } = string.Empty;
    public string DialCode { get; set; } = string.Empty;
    public string FlagEmoji { get; set; } = string.Empty;

    public bool IsFavorite { get; set; }

    public string FavoriteIcon =>
        IsFavorite ? "★" : "☆";
}
```

---

# Roadmap

- [x] Country service
- [x] Country picker
- [x] Search
- [x] Dial codes
- [x] ISO2 / ISO3
- [x] Favorites
- [x] Recent countries
- [x] Geolocation
- [x] Device region detection
- [x] Light / Dark / System themes
- [x] Built-in templates
- [x] Custom `CountryTemplate`
- [x] MVVM command support
- [ ] Image/SVG flag mode
- [ ] Advanced localization
- [ ] PhoneEntry control

---

## Supported Frameworks

| Framework | Status |
|---|---|
| .NET 10 MAUI | ✅ Supported |
| .NET 9 MAUI | ✅ Supported |
| .NET 8 MAUI | ⚠️ Legacy / no longer actively supported |

---

# 🤝 Contributing

Contributions are welcome.

- Repository: https://github.com/Belagol/Akeng.CountryPicker
- Issues: https://github.com/Belagol/Akeng.CountryPicker/issues
- Pull Requests: https://github.com/Belagol/Akeng.CountryPicker/pulls

---

# License

MIT License

---

Made with ❤️ by **Dr. Ange Gabriel Belinga** using .NET MAUI