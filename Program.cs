using System.ClientModel;
using System.Diagnostics;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using Polly;
using System.Net.Http;

var profile = args.Length > 0 ? args[0] : "local";
var config = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.{profile}.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

string provider = config["Provider"]!;
string endpoint = config["Endpoint"]!;
string model    = config["Model"]!;
string apiKey   = Environment.GetEnvironmentVariable("AI_API_KEY") ?? "ollama";

IChatClient client = new OpenAIClient(
        new ApiKeyCredential(apiKey),
        new OpenAIClientOptions { Endpoint = new Uri(endpoint) })
    .GetChatClient(model)
    .AsIChatClient();

const string prompt = "In two sentences, explain prefill versus decode in LLM inference to a .NET developer.";

var sw = Stopwatch.StartNew();
ChatResponse response = await Policy
    .Handle<HttpRequestException>()
    .Or<ClientResultException>(ex => ex.Status == 429 || (ex.Status >= 500 && ex.Status <= 599))
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(500 * Math.Pow(2, retryAttempt - 1)))
    .ExecuteAsync(async () => await client.GetResponseAsync(prompt));
sw.Stop();

Console.WriteLine(response.Text);
Console.WriteLine();
Console.WriteLine($"[{provider}] host={new Uri(endpoint).Host} model={response.ModelId ?? model} " +
                  $"in={response.Usage?.InputTokenCount} out={response.Usage?.OutputTokenCount} {sw.ElapsedMilliseconds} ms");
