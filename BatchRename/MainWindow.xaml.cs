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
using System.Diagnostics;
using System.Diagnostics.Metrics;
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
        private readonly ObservableCollection<ItemFile> _sourceFiles;
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
            ListViewFile.ItemsSource = _sourceFiles;

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
            var screen = new OpenFileDialog
            {
                Multiselect = true
            };

            if (screen.ShowDialog() == true)
            {
                string[] files = screen.FileNames;

                foreach (string file in files)
                {
                    bool isExisted = false;
                    var info = new FileInfo(file);
                    var shortName = info.Name;
                    var filePath = info.DirectoryName;

                    foreach (var itemFile in _sourceFiles)
                    {
                        if (itemFile.OldName.Equals(shortName) && itemFile.FilePath.Equals(filePath))
                        {
                            isExisted = true;
                            break;
                        }
                    }

                    if (!isExisted)
                    {
                        _sourceFiles.Add(new ItemFile
                        {
                            OldName = shortName,
                            NewName = shortName,
                            FilePath = filePath
                        });
                    }
                }
            }
            ListViewFile.ItemsSource = _sourceFiles;
        }

        private void ButtonConfig_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)e.OriginalSource;
            ListViewRulesApply.SelectedItem = button.DataContext;

            //11:15
            RefreshStatus();

            int indexSelected = ListViewRulesApply.SelectedIndex;
            if (indexSelected == -1) return;

            ItemRule SelectedItem = _listItemRuleApply[indexSelected];

            switch (SelectedItem.NameRule)
            {
                case "RemoveSpecialChars":
                    HandleRemoveSpecialCharsConfig(indexSelected);
                    break;

                case "AddCounter":
                    HandleAddCounterConfig(indexSelected);
                    break;

                case "OneSpace":
                    MessageBox.Show(SelectedItem.NameRule);
                    _listItemRuleApply[indexSelected].Data = "OneSpace";
                    break;

                case "AddPrefix":
                    MessageBox.Show(SelectedItem.NameRule);
                    _listItemRuleApply[indexSelected].Data = "AddPrefix Prefix=Facebook";
                    break;

                case "AddSuffix":
                    MessageBox.Show(SelectedItem.NameRule);
                    _listItemRuleApply[indexSelected].Data = "AddSuffix Prefix=hcmus";
                    break;

                case "RemoveWhiteSpace":
                    MessageBox.Show(SelectedItem.NameRule);
                    _listItemRuleApply[indexSelected].Data = "RemoveWhiteSpace";
                    break;

                default:
                    break;
            }

            UpdateActiveRules();
            UpdateConverterPreview();
        }

        private void HandleAddCounterConfig(int indexSelected)
        {
            string line = _listItemRuleApply[indexSelected].Data;
            string start = "";
            string step = "";
            string number = "";
            if (line != "")
            {
                var tokens = line.Split(new string[] { " " },
                    StringSplitOptions.None);
                var data = tokens[1];
                var attributes = data.Split(new string[] { "," },
                    StringSplitOptions.None);
                var pairs0 = attributes[0].Split(new string[] { "=" },
                    StringSplitOptions.None);
                var pairs1 = attributes[1].Split(new string[] { "=" },
                    StringSplitOptions.None);
                var pairs2 = attributes[2].Split(new string[] { "=" },
                    StringSplitOptions.None);
                start = pairs0[1];
                step = pairs1[1];
                number = pairs2[1];
            }

            var screen = new InputAddCounter(start, step, number);
            if (screen.ShowDialog() == true)
            {
                start = screen.inputStartTextBox.Text;
                step = screen.inputStepTextBox.Text;
                number = screen.inputNumberDigitsTextBox.Text;
                StringBuilder stringBuilder = new();
                stringBuilder.Append("AddCounter Start=");
                stringBuilder.Append(start);
                stringBuilder.Append(",Step=");
                stringBuilder.Append(step);
                stringBuilder.Append(",Number=");
                stringBuilder.Append(number);
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

            RefreshStatus();
            int indexSelected = ListViewRulesApply.SelectedIndex;
            if (indexSelected != -1)
            {
                _listItemRuleApply.RemoveAt(indexSelected);
            }

            UpdateActiveRules();
            UpdateConverterPreview();
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
            UpdateActiveRules();
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
            //UpdateConverterPreview();
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

        //funcion for test
        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            //UpdateActiveRules();
            //foreach (ItemFile itemFile in _sourceFiles)
            //{
            //    foreach (IRule itemRule in _activeRules)
            //    {
            //        itemFile.NewName = itemRule.Rename(itemFile.NewName);
            //    }
            //    try
            //    {
            //        File.Move(Path.Combine(itemFile.FilePath, itemFile.OldName), Path.Combine(itemFile.FilePath, itemFile.NewName));
            //        itemFile.Result = "Success";
            //    }
            //    catch (FileNotFoundException)
            //    {
            //        // Unhandled exception
            //    }
            //}
            RefreshStatus();
        }

        private void ButtonAddRule_Click(object sender, RoutedEventArgs e)
        {
            var selectedRule = ComboboxRule.SelectedItem as ItemRule;
            if (!_listItemRuleApply.Contains(selectedRule))
            {
                _listItemRuleApply.Add(selectedRule);
            }
        }

        private void ButtonPreview_Click(object sender, RoutedEventArgs e)
        {
            UpdateActiveRules();
            UpdateConverterPreview();
        }

        private void RefreshStatus()
        {
            foreach (ItemFile itemFile in _sourceFiles)
            {
                itemFile.OldName = itemFile.NewName;
                itemFile.Result = "";
            }
            UpdateActiveRules();
            UpdateConverterPreview();
        }

        private void UpdateConverterPreview()
        {
            // binding converter
            var converter = (PreviewRenameConverter)FindResource("PreviewRenameConverter");
            converter.Rules.Clear();
            foreach (IRule rule in _activeRules)
            {
                converter.Rules.Add((IRule)rule.Clone());
            }
            var temp = new ObservableCollection<ItemFile>();
            //foreach (var file in _sourceFiles)
            //{
            //    temp.Add( (ItemFile)file.Clone() );
            //}
            //_sourceFiles = temp;
            ListViewFile.ItemsSource = null;
            ListViewFile.ItemsSource = _sourceFiles;
        }

        private void UpdateActiveRules()
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
        }

        private void ButtonClearAllFile_Click(object sender, RoutedEventArgs e)
        {
            _sourceFiles.Clear();
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

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshStatus();
        }

        private void ListViewFile_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                //MessageBox.Show("File: " + files[0]);
                foreach (string file in files)
                {
                    bool isExisted = false;
                    var info = new FileInfo(file);
                    var shortName = info.Name;
                    var filePath = info.DirectoryName;

                    foreach (var itemFile in _sourceFiles)
                    {
                        if (itemFile.OldName.Equals(shortName) && itemFile.FilePath.Equals(filePath))
                        {
                            isExisted = true;
                            break;
                        }
                    }

                    if (!isExisted)
                    {
                        _sourceFiles.Add(new ItemFile
                        {
                            OldName = shortName,
                            NewName = shortName,
                            FilePath = filePath
                        });
                    }
                }
            }
        }
    }
}