using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BatchRename
{
    /// <summary>
    /// Interaction logic for InputAddCounter.xaml
    /// </summary>
    public partial class InputAddCounter : Window
    {
        public string Start { get; set; } = "";
        public string Step { get; set; } = "";
        public InputAddCounter(string start, string step)
        {
            InitializeComponent();
            Start = start;
            Step = step;
            DataContext = this;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        { 
            DialogResult = true;
        }
    }
}
