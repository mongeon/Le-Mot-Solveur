namespace LeMotSolveur.Core;

public class Results
{
    public IEnumerable<string> WordsList { get; set; } = new List<string>();
    public IEnumerable<LetterFrequency> LetterProbability { get; set; } = new List<LetterFrequency>();
}