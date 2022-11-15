using System;
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
    public Core.Results Process(string fixedLetters, string goodLetters, string badLetters)
    {
        if (string.IsNullOrEmpty(fixedLetters))

            fixedLetters = string.Empty;
        if (string.IsNullOrEmpty(goodLetters))
            goodLetters = string.Empty;
        if (string.IsNullOrEmpty(badLetters))
            badLetters = string.Empty;

        return Process(fixedLetters.ToCharArray(), goodLetters.ToCharArray(), badLetters.ToCharArray());
    }
    public Core.Results Process(char[] fixedLetters, char[] goodLetters, char[] badLetters)
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

        var wordsResult = new Core.Results() { WordsList = results };

        var letterFrequencies = CalculateLettersFrequencies(results);
        wordsResult.LetterProbability = letterFrequencies;
        return wordsResult;
    }

    private IEnumerable<Core.LetterFrequency> CalculateLettersFrequencies(List<string> words)
    {
        var letterFrequencies = new List<Core.LetterFrequency>();
        var calcLetter = new Dictionary<char, int>();
        var total = 0;

        foreach (var word in words)
        {
            for (int i = 0; i < word.Length; i++)
            {
                char letter = word[i];
                if (calcLetter.ContainsKey(letter))
                {
                    calcLetter[letter]++;
                }
                else
                    calcLetter[letter] = 1;
                total++;
            }
        }

        foreach (var letter in calcLetter)
        {
            letterFrequencies.Add(new Core.LetterFrequency
            {
                Letter = letter.Key,
                Count = letter.Value,
                Probability = Decimal.Divide(letter.Value, total)
            });
        }

        return letterFrequencies;
    }
}