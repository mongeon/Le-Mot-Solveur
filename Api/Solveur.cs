using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace LeMotSolveur.Api;

public class Solveur
{
    private readonly ILogger<Solveur> _logger;

    public Solveur(ILogger<Solveur> log)
    {
        _logger = log;
    }

    [FunctionName("GetAllWords")]
    [OpenApiOperation(operationId: "Run")]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "words")] HttpRequest req, ExecutionContext context)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string path = GetAllWords(context);
        var words = await File.ReadAllTextAsync(path);

        return new OkObjectResult(words);
    }

    [FunctionName("ResolveWords")]
    [OpenApiOperation(operationId: "Resolve")]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Parameters), Required = true, Description = "Parameters")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> Resolve(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "words")] HttpRequest req, ExecutionContext context)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        if (req.Body != null && req.ContentLength > 0)
        {
            string path = GetAllWords(context);
            var fileContent = await File.ReadAllTextAsync(path);

            var allWords = JsonSerializer.Deserialize<List<string>>(fileContent);
            var solver = new FindWords(allWords);
            // read the contents of the posted data into a string
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            // use Json.NET to deserialize the posted JSON into a C# dynamic object
            var data = JsonSerializer.Deserialize<Parameters>(requestBody, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            var words = solver.Process(data.FixedLetters, data.GoodLetters, data.BadLetters);

            return new OkObjectResult(words);
        }
        return new BadRequestObjectResult("Body of request is invalid");
    }

    private static string GetAllWords(ExecutionContext context)
    {
        return Path.Combine(context.FunctionAppDirectory, "Assets", "drawable-words.json");
    }


    //[OpenApiExample(typeof(Parameters))]
    public class Parameters
    {
        [JsonPropertyName("fixedLetters")]
        [Required(AllowEmptyStrings = true)]
        public string FixedLetters { get; set; }
        [JsonPropertyName("goodLetters")]
        [Required(AllowEmptyStrings = true)]
        public string GoodLetters { get; set; }

        /// <summary>
        /// List of all letters that are not in the word
        /// </summary>
        [JsonPropertyName("badLetters")]
        [Required(AllowEmptyStrings = true)]
        public string BadLetters { get; set; }
    }
}

