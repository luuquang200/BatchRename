using Core;
using System.Text;

namespace OneSpaceRule
{
    public class OneSpaceRule : IRule
    {
        public string Name => "OneSpace";

        public Dictionary<string, string> ListParameter { get; set; } = new();

        public object Clone()
        {
            return MemberwiseClone();
        }


        public IRule Parse(string line)
        {
            return new OneSpaceRule();
        }

        public string Rename(string origin)
        {
            var builder = new StringBuilder();
            builder.Append(origin[0]);
            int length = origin.Length;
            for (int i = 1; i < length - 1; i++)
            {
                if (origin[i] == ' ')
                {
                    if (origin[i - 1] != ' ')
                    {
                        builder.Append(origin[i]);
                    }
                }
                else
                {
                    builder.Append(origin[i]);
                }
            }

            var result = builder.ToString();
            return result;
        }

        public void SetData(string data)
        {

        }
    }
}