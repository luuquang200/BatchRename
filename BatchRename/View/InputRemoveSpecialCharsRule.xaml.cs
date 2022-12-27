using System.Windows;

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
