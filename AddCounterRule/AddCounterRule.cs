using System.Diagnostics;
using System.Text;
using Core;

namespace AddCounterRule
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

        public Dictionary<string, string> ListParameter
        {
            get;
            set;
        }

        public AddCounterRule()
        {
            Start = 1;
            Step = 3;
            ListParameter = new Dictionary<string, string>
            {
                { "Start", Start.ToString() },
                { "Step", Step.ToString() },
                { "NumberOfDigits", NumberOfDigits.ToString() }
            };
        }
        public string Rename(string origin, bool isFile)
        {
            string fileName = origin;
            string extension = "";
            if (isFile)
            {
                int indexExtension = 0;
                for (int i = 0; i < origin.Length; i++)
                {
                    if (origin[i].Equals('.'))
                    {
                        indexExtension = i;
                    }
                }
                fileName = origin.Substring(0, indexExtension);
                extension = origin.Substring(indexExtension + 1, origin.Length - indexExtension - 1);
            }

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
            if(isFile)
            {
                builder.Append('.');
                builder.Append(extension);
            }

            string result = builder.ToString();
            _current += Step;
            return result;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public IRule Parse(string dataInput)
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



            var rule = new AddCounterRule
            {
                Start = int.Parse(pairs0[1]),
                Step = int.Parse(pairs1[1]),
                NumberOfDigits = int.Parse(pairs2[1]),
                ListParameter = new Dictionary<string, string>
                {
                    { "Start", int.Parse(pairs0[1]).ToString() },
                    { "Step", int.Parse(pairs1[1]).ToString() },
                    { "NumberOfDigits", int.Parse(pairs2[1]).ToString() }
                }
            };
            Debug.WriteLine("parameter: " + ListParameter.ElementAt(0).Key + " - " + ListParameter.ElementAt(0).Value);
            return rule;
        }

        public void ResetCounter()
        {
            _current = Start;
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
            //
            ListParameter["Start"] = Start.ToString();
            ListParameter["Step"] = Step.ToString();
            ListParameter["NumberOfDigits"] = NumberOfDigits.ToString();
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