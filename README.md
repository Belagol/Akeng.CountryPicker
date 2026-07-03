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
    DefaultCountryIso2="CM"
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

| Property           | Default           | Description                     |
|--------------------|-------------------|---------------------------------|
| ShowFlags          | True              | Display country flags           |
| ShowDialCode       | True              | Display dialing codes           |
| ShowSearchBar      | True              | Display search bar              |
| Placeholder        | Search country... | Search placeholder              |
| EmptyMessage       | No country found  | Message when no country matches |
| DefaultCountryIso2 | Null              | Default selected country        |

---

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
- [ ] ISO2 / ISO3
- [ ] SVG flags
- [ ] Dark theme
- [ ] Localization
- [ ] Favorite countries
- [ ] Recent countries
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

Made with ❤️ by **Dr Ange Gabriel Belinga**using .NET MAUI