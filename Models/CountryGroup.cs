using System.Collections.ObjectModel;

namespace AkengCountryPicker.Models
{
    public sealed class CountryGroup : ObservableCollection<CountryInfo>
    {
        public CountryGroup(
            string key,
            string title,
            IEnumerable<CountryInfo> countries)
            : base(countries)
        {
            Key = key;
            Title = title;
        }

        public string Key { get; }

        public string Title { get; }
    }
}
