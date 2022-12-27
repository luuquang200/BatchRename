﻿using BatchRename.Core;
using System.Linq;

namespace BatchRename.Rules
{
    public class RemoveWhiteSpaceRule : IRule
    {
        public string Name => "RemoveWhiteSpace";

        public IRule Parse(string line)
        {
            return new RemoveWhiteSpaceRule();
        }

        public string Rename(string origin)
        {
            return new string(origin.Where(c => !char.IsWhiteSpace(c))
                                    .ToArray());
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}