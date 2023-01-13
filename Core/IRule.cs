using System;
using System.Collections.Generic;

namespace Core
{
    public interface IRule : ICloneable
    {
        string Rename(string origin);

        string Name { get; }

        IRule Parse(string line);
        public Dictionary<string, string> ListParameter { get; set; }
        public void SetData(string data);
    }
}