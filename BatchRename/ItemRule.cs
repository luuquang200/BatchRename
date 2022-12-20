using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename
{
    public class ItemRule : INotifyPropertyChanged
    {
        public string NameRule { get; set; } = "";
        public string Data { get; set; } = "";

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
