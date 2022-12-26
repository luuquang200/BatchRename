using System;
using System.Text;
using BatchRename.Core;

namespace BatchRename.Rules
{
    public class AddPrefixRule : IRule
    {
        public string Prefix { get; set; }

        public string Name => "AddPrefix";

        public AddPrefixRule()
        {
            Prefix = "";
        }

        public string Rename(string origin)
        {
            var builder = new StringBuilder();
            builder.Append(Prefix);
            builder.Append(' ');
            builder.Append(origin);

            string result = builder.ToString();
            return result;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public IRule Parse(string line)
        {
            var tokens = line.Split(new string[] { " " },
                StringSplitOptions.None);
            var data = tokens[1];

            var pairs = data.Split(new string[] { "=" },
                StringSplitOptions.None);

            var rule = new AddPrefixRule
            {
                Prefix = pairs[1]
            };
            return rule;
        }
    }
}