using Core;
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
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace BatchRename.View
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        IRule RuleInput;
        private StackPanel stackPanelMain = new();
        private TextBlock textBlockTitleRule = new();
        private TextBlock textBlockNoParam;
        private List<TextBox> textBoxes= new List<TextBox>();

        private StackPanel stackPanelButton = new();
        private Button cancelButton = new();
        private Button okButton = new();
        public ConfigWindow(IRule rule)
        {
            InitializeComponent();
            RuleInput = rule;
            int NumberOfParameter = rule.ListParameter.Count;
          
            stackPanelMain.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanelMain.VerticalAlignment = VerticalAlignment.Center;

            // Title rule:
            textBlockTitleRule.Text = rule.Name;
            textBlockTitleRule.Style = (Style)System.Windows.Application.Current.FindResource("TextblockTitle");
            textBlockTitleRule.FontSize = 25;
            textBlockTitleRule.HorizontalAlignment = HorizontalAlignment.Center;
            textBlockTitleRule.Margin = new Thickness(25, 0, 0, 0);

            stackPanelMain.Children.Add(textBlockTitleRule);
            // Parameter:
            for (int i = 0; i < NumberOfParameter; i++)
            {
                StackPanel stackPanel = new();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                stackPanel.Margin = new Thickness(0, 10, 30, 0);
                // Name of parameter:
                TextBlock textBlock = new();
                textBlock.Width = 135;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.Text = rule.ListParameter.ElementAt(i).Key + ":";
                textBlock.TextAlignment = TextAlignment.Right;
                textBlock.Margin = new Thickness(0, 0, 11, 0);
                textBlock.Style = (Style)System.Windows.Application.Current.FindResource("TextblockTitleSmall");
                
                stackPanel.Children.Add(textBlock);
                // Texbox:
                TextBox textBox = new();
                textBox.Height = 30;
                textBox.Width = 150;
                textBox.VerticalAlignment = VerticalAlignment.Center;
                textBox.Text = rule.ListParameter.ElementAt(i).Value;
                textBox.Padding = new Thickness(5);
                stackPanel.Children.Add(textBox);

                textBoxes.Add(textBox);
                stackPanelMain.Children.Add(stackPanel);
            }
            // StackPanel Button:
            stackPanelButton.Orientation = Orientation.Horizontal;
            stackPanelButton.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanelButton.Margin = new Thickness(0, 20, 0, 0);
            // Cancel button:
            cancelButton.Content = "Cancel";
            cancelButton.Margin = new Thickness(0, 0, 35, 0);
            var bc1 = new BrushConverter();
            cancelButton.Foreground = (Brush)bc1.ConvertFrom("#E1251B");
            cancelButton.Style = (Style)System.Windows.Application.Current.FindResource("ButtonCustomColorCancel");

            if (NumberOfParameter != 0)
            {
                stackPanelButton.Children.Add(cancelButton);
            }
            else
            {
                textBlockNoParam = new();
                textBlockNoParam.Text = "No parameter !";
                textBlockNoParam.Style = (Style)System.Windows.Application.Current.FindResource("TextblockTitleSmall");
                textBlockNoParam.FontSize = 20;
                textBlockNoParam.HorizontalAlignment = HorizontalAlignment.Center;
                textBlockNoParam.Margin = new Thickness(25, 0, 0, 0);

                stackPanelMain.Children.Add(textBlockNoParam);
            }
           
            // Ok button:
            okButton.Content = "Ok";
            okButton.Width = 80;
            okButton.FontSize = 15;
            var bc2 = new BrushConverter();
            okButton.Background = (Brush)bc2.ConvertFrom("#0033A1");
            okButton.Style = (Style)System.Windows.Application.Current.FindResource("ButtonCustomColorPositive");
            okButton.Click += this.OkButtonClick;

            stackPanelButton.Children.Add(okButton);

            stackPanelMain.Children.Add(stackPanelButton);
            this.AddChild(stackPanelMain);
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public string GetData()
        {
            int lenght = RuleInput.ListParameter.Count;
            if (lenght == 0)
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new();
            stringBuilder.Append(RuleInput.Name);
            stringBuilder.Append(' ');
            
            for (int i = 0; i < lenght; i++)
            {
                stringBuilder.Append(RuleInput.ListParameter.ElementAt(i).Key);
                stringBuilder.Append('=');
                stringBuilder.Append(textBoxes[i].Text);
                stringBuilder.Append(',');
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);

            return stringBuilder.ToString();
        }
    }
}
