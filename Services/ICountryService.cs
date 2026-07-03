using AkengCountryPicker.Models;

namespace AkengCountryPicker.Services
{
    public interface ICountryService
    {
        Task<List<CountryInfo>> GetCountriesAsync();
        Task<CountryInfo?> GetByIso2Async(string iso2);
        Task<CountryInfo?> GetByIso3Async(string iso3);
        Task<CountryInfo?> GetByDialCodeAsync(string dialCode);
        Task<List<CountryInfo>> SearchAsync(string searchText);
        Task<CountryInfo?> GetCurrentCountryAsync();
    }
}
