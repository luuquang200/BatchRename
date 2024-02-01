using Core;
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
			stringBuilder.Append(fileName);
			stringBuilder.Append(Suffix);
			if (isFile)
			{
				stringBuilder.Append('.');
				stringBuilder.Append(extension);
			};

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
			return new AddSuffixRule
			{
				Suffix = Suffix,
				ListParameter = new Dictionary<string, string>
				{
					{ "Suffix", Suffix }
				}
			};
		}

		public void SetData(string data)
		{
			var tokens = data.Split(new char[] { ' ', '=' });
			Suffix = tokens[SUFFIX_INDEX];
			ListParameter["Suffix"] = Suffix;
		}
	}
}