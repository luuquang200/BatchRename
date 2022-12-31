using System;

namespace BatchRename.Core
{
    public interface IRule : ICloneable
    {
        string Rename(string origin);

        string Name { get; }

        IRule Parse(string line);

        public IConfigRuleWindow ConfigRuleWindow();
        public void SetData(string data);
    }
}