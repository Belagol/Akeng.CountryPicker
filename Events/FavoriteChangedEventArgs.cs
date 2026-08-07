using AkengCountryPicker.Models;

namespace AkengCountryPicker.Events
{
    public sealed class FavoriteChangedEventArgs : EventArgs
    {
        public FavoriteChangedEventArgs(CountryInfo country, bool isFavorite)
        {
            Country = country;
            IsFavorite = isFavorite;
        }

        public CountryInfo Country { get; }

        public bool IsFavorite { get; }
    }
}
