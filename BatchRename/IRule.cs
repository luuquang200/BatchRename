using System;

namespace BatchRename
{
    public interface IRule : ICloneable
    {
        string Rename(string origin);
        string Name { get; }

        IRule Parse(string data);
    }
}
