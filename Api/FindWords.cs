using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LeMotSolveur.Api;

public class FindWords
{
	private readonly IEnumerable<string> _words;

	public FindWords(IEnumerable<string> words)
	{
		_words = words;
	}
	public IEnumerable<string> Process(string fixedLetters, string goodLetters, string badLetters)
	{
        if (string.IsNullOrEmpty(goodLetters))
			goodLetters = string.Empty;      
		if (string.IsNullOrEmpty(badLetters))		
			badLetters = string.Empty;
		
		return Process(fixedLetters.ToCharArray(), goodLetters.ToCharArray(), badLetters.ToCharArray());
	}
	public IEnumerable<string> Process(char[] fixedLetters, char[] goodLetters, char[] badLetters)
	{
		var regexExpression = "";

		foreach (var goodLetter in goodLetters)
		{
			regexExpression += $"(?=[A-Z]*{goodLetter.ToString().ToUpperInvariant()})";
		}
		if (badLetters.Length > 0)
		{
			regexExpression += $"(?![A-Z]*[{string.Concat(badLetters).ToUpperInvariant()}])";
		}

		regexExpression = $"^{regexExpression}[A-Z]{{5}}$";
		var results = _words.ToList().FindAll((string s) => Regex.IsMatch(s, regexExpression));

		regexExpression = string
			.Concat(fixedLetters)
			.ToUpperInvariant()
			.PadRight(5, '*')
			.Replace("*", "[A-Z]{1}")
			.Replace(" ", "[A-Z]{1}")
			.Replace("_", "[A-Z]{1}");

		regexExpression = $"^{regexExpression}$";
		results = results.ToList().FindAll((string s) => Regex.IsMatch(s, regexExpression));

		return results;
	}

}