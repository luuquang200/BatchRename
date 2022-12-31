using System;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using BatchRename.Core;

namespace BatchRename.Rules
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
        public int NumberOfDigits { get; set; } = 1;

        public string Name => "AddCounter";


        public AddCounterRule()
        {
            Start = 1;
            Step = 3;
        }
        public string Rename(string origin)
        {
            var tokens = origin.Split(new string[] { "." },
                StringSplitOptions.None);
            string fileName = tokens[0];
            string extension = tokens[1];


            var builder = new StringBuilder();
            builder.Append(fileName);

            int countNumber = CountNumber(_current);
            int tempNumberOfDigits = NumberOfDigits;
            if (tempNumberOfDigits > countNumber)
            {
                tempNumberOfDigits -= countNumber;
                for (int i = 0; i < tempNumberOfDigits; i++)
                {
                    builder.Append('0');
                }
            }

           
            builder.Append(_current);
            builder.Append('.');
            builder.Append(extension);

            string result = builder.ToString();
            _current += Step;
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
            var pairs2 = attributes[2].Split(new string[] { "=" },
                           StringSplitOptions.None);

            var rule = new AddCounterRule
            {
                Start = int.Parse(pairs0[1]),
                Step = int.Parse(pairs1[1]),
                NumberOfDigits = int.Parse(pairs2[1])
            };
            return rule;
        }

        public void ResetCounter()
        {
            _current = Start;
        }

        public IConfigRuleWindow ConfigRuleWindow()
        {
            return new InputAddCounter(Start.ToString(), Step.ToString(), NumberOfDigits.ToString());

        }

        public void SetData(string dataInput)
        {
            var tokens = dataInput.Split(new string[] { " " },
               StringSplitOptions.None);
            var data = tokens[1];
            var attributes = data.Split(new string[] { "," },
                StringSplitOptions.None);
            var pairs0 = attributes[0].Split(new string[] { "=" },
                StringSplitOptions.None);
            var pairs1 = attributes[1].Split(new string[] { "=" },
                StringSplitOptions.None);
            var pairs2 = attributes[2].Split(new string[] { "=" },
                           StringSplitOptions.None);

            Start = int.Parse(pairs0[1]);
            Step = int.Parse(pairs1[1]);
            NumberOfDigits = int.Parse(pairs2[1]);
        }

        public static int CountNumber(int num)
        {
            int temp = num, count = 0;
            while (temp != 0)
            {
                count++;
                temp = temp / 10;
            }
            return count;
        }
    }
}