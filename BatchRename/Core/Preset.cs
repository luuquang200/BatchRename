using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BatchRename.Core
{
    public class Preset
    {
        private static string PRESET_FOLDER_PATH => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")) + @"Presets\";
        private static readonly string PRESET_FILE_EXTENSION = ".txt";

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
    }
}