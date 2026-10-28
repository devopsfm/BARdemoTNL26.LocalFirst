---
mode: agent
---
Add a Polly retry policy around the chat call in Program.cs: 3 attempts, exponential backoff starting at 500 ms, retry on HttpRequestException and on ClientResultException with HTTP status 429 or 5xx. Add the package reference. Keep the console output identical. Run dotnet build to confirm.
