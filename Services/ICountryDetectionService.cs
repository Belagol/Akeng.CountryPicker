using AkengCountryPicker.Models;

namespace AkengCountryPicker.Services
{
    public interface ICountryDetectionService
    {
        Task<string?> DetectCountryIso2Async(CountryDetectionMode mode, CancellationToken cancellationToken = default);
    }
}
