using BatchRename.Converters;
using Core;
using BatchRename.View;
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
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Button = System.Windows.Controls.Button;
using FButton = Fluent.Button;
using System.Windows.Media.Media3D;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace BatchRename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly ObservableCollection<ItemFile> _sourceFiles;
        private readonly ObservableCollection<ItemFolder> _sourceFolder;
        private readonly ObservableCollection<IRule> _availableRules;
        private ObservableCollection<IRule> _activeRules;
        private readonly ObservableCollection<Preset> _presets;
        private Preset _activePreset;

        public MainWindow()
        {
            InitializeComponent();

            _sourceFiles = new ObservableCollection<ItemFile>();
            _sourceFolder = new ObservableCollection<ItemFolder>();
            _availableRules = new ObservableCollection<IRule>();
            _activeRules = new ObservableCollection<IRule>();
            _presets = new ObservableCollection<Preset>(Preset.GetPresets());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupFolder();
            LoadAvailableRules();
            ComboboxPreset.ItemsSource = _presets;

            ListViewRulesApply.ItemsSource = _activeRules;
            ComboboxRule.ItemsSource = _availableRules;
            ListViewFile.ItemsSource = _sourceFiles;
            ListViewFolder.ItemsSource = _sourceFolder;
        }

        private void LoadAvailableRules()
        {
            var exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            var dllFiles = new DirectoryInfo(exeFolder).GetFiles("rules/*.dll");
            foreach (var file in dllFiles)
            {
                var assembly = Assembly.LoadFrom(file.FullName);
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsClass && typeof(IRule).IsAssignableFrom(type))
                    {
                        IRule rule = (IRule)Activator.CreateInstance(type)!;
                        RuleFactory.Register(rule);
                        _availableRules.Add(rule);
                    }
                }
            }

        }

        private static void SetupFolder()
        {
            var exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            var rulesPath = Path.Combine(exeFolder, "rules");
            if (!File.Exists(rulesPath))
            {
                CreateFolder(rulesPath);
            }
        }

        public static void CreateFolder(string strPath)
        {
            try
            {
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
            }
            catch { /* Do nothing */ }
        }

        private void ButtonAddFile_Click(object sender, RoutedEventArgs e)
        {
            string[] files = Array.Empty<string>();
            if ((bool)addingRecursively.IsChecked) {

                var screen = new System.Windows.Forms.FolderBrowserDialog();
                if (screen.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string SelectedPath = screen.SelectedPath;
                    files = Directory.GetFiles(SelectedPath);
                    foreach(var file in files)
                    {
                        Debug.WriteLine(file);
                    }
                }
            }
            else
            {
                var screen = new OpenFileDialog
                {
                    Multiselect = true
                };

                if (screen.ShowDialog() == true)
                {
                    files = screen.FileNames;
                }
            }
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

            
            var screen = new ConfigWindow(_activeRules[indexSelected]);
            if(screen.ShowDialog() == true)
            {
                string data = screen.GetData();
                _activeRules[indexSelected].SetData(data);
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
            ObservableCollection<IRule> _tempRules = new();
            foreach (IRule itemRule in _activeRules)
            {
                _tempRules.Add((IRule)itemRule.Clone());
            }
            // 
            string targetFolder = "";
            if ((bool)makeCopy.IsChecked)
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                var result = dialog.ShowDialog();

                if (System.Windows.Forms.DialogResult.OK == result)
                {
                    targetFolder = dialog.SelectedPath;
                }
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
                string validCheck = CheckValid.CheckValidName(itemFile.NewName, true);
                if (validCheck != "")
                {
                    itemFile.Result = "Invalid name: " + validCheck;
                    continue;
                }

                try
                {
                    if((bool)makeCopy.IsChecked)
                        File.Copy(Path.Combine(itemFile.FilePath, itemFile.OldName), Path.Combine(targetFolder, itemFile.NewName));
                    else
                        File.Move(Path.Combine(itemFile.FilePath, itemFile.OldName), Path.Combine(itemFile.FilePath, itemFile.NewName));
                    itemFile.Result = "Success";
                }
                catch (FileNotFoundException)
                {
                    itemFile.Result = "Source file not found";
                    continue;
                }
            }

            _tempRules.Clear();
            foreach (IRule itemRule in _activeRules)
            {
                _tempRules.Add((IRule)itemRule.Clone());
            }

            foreach (ItemFolder itemFolder in _sourceFolder)
            {
                foreach (IRule itemRule in _tempRules)
                {
                    if (!itemFolder.Result.Equals("Success"))
                    {
                        itemFolder.NewName = itemRule.Rename(itemFolder.NewName, false);
                    }
                }
                string validCheck = CheckValid.CheckValidName(itemFolder.NewName, true); ;
                if (validCheck != "")
                {
                    itemFolder.Result = "Invalid name: " + validCheck;
                    continue;
                }

                try
                {
                    Directory.Move(Path.Combine(itemFolder.FolderPath, itemFolder.OldName), Path.Combine(itemFolder.FolderPath, itemFolder.NewName));
                    itemFolder.Result = "Success";
                }
                catch (DirectoryNotFoundException)
                {
                    itemFolder.Result = "Source directory not found";
                    continue;
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
        }

        private void ButtonPreview_Click(object sender, RoutedEventArgs e)
        {
            UpdateConverterPreview();
        }

        private void RefreshStatus()
        {
            
            foreach (ItemFile itemFile in _sourceFiles)
            {
                itemFile.OldName = itemFile.NewName;
                itemFile.Result = "";
            }
            foreach(ItemFolder itemFolder in _sourceFolder)
            {
                itemFolder.OldName = itemFolder.NewName;
                itemFolder.Result = "";
            }
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
            ListViewFile.ItemsSource = null;
            ListViewFile.ItemsSource = _sourceFiles;

            var converter2 = (PreviewRenameConverterFolder)FindResource("PreviewRenameConverterFolder");
            converter2.Rules.Clear(); 
            foreach (IRule rule in _activeRules)
            {
                converter2.Rules.Add((IRule)rule.Clone());
            }
            ListViewFolder.ItemsSource = null;
            ListViewFolder.ItemsSource = _sourceFolder;
        }

        private void ButtonClearAllFile_Click(object sender, RoutedEventArgs e)
        {
            _sourceFiles.Clear();
        }

        private void LoadPreset(Preset preset)
        {
            _activePreset = preset;
            _activeRules.Clear();
            foreach (var itemRule in _activePreset.GetRules())
            {
                _activeRules.Add((IRule)itemRule.Clone());
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

        private void ButtonClearAllRule_Click(object sender, RoutedEventArgs e)
        {
            _activeRules.Clear();
            UpdateConverterPreview();
        }

        private void OnTop_Click(object sender, RoutedEventArgs e)
        {
            int index = ListViewRulesApply.SelectedIndex;
            if (index != -1)
            {
                IRule temp = _activeRules[index];
                for (int i = index; i > 0; --i)
                    _activeRules[i] = _activeRules[i - 1];
                _activeRules[0] = temp;
            }
        }

        private void OnBottom_Click(object sender, RoutedEventArgs e)
        {
            int index = ListViewRulesApply.SelectedIndex;
            if (index != -1)
            {
                IRule temp = _activeRules[index];
                for (int i = index; i < _activeRules.Count - 1; ++i)
                    _activeRules[i] = _activeRules[i + 1];
                _activeRules[_activeRules.Count - 1] = temp;
            }   
        }

        private void OnNext_Click(object sender, RoutedEventArgs e)
        {
            int index = ListViewRulesApply.SelectedIndex;
            if (index != -1 && index != _activeRules.Count - 1)
            {
                IRule temp = _activeRules[index + 1];
                _activeRules[index + 1] = _activeRules[index];
                _activeRules[index] = temp;
            }
        }

        private void OnPreview_Click(object sender, RoutedEventArgs e)
        {
            int index = ListViewRulesApply.SelectedIndex;
            if (index != -1 && index != 0)
            {
                IRule temp = _activeRules[index - 1];
                _activeRules[index - 1] = _activeRules[index];
                _activeRules[index] = temp;
            }
        }

        private void ButtonSavePreset_Click(object sender, RoutedEventArgs e)
        {
            // If no preset is opening, ask user to enter new name for the preset
            if (_activePreset is null)
            {
                return;
            }

            // Else save to the opening preset
            using StreamWriter presetFile = new(_activePreset.GetPath());
            foreach (var rule in _activeRules)
            {
                // Not yet implemented
                int lenght = rule.ListParameter.Count;
                StringBuilder stringBuilder = new();
                stringBuilder.Append(rule.Name);
                stringBuilder.Append(' ');

                for (int i = 0; i < lenght; i++)
                {
                    stringBuilder.Append(rule.ListParameter.ElementAt(i).Key);
                    stringBuilder.Append('=');
                    stringBuilder.Append(rule.ListParameter.ElementAt(i).Value);
                    stringBuilder.Append(',');
                }
                stringBuilder.Remove(stringBuilder.Length - 1, 1);

                presetFile.WriteLine(stringBuilder.ToString());

            }
        }

        private void ButtonDeletePreset_Click(object sender, RoutedEventArgs e)
        {
            if (_activePreset is null)
            {
                //_listItemRuleApply.Clear();
                _activeRules.Clear();
                return;
            }

            var result = MessageBox.Show("Do you want to delete this preset?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;

            File.Delete(_activePreset.GetPath());
            _presets.Remove(_activePreset); // ComboBoxSelectionChanged will automatically called
        }

        private void ButtonSavePresetAlternative_Click(object sender, RoutedEventArgs e)
        {
            // Haven't implemented to handle saving preset's content

            if (_activePreset is null)
                return;

            string newName = TextboxRename.Text;
            if (string.IsNullOrEmpty(newName))
            {
                MessageBox.Show("Preset name cannot be empty.", "Invalid name", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string oldName = _activePreset.Name;
            if (_activePreset.Rename(newName))
            {
                File.Move(Preset.GetPath(oldName), Preset.GetPath(newName));
                ComboboxPreset.Text = newName;
            }
        }

        private void ButtonAddFolder_Click(object sender, RoutedEventArgs e)
        {
            var screen = new System.Windows.Forms.FolderBrowserDialog();
            if (screen.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string SelectedPath = screen.SelectedPath;
                string[] folders = Directory.GetDirectories(SelectedPath);
                //Directory.GetFiles(path)   
                foreach (var folder in folders)
                {
                    string shortName = folder.Remove(0, SelectedPath.Length + 1);
                    Debug.WriteLine(SelectedPath);
                    bool isExisted = false;

                    foreach (var itemFolder in _sourceFolder)
                    {
                        if (itemFolder.OldName.Equals(shortName) && itemFolder.FolderPath.Equals(SelectedPath))
                        {
                            isExisted = true;
                            break;
                        }
                    }

                    if (!isExisted)
                    {
                        _sourceFolder.Add(new ItemFolder
                        {
                            OldName = shortName,
                            NewName = shortName,
                            FolderPath = SelectedPath
                        });
                    }
                }
            }
           
        }

        private void ButtonClearAllFolder_Click(object sender, RoutedEventArgs e)
        {
            _sourceFolder.Clear();
        }
    }
}