using System;
using System.Collections.Generic;

namespace BatchRename
{
    public class RuleFactory
    {
        private static readonly Dictionary<string, IRule> _prototypes = new();

        public static void Register(IRule prototype)
        {
            _prototypes.Add(prototype.Name, prototype);
        }

        private static RuleFactory _instance = null;

        public static RuleFactory Instance()
        {
            _instance ??= new RuleFactory();

            return _instance;
        }

        private RuleFactory()
        {
        }

        public static IRule Parse(string data)
        {
            const string Space = " ";

            var tokens = data.Split(
                new string[] { Space }, StringSplitOptions.None
            );
            var keyword = tokens[0];
            IRule result = null;

            if (_prototypes.ContainsKey(keyword))
            {
                IRule prototype = _prototypes[keyword];
                result = prototype.Parse(data);
            }

            return result;
        }
    }
}