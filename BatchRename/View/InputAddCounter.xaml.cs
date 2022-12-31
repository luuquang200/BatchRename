using BatchRename.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BatchRename
{
    /// <summary>
    /// Interaction logic for InputAddCounter.xaml
    /// </summary>
    public partial class InputAddCounter : Window, IConfigRuleWindow
    {
        public string Start { get; set; } = "";
        public string Step { get; set; } = "";
        public string NumberOfDigits { get; set; } = "";
        public InputAddCounter(string start, string step, string numberOfDigits)
        {
            InitializeComponent();
            Start = start;
            Step = step;
            NumberOfDigits = numberOfDigits;
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

        public string GetData()
        {
            Start = inputStartTextBox.Text;
            Step = inputStepTextBox.Text;
            NumberOfDigits = inputNumberDigitsTextBox.Text;
            StringBuilder stringBuilder = new();
            stringBuilder.Append("AddCounter Start=");
            stringBuilder.Append(Start);
            stringBuilder.Append(",Step=");
            stringBuilder.Append(Step);
            stringBuilder.Append(",Number=");
            stringBuilder.Append(NumberOfDigits);
            return stringBuilder.ToString();
        }
        //private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    e.Cancel = true;
        //    this.Visibility = Visibility.Hidden;
        //}
        bool? IConfigRuleWindow.ShowDialog()
        {
            return this.ShowDialog();
        }
    }
}
