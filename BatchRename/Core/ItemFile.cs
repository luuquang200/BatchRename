using System.ComponentModel;

namespace BatchRename.Core
{
    public class ItemFile : INotifyPropertyChanged
    {
        public string OldName { get; set; } = "";
        public string NewName { get; set; } = "";
        public string FilePath { get; set; } = "";
        public string Result { get; set; } = "";

        public event PropertyChangedEventHandler PropertyChanged;
    }
}