# Akeng.CountryPicker

A lightweight and customizable **.NET MAUI Country Picker** with support for:

- 🌍 240+ countries
- 📞 International dialing codes
- 🏳️ Country emojis
- 🏳️ Country flags
- 🔍 Built-in search
- 🎯 Ready-to-use MAUI picker
- 🧩 Country service for custom UI
- 📱 Android, iOS, Windows and MacCatalyst

---

## Features

- ✅ 240+ countries
- ✅ ISO 3166-1 Alpha-2 & Alpha-3 codes
- ✅ International dialing codes
- ✅ Built-in searchable picker
- ✅ Customizable UI
- ✅ Dependency Injection support
- ✅ Works on all .NET MAUI platforms
- 🌍 Automatic country detection using the device's location (optional)
- 🌐 Automatic country detection using the device's region settings
- 🔒 No location permission required unless Geolocation mode is enabled

---

# Installation

Install from NuGet

```bash
dotnet add package Akeng.CountryPicker
```

or using the NuGet Package Manager.

---

# Register the service

In `MauiProgram.cs`

```csharp
using Akeng.CountryPicker.Extensions;

builder
    .UseMauiApp<App>()
    .UseAkengCountryPicker();
```

---

# Option 1 - Use the Country Service

Inject the service:

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

Available methods

```csharp
Task<List<CountryInfo>> GetCountriesAsync();

Task<List<CountryInfo>> SearchAsync(string text);

Task<CountryInfo?> GetByIso2Async(string iso2);

Task<CountryInfo?> GetByIso3Async(string iso3);

Task<CountryInfo?> GetByDialCodeAsync(string dialCode);

Task<CountryInfo?> GetCurrentCountryAsync();
```

---

# Option 2 - Use the built-in Country Picker

XAML

```xml
<countryPicker:CountryPickerView
    SelectedCountry="{Binding SelectedCountry}"
    ShowFlags="True"
    ShowDialCode="True"
    ShowSearchBar="True"/>
```

ViewModel

```csharp
public CountryInfo? SelectedCountry
{
    get => _selectedCountry;
    set => SetProperty(ref _selectedCountry, value);
}
```

---

# Customization

| Property               | Default             | Description                     |
|------------------------|---------------------|---------------------------------|
| ShowEmoji              | True                | Display country flags emoji     |
| ShowDialCode           | True                | Display dialing codes           |
| ShowSearchBar          | True                | Display search bar              |
| Placeholder            | Search country...   | Search placeholder              |
| EmptyMessage           | No country found    | Message when no country matches |
| ShowNativeName         | True                | Display native country name     |
| ShowIso2               | False               | Display country iso2 code       |
| ShowIso3               | False               | Display country iso3 code       |
| DetectionMode          | None                | Country mode dectection         |
| ShowFavorites          | True                | Show favorites countries        |
| ShowRecentCountries    | True                | Show recent selected countries  |
| AllowFavoriteSelection | True                | Allow favorites selection       |
| MaxRecentCountries     | 5                   | Set maximum recent countries    |

---

## 🌍 Automatic Country Detection (Geolocation)

`CountryPickerView` can automatically select the user's current country based on the device's geographic location.

```xml
<countryPicker:CountryPickerView
    DetectionMode="Geolocation" />
```

### Platform Permissions

When using `DetectionMode="Geolocation"`, your application **must** request location permissions.

### Android

Add the following permissions to your **Platforms/Android/AndroidManifest.xml**:

```xml
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```

### iOS / Mac Catalyst

Add the following key to your **Platforms/iOS/Info.plist** (and **Platforms/MacCatalyst/Info.plist** if applicable):

```xml
<key>NSLocationWhenInUseUsageDescription</key>
<string>This application uses your location to automatically select your current country.</string>
```

### Requesting Permission

The Country Picker requests location permission only when `DetectionMode` is set to `Geolocation`.

If the user denies the permission or the location cannot be determined, the picker simply loads the country list without selecting a default country.

> **Note**
> The package does **not** force location access. Applications that do not use `DetectionMode="Geolocation"` do not need to declare any location permissions.


# Country Selected Event

```xml
<countryPicker:CountryPickerView
    CountrySelected="OnCountrySelected"/>
```

```csharp
private void OnCountrySelected(object sender, CountryInfo country)
{
    Debug.WriteLine(country.Name);
}
```

---

# Country Model

```csharp
public class CountryInfo
{
    public string Name { get; set; }

    public string NativeName { get; set; }

    public string Iso2 { get; set; }

    public string Iso3 { get; set; }

    public string DialCode { get; set; }

    public string FlagEmoji { get; set; }

    public string FlagImage { get; set; }
     
    public string DisplayName => $"{FlagEmoji} {Name} ({DialCode})";
}
```

---

# Screenshots

Coming soon.

---

# Roadmap

- [x] Country service
- [x] Country picker
- [x] Search
- [x] Dialing codes
- [x] ISO2 / ISO3
- [ ] SVG flags
- [ ] Dark theme
- [x] Localization
- [x] Geolocation
- [x] Favorite countries
- [x] Recent countries
- [ ] Country templates
- [ ] PhoneEntry control

---

## 🤝 Contributing

Contributions are welcome!

You can contribute by:

- Reporting bugs
- Suggesting new features
- Improving documentation
- Submitting pull requests

Repository: https://github.com/Belagol/Akeng.CountryPicker

Issues: https://github.com/Belagol/Akeng.CountryPicker/issues

Pull Requests: https://github.com/Belagol/Akeng.CountryPicker/pulls

---

# License

MIT License

---

Made with ❤️ by **Dr. Ange Gabriel Belinga** using .NET MAUI