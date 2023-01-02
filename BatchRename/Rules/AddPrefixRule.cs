using BatchRename.Core;
using System;
using System.Collections.Generic;

namespace BatchRename.Rules
{
    public class AddPrefixRule : IRule
    {
        public string Prefix { get; set; }

        public string Name => "AddPrefix";

        public Dictionary<string, string> ListParameter {
            get => throw new NotImplementedException(); 
            set => throw new NotImplementedException();
        }

        public AddPrefixRule()
        {
            Prefix = "";
        }

        public string Rename(string origin)
        {
            return string.Concat(Prefix, origin);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public IRule Parse(string line)
        {
            var tokens = line.Split(' ');
            var data = tokens[1];

            //var pairs = data.Split(' ');

            //var tokens = line.Split(new string[] { " " },
            //    StringSplitOptions.None);
            //var data = tokens[1];

            var pairs = data.Split(new string[] { "=" },
                StringSplitOptions.None);

            var rule = new AddPrefixRule
            {
                Prefix = pairs[1]
            };

            return rule;
        }

        public IConfigRuleWindow ConfigRuleWindow()
        {
            throw new NotImplementedException();
        }

        public void SetData(string data)
        {
            throw new NotImplementedException();
        }
    }
}