using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AkengCountryPicker.Models
{
    public class CountryInfo : INotifyPropertyChanged
    {
        private bool _isFavorite;

        public string Name { get; set; } = string.Empty;
        public string NativeName { get; set; } = string.Empty;
        public string Iso2 { get; set; } = string.Empty;
        public string Iso3 { get; set; } = string.Empty;
        public string DialCode { get; set; } = string.Empty;
        public string FlagEmoji { get; set; } = string.Empty;
        public string FlagImage { get; set; } = string.Empty;

        public string DisplayName => $"{FlagEmoji} {Name} ({DialCode})";

        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                if (_isFavorite == value)
                    return;

                _isFavorite = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FavoriteIcon));
            }
        }

        public string FavoriteIcon => IsFavorite ? "★" : "☆";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
