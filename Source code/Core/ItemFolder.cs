using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ItemFolder : INotifyPropertyChanged, ICloneable
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public string OldName { get; set; } = "";
        public string NewName { get; set; } = "";
        public string FolderPath { get; set; } = "";
        public string Result { get; set; } = "";
        public object Clone() 
        {
            return MemberwiseClone();
        }
    }
}
