using Core;
using System.Text;

namespace ChangeExtentionRule
{
	public class ChangeExtensionRule : IRule
	{
		public string Extention { get; set; }

		public string Name => "ChangeExtention";

		public Dictionary<string, string> ListParameter
		{
			get;
			set;
		}

		public ChangeExtensionRule()
		{
			Extention = "";
			ListParameter = new Dictionary<string, string>
			{
				{ "Extention", Extention }
			};
		}

		public string Rename(string origin, bool isFile)
		{
			if (!isFile)
			{
				return origin;
			}
			int indexExtension = 0;
			for (int i = 0; i < origin.Length; i++)
			{
				if (origin[i].Equals('.'))
				{
					indexExtension = i;
				}
			}

			string fileName = origin.Substring(0, indexExtension);
			string extension = Extention;

			StringBuilder stringBuilder = new();
			stringBuilder.Append(fileName);
			stringBuilder.Append('.');
			stringBuilder.Append(extension);

			return stringBuilder.ToString();
		}

		public object Clone()
		{
			return new ChangeExtensionRule
			{
				Extention = Extention,
				ListParameter = new Dictionary<string, string>
				{
					{ "Extention", Extention }
				}
			};
		}

		public IRule Parse(string line)
		{
			var tokens = line.Split(' ');
			var data = tokens[1];

			//var pairs = data.Split(' ');

			//var tokens = line.Split(new string[] { " " },
			//    StringSplitOptions.None);
			//var data = tokens[1];

			var pairs = data.Split(new string[] { "=" },
				StringSplitOptions.None);

			var rule = new ChangeExtensionRule
			{
				Extention = pairs[1],
				ListParameter = new Dictionary<string, string>
				{
					{ "Extention", pairs[1] }
				}
			};

			return rule;
		}

		public void SetData(string line)
		{
			var tokens = line.Split(' ');
			var data = tokens[1];

			var pairs = data.Split(new string[] { "=" },
				StringSplitOptions.None);

			Extention = pairs[1];
			ListParameter["Extention"] = pairs[1];
		}

		public IConfigRuleWindow ConfigRuleWindow()
		{
			throw new NotImplementedException();
		}
	}
}