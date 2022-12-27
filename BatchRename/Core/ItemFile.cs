using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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