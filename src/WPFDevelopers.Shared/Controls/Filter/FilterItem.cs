using System.ComponentModel;

namespace WPFDevelopers.Controls
{
    public class FilterItem : INotifyPropertyChanged
    {
        private bool _isChecked = true;
        public object Value { get; set; }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this,
                        new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
