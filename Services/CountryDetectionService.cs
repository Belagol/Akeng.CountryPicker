using AkengCountryPicker.Models;
using System.Globalization;

namespace AkengCountryPicker.Services
{
    public sealed class CountryDetectionService : ICountryDetectionService
    {
        public async Task<string?> DetectCountryIso2Async(CountryDetectionMode mode, CancellationToken cancellationToken = default)
        {
            return mode switch
            {
                CountryDetectionMode.None => null,

                CountryDetectionMode.DeviceRegion => DetectFromDeviceRegion(),

                CountryDetectionMode.Geolocation =>  await DetectFromGeolocationAsync(cancellationToken),

                _ => null
            };
        }

        private static string? DetectFromDeviceRegion()
        {
            try
            {
                var culture = CultureInfo.CurrentCulture;

                if (string.IsNullOrWhiteSpace(culture.Name))
                    return null;

                return new RegionInfo(culture.Name).TwoLetterISORegionName
                    .ToUpperInvariant();
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private static async Task<string?> DetectFromGeolocationAsync(CancellationToken cancellationToken)
        {
            try
            {
                var permission = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (permission != PermissionStatus.Granted)
                {
                    permission = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (permission != PermissionStatus.Granted)
                    return null;

                var location =
                    await Geolocation.Default.GetLastKnownLocationAsync();

                location ??= await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)), cancellationToken);

                if (location is null)
                    return null;

                var placemarks = await Geocoding.Default.GetPlacemarksAsync(location.Latitude, location.Longitude);

                return placemarks.FirstOrDefault()?.CountryCode?.ToUpperInvariant();
            }
            catch (FeatureNotSupportedException)
            {
                return null;
            }
            catch (FeatureNotEnabledException)
            {
                return null;
            }
            catch (PermissionException)
            {
                return null;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Country detection failed: {exception}");

                return null;
            }
        }
    }
}
