using System.Text.Json;

namespace AkengCountryPicker.Services
{
    public sealed class CountryHistoryService : ICountryHistoryService
    {
        private const string FavoritesKey = "Akeng.CountryPicker.Favorites";

        private const string RecentsKey = "Akeng.CountryPicker.Recents";

        private readonly IPreferences _preferences;

        public CountryHistoryService() : this(Preferences.Default)
        {
        }

        public CountryHistoryService(IPreferences preferences)
        {
            _preferences = preferences;
        }

        public IReadOnlyList<string> GetFavoriteIso2Codes()
        {
            return ReadCodes(FavoritesKey);
        }

        public void SetFavoriteIso2Codes(IEnumerable<string> iso2Codes)
        {
            SaveCodes(FavoritesKey, iso2Codes);
        }

        public IReadOnlyList<string> GetRecentIso2Codes()
        {
            return ReadCodes(RecentsKey);
        }

        public void SetRecentIso2Codes(IEnumerable<string> iso2Codes)
        {
            SaveCodes(RecentsKey, iso2Codes);
        }

        public void ClearRecentCountries()
        {
            _preferences.Remove(RecentsKey);
        }

        private IReadOnlyList<string> ReadCodes(string key)
        {
            var json = _preferences.Get(key, "[]");

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json)
                           ?.Where(code => !string.IsNullOrWhiteSpace(code))
                           .Select(NormalizeIso2)
                           .Distinct(StringComparer.OrdinalIgnoreCase)
                           .ToArray()
                       ?? Array.Empty<string>();
            }
            catch (JsonException)
            {
                _preferences.Remove(key);
                return Array.Empty<string>();
            }
        }

        private void SaveCodes(string key, IEnumerable<string> iso2Codes)
        {
            var codes = iso2Codes
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Select(NormalizeIso2)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var json = JsonSerializer.Serialize(codes);

            _preferences.Set(key, json);
        }

        private static string NormalizeIso2(string iso2)
        {
            return iso2.Trim().ToUpperInvariant();
        }
    }
}
