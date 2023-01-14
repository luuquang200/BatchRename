
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Core
{
    public class Preset
    {
        public static string PRESET_FOLDER_PATH => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")) + @"Presets\";
        public static readonly string PRESET_FILE_EXTENSION = ".txt";

        public string Name { get; }

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

        public string GetPath(string name = null) => name switch
        {
            null => Path.Combine(PRESET_FOLDER_PATH, Name + PRESET_FILE_EXTENSION),
            _ => Path.Combine(PRESET_FOLDER_PATH, name + PRESET_FILE_EXTENSION)
        };

        public bool Exists(string name = null)
        {
            return name is null ? File.Exists(GetPath()) : File.Exists(GetPath(name));
        }

        public string[] Read()
        {
            return File.ReadAllLines(GetPath());
        }

        public List<IRule> GetRules()
        {
            var rules = new List<IRule>();

            var lines = Read();

            foreach (var line in lines)
            {
                //rules.Add(RuleFactory.Parse(line));
                IRule rule = RuleFactory.Parse(line);
                rules.Add(rule);
                Debug.WriteLine(rule.Name);
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