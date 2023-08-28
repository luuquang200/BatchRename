using System;
using System.ComponentModel;

namespace Core
{
    public class ItemFile : INotifyPropertyChanged, ICloneable
    {
        public string OldName { get; set; } = "";
        public string NewName { get; set; } = "";
        public string FilePath { get; set; } = "";
        public string Result { get; set; } = "";

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}