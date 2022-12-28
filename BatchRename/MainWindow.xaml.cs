using BatchRename.Converters;
using BatchRename.Core;
using BatchRename.Rules;
using Fluent;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Button = System.Windows.Controls.Button;
using FButton = Fluent.Button;

namespace BatchRename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private ObservableCollection<ItemFile> _sourceFiles;
        private readonly ObservableCollection<ItemRule> _availableRules;
        private readonly ObservableCollection<ItemRule> _listItemRuleApply;
        private readonly List<IRule> _activeRules;
        private readonly ObservableCollection<Preset> _presets;
        private Preset _activePreset = null;

        public MainWindow()
        {
            InitializeComponent();

            _sourceFiles = new ObservableCollection<ItemFile>();
            _listItemRuleApply = new ObservableCollection<ItemRule>();
            _availableRules = new ObservableCollection<ItemRule>();
            _activeRules = new List<IRule>();
            _presets = new ObservableCollection<Preset>(Preset.GetPresets());

            DebuggingTest();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Đăng kí cho biết là mình có thể gặp những luật đổi tên gì?
            // Cảnh giới cuối: Đọc dll, đăng kí khả năng, chọn tập tin luật

            // Cách 1: Sắp làm: code cứng trước

            // Cách 2: Cac luat duoc luu trong tap tin Preset

            LoadAvailableRules();
            ComboboxPreset.ItemsSource = _presets;

            ListViewRulesApply.ItemsSource = _listItemRuleApply;
            ComboboxRule.ItemsSource = _availableRules;

            RuleFactory.Register(new RemoveSpecialCharsRule());
            RuleFactory.Register(new AddPrefixRule());
            RuleFactory.Register(new AddSuffixRule());
            RuleFactory.Register(new OneSpaceRule());
            RuleFactory.Register(new AddCounterRule());
            RuleFactory.Register(new RemoveWhiteSpaceRule());
        }

        private void LoadAvailableRules()
        {
            // Temporary hard-coded rules
            _availableRules.Add(new ItemRule() { NameRule = "RemoveSpecialChars" });
            _availableRules.Add(new ItemRule() { NameRule = "AddCounter" });
            _availableRules.Add(new ItemRule() { NameRule = "AddPrefix" });
            _availableRules.Add(new ItemRule() { NameRule = "AddSuffix" });
            _availableRules.Add(new ItemRule() { NameRule = "OneSpace" });
            _availableRules.Add(new ItemRule() { NameRule = "RemoveWhiteSpace" });
        }

        private void ButtonAddFile_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog(); // CommonOpenFileDialog

            if (screen.ShowDialog() == true)
            {
                var fullPath = screen.FileName;
                var info = new FileInfo(fullPath);
                var shortName = info.Name;
                var filePath = info.DirectoryName;
                _sourceFiles.Add(new ItemFile
                {
                    OldName = shortName,
                    NewName = shortName,
                    FilePath = filePath
                });
            }

            ListViewFile.ItemsSource = _sourceFiles;
        }

        private void ButtonConfig_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)e.OriginalSource;
            ListViewRulesApply.SelectedItem = button.DataContext;

            int indexSelected = ListViewRulesApply.SelectedIndex;
            if (indexSelected == -1) return;

            ItemRule SelectedItem = _listItemRuleApply[indexSelected];

            switch (SelectedItem.NameRule)
            {
                case "RemoveSpecialChars":
                    HandleRemoveSpecialCharsConfig(indexSelected);
                    return;

                case "AddCounter":
                    HandleAddCounterConfig(indexSelected);
                    return;

                case "OneSpace":
                    MessageBox.Show(SelectedItem.NameRule);
                    _listItemRuleApply[indexSelected].Data = "OneSpace";
                    return;

                case "AddPrefix":
                    MessageBox.Show(SelectedItem.NameRule);
                    _listItemRuleApply[indexSelected].Data = "AddPrefix Prefix=Facebook";
                    return;

                case "AddSuffix":
                    MessageBox.Show(SelectedItem.NameRule);
                    _listItemRuleApply[indexSelected].Data = "AddSuffix Prefix=hcmus";
                    return;

                case "RemoveWhiteSpace":
                    MessageBox.Show(SelectedItem.NameRule);
                    _listItemRuleApply[indexSelected].Data = "RemoveWhiteSpace";
                    return;

                default:
                    return;
            }
        }

        private void HandleAddCounterConfig(int indexSelected)
        {
            string line = _listItemRuleApply[indexSelected].Data;
            string start = "";
            string step = "";

            if (line != "")
            {
                var tokens = line.Split(' ');
                var data = tokens[1];
                var attributes = data.Split(',');
                var pairs0 = attributes[0].Split('=');
                var pairs1 = attributes[1].Split('=');
                start = pairs0[1];
                step = pairs1[1];
            }

            var screen = new InputAddCounter(start, step);
            if (screen.ShowDialog() == true)
            {
                start = screen.inputStartTextBox.Text;
                step = screen.inputStepTextBox.Text;
                StringBuilder stringBuilder = new();
                stringBuilder.Append("AddCounter Start=");
                stringBuilder.Append(start);
                stringBuilder.Append(",Step=");
                stringBuilder.Append(step);
                _listItemRuleApply[indexSelected].Data = stringBuilder.ToString();
            }
        }

        private void HandleRemoveSpecialCharsConfig(int indexSelected)
        {
            string line = _listItemRuleApply[indexSelected].Data;
            var specials = "";
            if (line != "")
            {
                var tokens = line.Split(' ');
                var data = tokens[1]; // SpecialChars=-_
                var pairs = data.Split('='); // -_
                specials = pairs[1];
            }

            var screen = new InputRemoveSpecialCharsRule(specials);
            if (screen.ShowDialog() == true)
            {
                string input = screen.inputTextBox.Text;
                StringBuilder stringBuilder = new();
                stringBuilder.Append("RemoveSpecialChars SpecialChars=");
                stringBuilder.Append(input);
                _listItemRuleApply[indexSelected].Data = stringBuilder.ToString();
            }
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.OriginalSource;
            ListViewRulesApply.SelectedItem = btn.DataContext;

            int indexSelected = ListViewRulesApply.SelectedIndex;
            if (indexSelected != -1)
            {
                _listItemRuleApply.RemoveAt(indexSelected);
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

        private static DependencyObject GetParentDependencyObjectFromVisualTree(DependencyObject startObject, Type type)
        {
            // Walk the visual tree to get the parent of this control
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

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            _activeRules.Clear();
            if (_listItemRuleApply.Any() && _listItemRuleApply != null)
            {
                foreach (var itemRule in _listItemRuleApply!.Where(itemRule => itemRule.Data != null))
                {
                    IRule rule = RuleFactory.Parse(itemRule.Data);
                    if (rule != null)
                    {
                        _activeRules.Add(rule);
                    }
                }
            }

            // binding converter
            var converter = (PreviewRenameConverter)FindResource("PreviewRenameConverter");
            foreach (IRule rule in _activeRules)
            {
                converter.Rules.Add((IRule)rule.Clone());
            }

            var temp = new ObservableCollection<ItemFile>();
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

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            foreach (ItemFile itemFile in _sourceFiles)
            {
                foreach (IRule itemRule in _activeRules)
                {
                    itemFile.NewName = itemRule.Rename(itemFile.NewName);
                }
                try
                {
                    File.Move(Path.Combine(itemFile.FilePath, itemFile.OldName), Path.Combine(itemFile.FilePath, itemFile.NewName));
                    itemFile.Result = "Success";
                }
                catch (FileNotFoundException)
                {
                    // Unhandled exception
                }
            }
        }

        private void ButtonAddRule_Click(object sender, RoutedEventArgs e)
        {
            var selectedRule = ComboboxRule.SelectedItem as ItemRule;

            _listItemRuleApply.Add(selectedRule);
        }

        private void DebuggingTest()
        {
        }

        private void LoadPreset(Preset preset)
        {
            _activePreset = preset;
            _listItemRuleApply.Clear();

            foreach (var itemRule in _activePreset.GetRuleItems())
            {
                _listItemRuleApply.Add(itemRule);
            }
        }

        private void ComboboxPreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as Fluent.ComboBox;

            LoadPreset(combobox.SelectedItem as Preset);
        }

        private void ButtonNewPreset_Click(object sender, RoutedEventArgs e)
        {
            var newPreset = new Preset();
            _presets.Add(newPreset);

            ComboboxPreset.SelectedItem = newPreset;
        }

        private void ButtonOpenPreset_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = Preset.PRESET_FOLDER_PATH,
                Filter = "txt files (*.txt)|*.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                var filePath = dialog.FileName;

                /* Selected preset is in Presets folder */
                if (Path.GetDirectoryName(filePath).TrimEnd('\\') == Preset.PRESET_FOLDER_PATH.TrimEnd('\\'))
                {
                    var name = Path.GetFileNameWithoutExtension(filePath);
                    ComboboxPreset.SelectedItem = _presets.FirstOrDefault(p => p.Name == name);
                    return;
                }

                /* If not try to copy into Presets folder */
                var presetName = Path.GetFileName(filePath);
                var newPath = Path.Combine(Preset.PRESET_FOLDER_PATH, presetName);

                try
                {
                    File.Copy(filePath, newPath);
                }
                catch (IOException) // Catch exception when preset with the same name already existed in the Presets folder
                {
                    // Ask if want to overwrite
                    var result = MessageBox.Show("Preset with that name already existed, do you want to overwrite?",
                                    "Preset existed",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning);

                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }

                    File.Delete(newPath);
                    // Also delete from the presets list
                    _presets.Remove(_presets.FirstOrDefault(p => p.Name == Path.GetFileNameWithoutExtension(newPath)));

                    File.Copy(filePath, newPath);
                }

                // Add new preset into the preset list
                var preset = new Preset(Path.GetFileNameWithoutExtension(newPath));
                _presets.Add(preset);

                ComboboxPreset.SelectedItem = preset;
            }
        }
    }
}