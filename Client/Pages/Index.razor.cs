using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace LeMotSolveur.Client.Pages;

public partial class Index
{
    [Inject]
    public HttpClient http { get; set; }
    public string PossibleCharacters { get; set; }
    public string ExactCharacters { get; set; }
    public string BadCharacters { get; set; }
    

    public IEnumerable<string> allWords = new List<string>();
    public IEnumerable<string> words = new List<string>();
    public bool IsSearching = false;

    protected override async Task OnInitializedAsync()
    {
        //IsSearching = true;
        //allWords = await http.GetFromJsonAsync<IEnumerable<string>>("api/words");
        words = allWords;
        //IsSearching = false;
    }
    public async void Search()
    {
        IsSearching = true;
        var request = new
        {
            fixedLetters = ExactCharacters?.ToUpperInvariant(),
            goodLetters = PossibleCharacters?.ToUpperInvariant(),
            badLetters = BadCharacters?.ToUpperInvariant(),
        };
        var result = await http.PostAsJsonAsync("api/words", request);
        if (result.IsSuccessStatusCode)
        {
            
            words = await result.Content.ReadFromJsonAsync<IEnumerable<string>>();
        }
        else
        {
            words = new List<string>();
        }        
        IsSearching = false;
        base.StateHasChanged();
    }
}
