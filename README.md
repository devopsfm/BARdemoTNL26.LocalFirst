Swap is a .NET 10 console app that sends one prompt through Microsoft.Extensions.AI's IChatClient and switches between a local Ollama model and an Azure AI Foundry deployment by changing only a config file.
Run against local Ollama: `dotnet run -- local`
Run against Foundry with `AI_API_KEY` set in the environment: `dotnet run -- cloud`
