using System.IO;
using System.Net;
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

    [FunctionName("words")]
    [OpenApiOperation(operationId: "Run")]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ExecutionContext context)
    {
         _logger.LogInformation("C# HTTP trigger function processed a request.");

        var path = Path.Combine(context.FunctionAppDirectory, "Assets", "drawable-words.json");
        var words = await File.ReadAllTextAsync(path);

        return new OkObjectResult(words);
    }
}

