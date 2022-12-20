using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename
{
    public class RuleFactory
    {
        static Dictionary<string, IRule> _prototypes = new Dictionary<string, IRule>();

        public static void Register(IRule prototype)
        {
            _prototypes.Add(prototype.Name, prototype);
        }

        private static RuleFactory? _instance = null;
        public static RuleFactory Instance()
        {
            if (_instance == null)
            {
                _instance = new RuleFactory();
            }

            return _instance;
        }

        private RuleFactory()
        {
        }
        public IRule? Parse(string data)
        {
            const string Space = " ";

            var tokens = data.Split(
                new string[] { Space }, StringSplitOptions.None
            );
            var keyword = tokens[0];
            IRule? result = null;

            if (_prototypes.ContainsKey(keyword))
            {
                IRule prototype = _prototypes[keyword];
                result = prototype.Parse(data);
            }

            return result;
        }
    }
    //public class RuleFactory
    //{
    //    public IRule Parse(string data)
    //    {
    //        var tokens = data.Split(new string[] { " " },
    //            StringSplitOptions.None);
    //        var keyword = tokens[0]; // RemoveSpecialChars / AddPrefix

    //        var rule1 = new RemoveSpecialCharsRule();
    //        var rule2 = new AddPrefixRule();
    //        var rule3 = new OneSpaceRule();
    //        var rule4 = new AddCounterRule();

    //        var prototypes = new Dictionary<string, IRule>()
    //        {
    //            { rule1.Name, rule1 },
    //            { rule2.Name, rule2 },
    //            { rule3.Name, rule3 },
    //            { rule4.Name, rule4 },
    //        };

    //        var rule = prototypes[keyword].Parse(data);

    //        return rule;
    //    }

    //}
}
