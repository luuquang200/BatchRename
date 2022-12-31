﻿using BatchRename.Converters;
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
using static System.Runtime.InteropServices.JavaScript.JSType;
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
        private readonly ObservableCollection<IRule> _availableRules;
        private ObservableCollection<IRule> _activeRules;
        private ObservableCollection<IRule> _refeshRules;
        private readonly ObservableCollection<Preset> _presets;
        private Preset _activePreset;

        public MainWindow()
        {
            InitializeComponent();

            _sourceFiles = new ObservableCollection<ItemFile>();
            _availableRules = new ObservableCollection<IRule>();
            _activeRules = new ObservableCollection<IRule>();
            _refeshRules = new ObservableCollection<IRule>();
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

            ListViewRulesApply.ItemsSource = _activeRules;
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

            _availableRules.Add(new RemoveSpecialCharsRule());
            _availableRules.Add(new AddPrefixRule());
            _availableRules.Add(new AddSuffixRule());
            _availableRules.Add(new OneSpaceRule());
            _availableRules.Add(new AddCounterRule());
            _availableRules.Add(new RemoveWhiteSpaceRule());
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
            //RefreshStatus();

            int indexSelected = ListViewRulesApply.SelectedIndex;
            if (indexSelected == -1) return;

            var screen = _activeRules[indexSelected].ConfigRuleWindow();

            if (screen.ShowDialog() == true)
            {
               
                string data = screen.GetData();
                MessageBox.Show(data);
                _activeRules[indexSelected].SetData(data);
                //_refeshRules[indexSelected].SetData(data);
            }
              
            UpdateConverterPreview();
        }

        

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)e.OriginalSource;
            ListViewRulesApply.SelectedItem = btn.DataContext;

            RefreshStatus();
            int indexSelected = ListViewRulesApply.SelectedIndex;
            if (indexSelected != -1)
            {
                _activeRules.RemoveAt(indexSelected);
                //_refeshRules.RemoveAt(indexSelected);
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
            ObservableCollection<IRule> _tempRules = new();
            foreach (IRule itemRule in _activeRules)
            {
                _tempRules.Add((IRule)itemRule.Clone());
            }

            foreach (ItemFile itemFile in _sourceFiles)
            {
                //foreach (IRule itemRule in _activeRules)
                foreach (IRule itemRule in _tempRules)
                {
                    if (!itemFile.Result.Equals("Success"))
                    {
                        itemFile.NewName = itemRule.Rename(itemFile.NewName);
                    }
                    
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
            UpdateConverterPreview();
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
            
            RefreshStatus();
        }

        private void ButtonAddRule_Click(object sender, RoutedEventArgs e)
        {
            var selectedRule = ComboboxRule.SelectedItem as IRule;
            _activeRules.Add((IRule)selectedRule.Clone()); 
            //_refeshRules.Add((IRule)selectedRule.Clone());
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
            //foreach (IRule rule in _refeshRules)
            foreach (IRule rule in _activeRules)
            {
                converter.Rules.Add((IRule)rule.Clone());
            }

            ListViewFile.ItemsSource = null;
            ListViewFile.ItemsSource = _sourceFiles;
        }

        private void UpdateActiveRules()
        {
           // _activeRules.Clear();   
           //foreach(IRule rule in _refeshRules)
           //{
           //     _activeRules.Add((IRule)rule.Clone());
           //}
        }

        private void ButtonClearAllFile_Click(object sender, RoutedEventArgs e)
        {
            _sourceFiles.Clear();
        }

        private void LoadPreset(Preset preset)
        {
            _activePreset = preset;
            _activeRules.Clear();
            _refeshRules.Clear();
            foreach (var itemRule in _activePreset.GetRules())
            {
                _activeRules.Add(itemRule);
                _refeshRules.Add(itemRule);
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