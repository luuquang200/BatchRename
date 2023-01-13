using Core;
using System.Text;

namespace PascalCaseRule
{
    public class PascalCaseRule : IRule
    {
        public string Delimiter { get; set; } = "_";

        public string Name => "PascalCase";

        public Dictionary<string, string> ListParameter
        {
            get;
            set;
        }

        public PascalCaseRule()
        {

            ListParameter = new Dictionary<string, string>
            {
                { "Delimiter", Delimiter }
            };
        }

        public string Rename(string origin)
        {
            int indexExtension = 0;
            for (int i = 0; i < origin.Length; i++)
            {
                if (origin[i].Equals('.'))
                {
                    indexExtension = i;
                }
            }

            string fileName = origin.Substring(0, indexExtension);
            string extension = origin.Substring(indexExtension + 1, origin.Length - indexExtension - 1);

            string[] result = fileName.Split(Delimiter);

            StringBuilder stringBuilder = new();
            foreach (string str in result)
            {
                string temp = str;
                if (temp.Length != 0)
                    temp = temp.ToUpper()[0] + temp.Substring(1);
                stringBuilder.Append(temp);
            }

            stringBuilder.Append('.');
            stringBuilder.Append(extension);

            return stringBuilder.ToString();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public IRule Parse(string line)
        {
            var tokens = line.Split(' ');
            var data = tokens[1];

            var pairs = data.Split(new string[] { "=" },
                StringSplitOptions.None);

            var rule = new PascalCaseRule();
            {
                Delimiter = pairs[1];
            };

            return rule;
        }



        public void SetData(string line)
        {
            var tokens = line.Split(' ');
            var data = tokens[1];

            var pairs = data.Split(new string[] { "=" },
                StringSplitOptions.None);

            Delimiter = pairs[1];
            ListParameter["Delimiter"] = pairs[1];
        }

    }
}