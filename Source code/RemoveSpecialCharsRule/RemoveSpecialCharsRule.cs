﻿using Core;
using System.Text;

namespace RemoveSpecialCharsRule
{
	public class RemoveSpecialCharsRule : IRule
	{
		public List<string> SpecialChars { get; set; }
		public string Replacement { get; set; }

		public string Name => "RemoveSpecialChars";

		public Dictionary<string, string> ListParameter { get; set; }

		public RemoveSpecialCharsRule()
		{
			SpecialChars = new List<string>();
			Replacement = " ";

			StringBuilder stringBuilder = new StringBuilder();
			foreach (string s in SpecialChars)
			{
				stringBuilder.Append(s);
			}
			ListParameter = new Dictionary<string, string>
			{
				{ "SpecialChars", stringBuilder.ToString() }
			};
		}

		public string Rename(string origin, bool isFile)
		{
			StringBuilder builder = new();
			foreach (var c in origin)
			{
				if (SpecialChars.Contains($"{c}"))
				{
					builder.Append(Replacement);
				}
				else
				{
					builder.Append(c);
				}
			}

			string result = builder.ToString();
			return result;
		}

		public object Clone()
		{
			return new RemoveSpecialCharsRule
			{
				SpecialChars = new List<string>(SpecialChars),
				ListParameter = new Dictionary<string, string>
				 {
					{ "SpecialChars", ListParameter["SpecialChars"] }
				 }
			};
		}

		public IRule Parse(string line)
		{
			var tokens = line.Split(new string[] { " " },
				StringSplitOptions.None);
			var data = tokens[1]; // SpecialChars=-_
			var pairs = data.Split(new string[] { "=" },
				StringSplitOptions.None); // -_
			var specials = pairs[1];

			var rule = new RemoveSpecialCharsRule();

			foreach (var c in specials)
			{
				rule.SpecialChars.Add($"{c}");
			}

			rule.ListParameter = new Dictionary<string, string>
			{
				{ "SpecialChars", specials }
			};

			return rule;
		}

		public void SetData(string dataInput)
		{
			var tokens = dataInput.Split(new string[] { " " },
				StringSplitOptions.None);
			var data = tokens[1]; // SpecialChars=-_
			var pairs = data.Split(new string[] { "=" },
				StringSplitOptions.None); // -_
			var specials = pairs[1];

			SpecialChars.Clear();
			foreach (var c in specials)
			{
				SpecialChars.Add($"{c}");
			}
			ListParameter["SpecialChars"] = specials;
		}
	}
}