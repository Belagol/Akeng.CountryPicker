namespace AkengCountryPicker.Services
{
    public interface ICountryHistoryService
    {
        IReadOnlyList<string> GetFavoriteIso2Codes();

        void SetFavoriteIso2Codes(IEnumerable<string> iso2Codes);

        IReadOnlyList<string> GetRecentIso2Codes();

        void SetRecentIso2Codes(IEnumerable<string> iso2Codes);

        void ClearRecentCountries();
    }
}
