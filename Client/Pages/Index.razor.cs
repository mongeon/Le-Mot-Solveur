using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace LeMotSolveur.Client.Pages;

public partial class Index
{
    [Inject]
    public HttpClient http { get; set; }
    public string PossibleCharacters { get; set; }

    
    public IEnumerable<string> allWords = new List<string>();
    public IEnumerable<string> words = new List<string>();

    protected override async Task OnInitializedAsync()
    {

        allWords = await http.GetFromJsonAsync<IEnumerable<string>>("api/words");
        words = allWords;
    }
    public async void Search()
    {
        PossibleCharacters = PossibleCharacters.ToUpperInvariant();
        var results = string.IsNullOrEmpty(PossibleCharacters) || PossibleCharacters.Equals("*") ?
                               allWords :
                               allWords.ToList().FindAll((string s) => Regex.IsMatch(s, "^[" + PossibleCharacters + "]{5}$"));
        words = results;
    }
}
