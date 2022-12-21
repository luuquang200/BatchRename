using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BatchRename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<object> _sourceFiles;
        private ObservableCollection<ItemRule> _listItemRuleApply;
        private readonly List<IRule> _activeRules = new();

        public MainWindow()
        {
            InitializeComponent();
            _sourceFiles = new ObservableCollection<object>();
            _listItemRuleApply = new ObservableCollection<ItemRule>();
        }

        private void AddFileButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog(); // CommonOpenFileDialog

            if (screen.ShowDialog() == true)
            {
                var fullPath = screen.FileName;
                var info = new FileInfo(fullPath);
                var shortName = info.Name;

                _sourceFiles.Add(new
                {
                    FullPath = fullPath,
                    ShortName = shortName,
                });
            }

            ListViewFile.ItemsSource = _sourceFiles;
            //previewListView.ItemsSource = _sourceFiles;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Đăng kí cho biết là mình có thể gặp những luật đổi tên gì?
            // Cảnh giới cuối: Đọc dll, đăng kí khả năng, chọn tập tin luật

            // Cachs 1: Sắp làm: code cứng trước
            //_activeRules.Add(new RemoveSpecialCharsRule()
            //{
            //    SpecialChars = new List<string> { "-", "_" }
            //});
            //_activeRules.Add(new OneSpaceRule());
            //_activeRules.Add(new AddPrefixRule() { Prefix = "Google" });

            // Cach 2: Cac luat duoc luu trong tap tin Preset

            //
            _listItemRuleApply = new ObservableCollection<ItemRule>()
            {
                new ItemRule(){ NameRule = "RemoveSpecialChars"},
                new ItemRule(){ NameRule = "AddCounter"},
                new ItemRule(){ NameRule = "AddPrefix"},
                new ItemRule(){ NameRule = "OneSpace"}
            };

            ListViewRulesApply.ItemsSource = _listItemRuleApply;

            RuleFactory.Register(new RemoveSpecialCharsRule());
            RuleFactory.Register(new AddPrefixRule());
            RuleFactory.Register(new OneSpaceRule());
            RuleFactory.Register(new AddCounterRule());
        }

        //private void loadPresetButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //var screen = new OpenFileDialog();
        //    OpenFileDialog screen = new()
        //    {
        //        Title = "Load preset file",
        //        Filter = "All supported |*.jpg;*.jpeg;*.png;*.pdf;*.txt|" + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "Portable Network Graphic (*.png)|*.png"

        //    };
        //    if (screen.ShowDialog() == true)
        //    {
        //        string presetPath = screen.FileName;

        //        var lines = File.ReadAllLines(presetPath);
        //        RuleFactory factory = new RuleFactory();

        //        foreach (var line in lines)
        //        {
        //            IRule rule = factory.Parse(line);
        //            _activeRules.Add(rule);
        //        }

        //        var converter = (PreviewRenameConverter)FindResource("converter");
        //        converter.Rules = _activeRules;

        //        var temp = new ObservableCollection<object>();

        //        foreach (var file in _sourceFiles)
        //        {
        //            temp.Add(file);
        //        }

        //        _sourceFiles = temp;
        //        //previewListView.ItemsSource = _sourceFiles;
        //    }
        //}

        private void ButtonConfig_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.OriginalSource;
            ListViewRulesApply.SelectedItem = btn.DataContext;

            int indexSelected = ListViewRulesApply.SelectedIndex;
            if (indexSelected != -1)
            {
                ItemRule SelectedItem = _listItemRuleApply[indexSelected];
                if (SelectedItem.NameRule.Equals("RemoveSpecialChars"))
                {
                    MessageBox.Show("RemoveSpecialChars");
                    _listItemRuleApply[indexSelected].Data = "RemoveSpecialChars SpecialChars=-_";
                }
                else if (SelectedItem.NameRule.Equals("AddCounter"))
                {
                    MessageBox.Show(SelectedItem.NameRule);
                    _listItemRuleApply[indexSelected].Data = "AddCounter Start=10,Step=5";
                }
                else if (SelectedItem.NameRule.Equals("OneSpace"))
                {
                    MessageBox.Show(SelectedItem.NameRule);
                    _listItemRuleApply[indexSelected].Data = "OneSpace";
                }
                else if (SelectedItem.NameRule.Equals("AddPrefix"))
                {
                    MessageBox.Show(SelectedItem.NameRule);
                    _listItemRuleApply[indexSelected].Data = "AddPrefix Prefix=Facebook";
                }
            }
        }

        private void List_PreviewLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (GetParentDependencyObjectFromVisualTree((DependencyObject)e.MouseDevice.DirectlyOver,
                                                        typeof(ListViewItem)) is ListViewItem clickedOnItem)
            {
                if (clickedOnItem.IsSelected)
                {
                    return;
                }
                clickedOnItem.IsSelected = true;
                clickedOnItem.Focus();
            }
        }

        private static DependencyObject? GetParentDependencyObjectFromVisualTree(DependencyObject startObject, Type type)
        {
            //Walk the visual tree to get the parent of this control
            DependencyObject parent = startObject;
            while (parent != null)
            {
                if (type.IsInstanceOfType(parent))
                {
                    break;
                }
                else
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }
            }

            return parent;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            _activeRules.Clear();
            if (_listItemRuleApply.Any() && _listItemRuleApply != null)
            {
                foreach (var itemRule in _listItemRuleApply!.Where(itemRule => itemRule.Data != null))
                {
                    var rule = RuleFactory.Instance().Parse(itemRule.Data);
                    if (rule != null)
                    {
                        _activeRules.Add(rule);
                    }
                }
            }

            // binding converter
            var converter = (PreviewRenameConverter)FindResource("PreviewRenameConverter");
            converter.Rules = _activeRules;

            var temp = new ObservableCollection<object>();
            foreach (var file in _sourceFiles)
            {
                temp.Add(file);
            }

            _sourceFiles = temp;
            ListViewFile.ItemsSource = _sourceFiles;
        }

        public static List<string> GetListName(List<IRule> _activeRules)
        {
            List<string> strings = new();
            foreach (IRule rule in _activeRules)
            {
                strings.Add(rule.Name);
            }
            return strings;
        }
    }
}