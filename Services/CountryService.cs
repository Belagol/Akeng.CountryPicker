using AkengCountryPicker.Models;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace AkengCountryPicker.Services
{
    public class CountryService : ICountryService
    {
        private List<CountryInfo>? _countries;

        public async Task<List<CountryInfo>> GetCountriesAsync()
        {
            if (_countries != null)
                return _countries;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "AkengCountryPicker.Data.countries.json";

            await using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
                return new List<CountryInfo>();

            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            _countries = JsonSerializer.Deserialize<List<CountryInfo>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<CountryInfo>();

            return _countries;
        }

        public async Task<CountryInfo?> GetByIso2Async(string iso2)
        {
            var countries = await GetCountriesAsync();
            return countries.FirstOrDefault(x =>
                x.Iso2.Equals(iso2, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<CountryInfo?> GetByDialCodeAsync(string dialCode)
        {
            var countries = await GetCountriesAsync();
            return countries.FirstOrDefault(x => x.DialCode == dialCode);
        }

        public async Task<CountryInfo?> GetByIso3Async(string iso3)
        {
            var countries = await GetCountriesAsync();
            return countries.FirstOrDefault(x =>
                x.Iso2.Equals(iso3, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<CountryInfo>> SearchAsync(string searchText)
        {
            var countries = await GetCountriesAsync();

            if (string.IsNullOrWhiteSpace(searchText))
                return countries;

            searchText = searchText.Trim().ToLowerInvariant();

            return countries
                .Where(x =>
                    x.Name.ToLowerInvariant().Contains(searchText) ||
                    x.NativeName.ToLowerInvariant().Contains(searchText) ||
                    x.Iso2.ToLowerInvariant().Contains(searchText) ||
                    x.Iso3.ToLowerInvariant().Contains(searchText) ||
                    x.DialCode.Contains(searchText))
                .ToList();
        }

        public async Task<CountryInfo?> GetCurrentCountryAsync()
        {
            var region = new RegionInfo(CultureInfo.CurrentCulture.Name);
            return await GetByIso2Async(region.TwoLetterISORegionName);
        }
    }
}
