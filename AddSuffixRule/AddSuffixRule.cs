using Core;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Text;

namespace AddSuffixRule
{
    public class AddSuffixRule : IRule
    {
        private readonly int SUFFIX_INDEX = 2;
        public string Suffix { get; set; }

        public string Name => "AddSuffix";

        public Dictionary<string, string> ListParameter { get; set; }

        public AddSuffixRule()
        {
            Suffix = string.Empty;
            ListParameter = new Dictionary<string, string>
            {
                { "Suffix",  Suffix }
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
            Debug.WriteLine("indexExt: " + indexExtension + "origin.Length :" +origin.Length.ToString() );
            string extension = origin.Substring(indexExtension + 1, origin.Length - indexExtension - 1);
            Debug.WriteLine("FileName: " + fileName);
            Debug.WriteLine("extension: " + extension);
            StringBuilder stringBuilder = new();
            stringBuilder.Append(fileName);    
            stringBuilder.Append(Suffix);
            stringBuilder.Append('.');
            stringBuilder.Append(extension);

            return stringBuilder.ToString();
        }

        public IRule Parse(string line)
        {
            // Extract Suffix
            var tokens = line.Split(new char[] { ' ', '=' });


            var rule = new AddSuffixRule
            {
                Suffix = tokens[SUFFIX_INDEX],
                ListParameter = new Dictionary<string, string>
                {
                    { "Suffix", tokens[SUFFIX_INDEX] }
                }
            };
            return rule;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }


        public void SetData(string data)
        {
            var tokens = data.Split(new char[] { ' ', '=' });
            Suffix = tokens[SUFFIX_INDEX];
            ListParameter["Suffix"] = Suffix;
        }
    }
}