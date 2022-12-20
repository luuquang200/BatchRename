using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename
{
    public interface IRule : ICloneable
    {
        string Rename(string origin);
        string Name { get; }

        IRule Parse(string data);
    }
}
