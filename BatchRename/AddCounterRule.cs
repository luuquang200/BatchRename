using System;
using System.Text;

namespace BatchRename
{
    public class AddCounterRule : IRule
    {
        private int _current = 0;
        private int _start = 0;

        public int Start
        {
            get => _start; set
            {
                _start = value;

                _current = value;
            }
        }
        public int Step { get; set; }

        public string Name => "AddCounter";

        public AddCounterRule()
        {
            Start = 1;
            Step = 3;
        }
        public string Rename(string origin)
        {
            var builder = new StringBuilder();
            builder.Append(origin);
            builder.Append(" ");
            builder.Append(_current);

            _current += Step;

            string result = builder.ToString();
            return result;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public IRule Parse(string line)
        {
            var tokens = line.Split(new string[] { " " },
                StringSplitOptions.None);
            var data = tokens[1];
            var attributes = data.Split(new string[] { "," },
                StringSplitOptions.None);
            var pairs0 = attributes[0].Split(new string[] { "=" },
                StringSplitOptions.None);
            var pairs1 = attributes[1].Split(new string[] { "=" },
                StringSplitOptions.None);

            var rule = new AddCounterRule();
            rule.Start = int.Parse(pairs0[1]);
            rule.Step = int.Parse(pairs1[1]);

            return rule;
        }

        public void ResetCounter()
        {
            _current = Start;
        }
    }

}
