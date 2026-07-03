namespace AkengCountryPicker.Models
{
    public class CountryInfo
    {
        public string Name { get; set; } = string.Empty;
        public string NativeName { get; set; } = string.Empty;
        public string Iso2 { get; set; } = string.Empty;
        public string Iso3 { get; set; } = string.Empty;
        public string DialCode { get; set; } = string.Empty;
        public string FlagEmoji { get; set; } = string.Empty;
        public string FlagImage { get; set; } = string.Empty;

        public string DisplayName => $"{FlagEmoji} {Name} ({DialCode})";
    }
}
