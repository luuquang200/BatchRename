using Core;
using System.Diagnostics;
using System.Text;

namespace UppercaseRule
{
    public class UppercaseRule : IRule
    {
        public string Name => "Uppercase";

        public Dictionary<string, string> ListParameter { get; set; } = new();

        public object Clone()
        {
            return MemberwiseClone();
        }


        public IRule Parse(string line)
        {
            return new UppercaseRule();
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

            StringBuilder stringBuilder = new();
            stringBuilder.Append(fileName.ToUpper());
            if (isFile)
            {
                stringBuilder.Append('.');
                stringBuilder.Append(extension);
            };

            return stringBuilder.ToString();
        }

        public void SetData(string data) { }
    }
}