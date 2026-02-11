# Jobo Enterprise .NET Client

Official .NET client library for the [Jobo Enterprise Jobs API](https://api.jobo.ai). Access millions of job listings from 15+ ATS platforms including Greenhouse, Lever, Workday, SmartRecruiters, and more.

[![NuGet](https://img.shields.io/nuget/v/Jobo.Enterprise.Client)](https://www.nuget.org/packages/Jobo.Enterprise.Client)
[![.NET](https://img.shields.io/badge/.NET-6.0%20%7C%208.0-blue)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

## Installation

```bash
dotnet add package Jobo.Enterprise.Client
```

Or via the NuGet Package Manager:

```
Install-Package Jobo.Enterprise.Client
```

## Quick Start

```csharp
using Jobo.Enterprise.Client;

using var client = new JoboClient(new JoboClientOptions { ApiKey = "your-api-key" });

var results = await client.SearchJobsAsync(q: "software engineer", location: "San Francisco");
foreach (var job in results.Jobs)
{
    Console.WriteLine($"{job.Title} at {job.Company.Name}");
}
```

## Authentication

All API requests require an API key passed via the `X-Api-Key` header. The client handles this automatically:

```csharp
var client = new JoboClient(new JoboClientOptions { ApiKey = "your-api-key" });
```

For self-hosted deployments:

```csharp
var client = new JoboClient(new JoboClientOptions
{
    ApiKey = "your-api-key",
    BaseUrl = "https://your-instance.example.com"
});
```

## Dependency Injection

Register the client in your ASP.NET Core application:

```csharp
using Jobo.Enterprise.Client.Extensions;

builder.Services.AddJoboClient(options =>
{
    options.ApiKey = builder.Configuration["Jobo:ApiKey"]!;
    options.BaseUrl = "https://api.jobo.ai";
    options.Timeout = TimeSpan.FromSeconds(60);
});
```

Or with a simple API key:

```csharp
builder.Services.AddJoboClient(builder.Configuration["Jobo:ApiKey"]!);
```

Then inject it:

```csharp
public class MyService(JoboClient jobo)
{
    public async Task DoWorkAsync()
    {
        var results = await jobo.SearchJobsAsync(q: "engineer");
    }
}
```

## Usage

### Job Search (Simple)

Search jobs with simple query parameters:

```csharp
var results = await client.SearchJobsAsync(
    q: "data scientist",
    location: "New York",
    sources: "greenhouse,lever",
    remote: true,
    page: 1,
    pageSize: 50
);

Console.WriteLine($"Found {results.Total} jobs across {results.TotalPages} pages");
foreach (var job in results.Jobs)
{
    Console.WriteLine($"  {job.Title} at {job.Company.Name} — {job.ListingUrl}");
}
```

### Job Search (Advanced)

Use the advanced endpoint for multiple queries and locations:

```csharp
using Jobo.Enterprise.Client.Models;

var results = await client.SearchJobsAdvancedAsync(new JobSearchRequest
{
    Queries = ["machine learning engineer", "ML engineer", "AI engineer"],
    Locations = ["San Francisco", "New York", "Remote"],
    Sources = ["greenhouse", "lever", "ashby"],
    IsRemote = true,
    PageSize = 100
});
```

### Auto-Paginated Search

Iterate over all matching jobs without managing pagination:

```csharp
await foreach (var job in client.EnumerateSearchJobsAsync(new JobSearchRequest
{
    Queries = ["backend engineer"],
    Locations = ["London"],
    PageSize = 100
}))
{
    Console.WriteLine($"{job.Title} — {job.Company.Name}");
}
```

### Jobs Feed (Bulk)

Fetch large batches of active jobs using cursor-based pagination:

```csharp
var response = await client.GetJobsFeedAsync(new JobFeedRequest
{
    Locations =
    [
        new LocationFilter { Country = "US", Region = "California" },
        new LocationFilter { Country = "US", City = "New York" }
    ],
    Sources = ["greenhouse", "workday"],
    IsRemote = true,
    BatchSize = 1000
});

Console.WriteLine($"Got {response.Jobs.Count} jobs, HasMore={response.HasMore}");

// Continue with cursor
if (response.HasMore)
{
    var next = await client.GetJobsFeedAsync(new JobFeedRequest
    {
        Cursor = response.NextCursor,
        BatchSize = 1000
    });
}
```

### Auto-Paginated Feed

Stream all jobs without managing cursors:

```csharp
await foreach (var job in client.EnumerateJobsFeedAsync(new JobFeedRequest
{
    Sources = ["greenhouse"],
    BatchSize = 1000
}))
{
    await ProcessJobAsync(job);
}
```

### Expired Job IDs

Sync expired jobs to keep your data fresh:

```csharp
var expiredSince = DateTime.UtcNow.AddDays(-1);

await foreach (var jobId in client.EnumerateExpiredJobIdsAsync(expiredSince))
{
    await MarkAsExpiredAsync(jobId);
}
```

## Error Handling

The client throws typed exceptions for different error scenarios:

```csharp
using Jobo.Enterprise.Client.Exceptions;

try
{
    var results = await client.SearchJobsAsync(q: "engineer");
}
catch (JoboAuthenticationException)
{
    Console.WriteLine("Invalid API key");
}
catch (JoboRateLimitException ex)
{
    Console.WriteLine($"Rate limited. Retry after {ex.RetryAfterSeconds}s");
}
catch (JoboValidationException ex)
{
    Console.WriteLine($"Bad request: {ex.Detail}");
}
catch (JoboServerException)
{
    Console.WriteLine("Server error — try again later");
}
catch (JoboException ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

## Models

All response data is returned as strongly-typed models:

| Model | Description |
|---|---|
| `Job` | A job listing with title, company, locations, compensation, etc. |
| `JobCompany` | Company ID and name |
| `JobLocation` | City, state, country, coordinates |
| `JobCompensation` | Min/max salary, currency, period |
| `LocationFilter` | Structured filter for feed endpoint |
| `JobFeedResponse` | Feed response with cursor pagination |
| `ExpiredJobIdsResponse` | Expired job IDs with cursor pagination |
| `JobSearchResponse` | Search response with page-based pagination |

## Supported Sources

| Category | Sources |
|---|---|
| **Tech/Startup** | `greenhouse`, `lever`, `ashby`, `workable`, `rippling`, `polymer` |
| **Enterprise** | `workday`, `smartrecruiters` |
| **SMB** | `bamboohr`, `breezy`, `jazzhr`, `recruitee`, `personio` |

## Configuration

| Property | Default | Description |
|---|---|---|
| `ApiKey` | *required* | Your Jobo Enterprise API key |
| `BaseUrl` | `https://api.jobo.ai` | API base URL |
| `Timeout` | `00:00:30` | Request timeout |

## Custom HttpClient

You can provide your own `HttpClient` for advanced scenarios (e.g., Polly retry policies):

```csharp
var httpClient = new HttpClient { BaseAddress = new Uri("https://api.jobo.ai") };
httpClient.DefaultRequestHeaders.Add("X-Api-Key", "your-api-key");

var client = new JoboClient(httpClient);
```

## Target Frameworks

- .NET 8.0
- .NET 6.0

## Development

```bash
git clone https://github.com/jobo-ai/jobo-dotnet.git
cd jobo-dotnet

# Build
dotnet build

# Pack
dotnet pack -c Release
```

## License

MIT — see [LICENSE](LICENSE).
