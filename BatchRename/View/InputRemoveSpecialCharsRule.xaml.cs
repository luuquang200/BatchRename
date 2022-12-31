using BatchRename.Core;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace BatchRename
{
    /// <summary>
    /// Interaction logic for InputRemoveSpecialCharsRule.xaml
    /// </summary>
    public partial class InputRemoveSpecialCharsRule : Window, IConfigRuleWindow
    {
        public string Data { get; set; } 
        public InputRemoveSpecialCharsRule(List<string> specials)
        {
            InitializeComponent();
            DataContext = this;

            StringBuilder stringBuilder= new StringBuilder();
            foreach (var c in specials)
            {
                stringBuilder.Append(c);
            }
            Data = stringBuilder.ToString();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

        }

        public string GetData()
        {
            string input = inputTextBox.Text;
            StringBuilder stringBuilder = new();
            stringBuilder.Append("RemoveSpecialChars SpecialChars=");
            stringBuilder.Append(input);
            return stringBuilder.ToString();

        }
    }
}
