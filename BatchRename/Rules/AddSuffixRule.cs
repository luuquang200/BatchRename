using BatchRename.Core;
using System;
using System.Text;

namespace BatchRename.Rules
{
    public class AddSuffixRule : IRule
    {
        private readonly int SUFFIX_INDEX = 2;
        public string Suffix { get; set; }

        public string Name => "AddSuffix";

        public AddSuffixRule()
        {
            Suffix = string.Empty;
        }

        public string Rename(string origin)
        {
            return string.Concat(origin, Suffix);
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
    }
}