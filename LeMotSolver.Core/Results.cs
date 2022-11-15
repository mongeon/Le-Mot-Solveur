namespace LeMotSolveur.Core;

public class Results
{
    public string[] WordsList { get; set; }
    public IEnumerable<LetterFrequency> LetterProbability { get; set; }
}