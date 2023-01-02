using System;
using System.Collections.Generic;

namespace BatchRename.Core
{
    public interface IRule : ICloneable
    {
        string Rename(string origin);

        string Name { get; }

        IRule Parse(string line);
        public Dictionary<string, string> ListParameter { get; set; }
        public IConfigRuleWindow ConfigRuleWindow();
        public void SetData(string data);
    }
}