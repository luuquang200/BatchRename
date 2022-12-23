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
    /// Interaction logic for InputRemoveSpecialCharsRule.xaml
    /// </summary>
    public partial class InputRemoveSpecialCharsRule : Window
    {
        public string Data { get; set; } = "";
        public InputRemoveSpecialCharsRule(string specials)
        {
            InitializeComponent();
            DataContext = this;
            Data = specials;
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

        }
    }
}
