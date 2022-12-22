using System.ComponentModel;

namespace BatchRename
{
    public class ItemRule : INotifyPropertyChanged
    {
        public string NameRule { get; set; } = "";
        public string Data { get; set; } = "";

        public event PropertyChangedEventHandler PropertyChanged;
    }
}