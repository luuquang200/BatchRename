using BatchRename.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Shapes;

namespace BatchRename.Rules
{
    public class AddSuffixRule : IRule
    {
        private readonly int SUFFIX_INDEX = 2;
        public string Suffix { get; set; }

        public string Name => "AddSuffix";

        public Dictionary<string, string> ListParameter { get; set; }

        public AddSuffixRule()
        {
            Suffix = string.Empty;
            ListParameter = new Dictionary<string, string>
            {
                { "Suffix",  Suffix }
            };
        }

        public string Rename(string origin)
        {
            var tokens = origin.Split(new string[] { "." },
               StringSplitOptions.None);
            string fileName = tokens[0];
            string extension = tokens[1];

            StringBuilder stringBuilder = new();
            stringBuilder.Append(fileName);
            stringBuilder.Append(Suffix);
            stringBuilder.Append('.');
            stringBuilder.Append(extension);

            return stringBuilder.ToString();
            //return string.Concat(origin, Suffix);
        }

        public IRule Parse(string line)
        {
            // Extract Suffix
            var tokens = line.Split(new char[] { ' ', '=' });


            var rule = new AddSuffixRule
            {
                Suffix = tokens[SUFFIX_INDEX]
            };
            return rule;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public IConfigRuleWindow ConfigRuleWindow()
        {
            throw new System.NotImplementedException();
        }

        public void SetData(string data)
        {
            var tokens = data.Split(new char[] { ' ', '=' });
            Suffix = tokens[SUFFIX_INDEX];
            ListParameter["Suffix"] = Suffix;
        }
    }
}