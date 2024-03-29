﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace Core
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

        public static IRule Parse(string data)
        {
            const char Space = ' ';

            var tokens = data.Split(Space);
            var keyword = tokens[0];
            IRule result = null;

            if (_prototypes.TryGetValue(keyword, out IRule value))
            {
                IRule prototype = value;
                result = prototype.Parse(data);
            }

            return result;
        }

        public static string[] GetAllRulesName()
        {
            return _prototypes.Keys.ToArray();
        }
    }
}