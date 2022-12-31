using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Documents;

namespace BatchRename.Core
{
    public class Preset : ObservableObject
    {
        public static string PRESET_FOLDER_PATH => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")) + @"Presets\";
        public static readonly string PRESET_FILE_EXTENSION = ".txt";

        public string Name { get; private set; }

        public Preset()
        {
            // Create preset named Untitled, add counter if file exists
            string defaultName = "Untitled";
            string name = defaultName;
            int counter = 0;

            while (Exists(name))
            {
                counter++;
                name = defaultName + counter.ToString();
            }

            Name = name;
            File.Create(GetPath()).Close();
        }

        public Preset(string name)
        {
            Name = name;

            if (!Exists())
            {
                File.Create(GetPath()).Close();
            }
        }

        // Returns true if rename successfully, other wise returns false
        public bool Rename(string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                throw new ArgumentException("Preset name cannot be null or empty.");
            }

            if (Exists(newName))
                return false;

            Name = newName;
            NotifyPropertyChanged(nameof(Name));
            return true;
        }

        public string GetPath()
        {
            return Path.Combine(PRESET_FOLDER_PATH, Name + PRESET_FILE_EXTENSION);
        }

        public static string GetPath(string name)
        {
            return Path.Combine(PRESET_FOLDER_PATH, name + PRESET_FILE_EXTENSION);
        }

        public bool Exists(string name = null)
        {
            return name is null ? File.Exists(GetPath()) : File.Exists(GetPath(name));
        }

        public string[] Read()
        {
            return File.ReadAllLines(GetPath());
        }

        public List<ItemRule> GetRuleItems()
        {
            var rules = new List<ItemRule>();

            var lines = Read();

            const char Space = ' ';

            foreach (var line in lines)
            {
                string ruleName = line.Split(Space)[0];
                rules.Add(new ItemRule
                {
                    NameRule = ruleName,
                    Data = line
                });
            }
            return rules;
        }

        public static Preset[] GetPresets()
        {
            DirectoryInfo directory = new(PRESET_FOLDER_PATH);
            FileInfo[] files = directory.GetFiles("*.txt");
            Preset[] presets = new Preset[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                presets[i] = new Preset(Path.GetFileNameWithoutExtension(files[i].Name));
            }
            return presets;
        }
    }
}