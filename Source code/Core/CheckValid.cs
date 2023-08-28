using System.Xml.Linq;

namespace Core
{
    public class CheckValid
    {
        public static string CheckValidName(string origin, bool isFile)
        {
            string fileName = origin;
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
            }

            char[] invalidChar =
            {
                '<',
                '>',
                ':',
                '\"',
                '/',
                '\\',
                '|',
                '?',
                '*',
                (char)0,
                (char)1,
                (char)2,
                (char)3,
                (char)4,
                (char)5,
                (char)6,
                (char)7,
                (char)8,
                (char)9,
                (char)10,
                (char)11,
                (char)12,
                (char)13,
                (char)14,
                (char)15,
                (char)16,
                (char)17,
                (char)18,
                (char)19,
                (char)20,
                (char)21,
                (char)22,
                (char)23,
                (char)24,
                (char)25,
                (char)26,
                (char)27,
                (char)28,
                (char)29,
                (char)30,
                (char)31
            };

            foreach (char c in invalidChar)
            {
                if (fileName.Contains(c))
                {
                    return $"contain \"{c}\"";
                }
            }
                   

            if (fileName.Length > 225)
            {
                return "Name exceed 255 characters";
            }

            if (fileName.Equals(string.Empty))
            {
                return "Empty name";
            }

            return "";
        }
    }
}