﻿using System;
using System.Collections.Generic;

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

        private RuleFactory()
        {
        }

        public static IRule Parse(string line)
        {
            const char Space = ' ';

            var tokens = line.Split(Space);
            var keyword = tokens[0];
            IRule result = null;

            if (_prototypes.TryGetValue(keyword, out IRule rule))
            {
                result = rule.Parse(line);
            }

            return result;
        }
    }
}