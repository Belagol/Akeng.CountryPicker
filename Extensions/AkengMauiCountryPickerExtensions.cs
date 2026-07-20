using AkengCountryPicker.Services;

namespace AkengCountryPicker.Extensions
{
    public static class AkengMauiCountryPickerExtensions
    {
        public static MauiAppBuilder UseMauiCountryPicker(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<ICountryService, CountryService>();
            builder.Services.AddSingleton<ICountryDetectionService, CountryDetectionService>();
            builder.Services.AddSingleton<ICountryHistoryService, CountryHistoryService>();
            return builder;
        }
    }
}
