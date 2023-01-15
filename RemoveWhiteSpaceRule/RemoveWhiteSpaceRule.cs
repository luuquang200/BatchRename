using Core;

namespace RemoveWhiteSpaceRule
{
    public class RemoveWhiteSpaceRule : IRule
    {
        public string Name => "RemoveWhiteSpace";

        public Dictionary<string, string> ListParameter { get; set; } = new();

        public IRule Parse(string line)
        {
            return new RemoveWhiteSpaceRule();
        }

        public string Rename(string origin, bool isFile)
        {
            return new string(origin.Where(c => !char.IsWhiteSpace(c))
                                    .ToArray());
        }

        public object Clone()
        {
            return MemberwiseClone();
        }


        public void SetData(string data)
        {

        }
    }
}