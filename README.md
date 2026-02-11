<p align="center">
  <img src="https://raw.githubusercontent.com/Prakkie91/jobo-dotnet/main/jobo-logo.png" alt="Jobo" width="120" />
</p>

<h1 align="center">Jobo Enterprise — .NET Client</h1>

<p align="center">
  <strong>Access millions of job listings from 45+ ATS platforms with a single API.</strong><br/>
  Build job boards, power job aggregators, or sync ATS data — Greenhouse, Lever, Workday, iCIMS, and more.
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/Jobo.Enterprise.Client"><img src="https://img.shields.io/nuget/v/Jobo.Enterprise.Client" alt="NuGet" /></a>
  <a href="https://dotnet.microsoft.com/"><img src="https://img.shields.io/badge/.NET-6.0%20%7C%208.0-blue" alt=".NET" /></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-blue.svg" alt="License: MIT" /></a>
</p>

---

## Why Jobo Enterprise?

- **45+ ATS integrations** — Greenhouse, Lever, Workday, iCIMS, SmartRecruiters, BambooHR, Ashby, and many more
- **Bulk feed endpoint** — Cursor-based pagination to sync millions of jobs efficiently
- **Real-time search** — Full-text search with location, remote, and source filters
- **Expired job sync** — Keep your job board fresh by removing stale listings
- **ASP.NET Core DI** — First-class dependency injection support
- **Strongly typed** — Complete model definitions with `System.Text.Json` serialization
- **IAsyncEnumerable** — Stream results with `await foreach` for auto-pagination

> **Get your API key** → [enterprise.jobo.world/api-keys](https://enterprise.jobo.world/api-keys)
>
> **Learn more** → [jobo.world/enterprise](https://jobo.world/enterprise/)

---

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

// Search for software engineering jobs from Greenhouse
var results = await client.SearchJobsAsync(
    q: "software engineer",
    location: "San Francisco",
    sources: "greenhouse,lever"
);

foreach (var job in results.Jobs)
{
    Console.WriteLine($"{job.Title} at {job.Company.Name} — {job.ListingUrl}");
}
```

## Authentication

Get your API key at **[enterprise.jobo.world/api-keys](https://enterprise.jobo.world/api-keys)**.

All requests require an API key passed via the `X-Api-Key` header. The client handles this automatically:

```csharp
var client = new JoboClient(new JoboClientOptions { ApiKey = "your-api-key" });
```

## Dependency Injection

Register the client in your ASP.NET Core application:

```csharp
using Jobo.Enterprise.Client.Extensions;

builder.Services.AddJoboClient(options =>
{
    options.ApiKey = builder.Configuration["Jobo:ApiKey"]!;
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

### Search Jobs (Simple)

Search jobs with query parameters — ideal for building job board search pages:

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

### Search Jobs (Advanced)

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

### Bulk Jobs Feed

Fetch large batches of active jobs using cursor-based pagination — perfect for building a job aggregator or syncing to your database:

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
    await SaveToDatabaseAsync(job);
}
```

### Expired Job IDs

Keep your job board fresh by syncing expired listings:

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
    Console.WriteLine("Invalid API key — get one at https://enterprise.jobo.world/api-keys");
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

## Supported ATS Sources (45+)

Filter jobs by any of these applicant tracking systems:

| Category | Sources |
|---|---|
| **Enterprise ATS** | `workday`, `smartrecruiters`, `icims`, `successfactors`, `oraclecloud`, `taleo`, `dayforce`, `csod` (Cornerstone), `adp`, `ultipro`, `paycom` |
| **Tech & Startup** | `greenhouse`, `lever_co`, `ashby`, `workable`, `workable_jobs`, `rippling`, `polymer`, `gem`, `pinpoint`, `homerun` |
| **Mid-Market** | `bamboohr`, `breezy`, `jazzhr`, `recruitee`, `personio`, `jobvite`, `teamtailor`, `comeet`, `trakstar`, `zoho` |
| **SMB & Niche** | `gohire`, `recooty`, `applicantpro`, `hiringthing`, `careerplug`, `hirehive`, `kula`, `careerpuck`, `talnet`, `jobscore` |
| **Specialized** | `freshteam`, `isolved`, `joincom`, `eightfold`, `phenompeople` (via `eightfold`) |

> Pass source identifiers in the `Sources` parameter, e.g. `Sources = ["greenhouse", "lever_co", "workday"]`

## Configuration

| Property | Default | Description |
|---|---|---|
| `ApiKey` | *required* | Your Jobo Enterprise API key ([get one here](https://enterprise.jobo.world/api-keys)) |
| `BaseUrl` | `https://jobs-api.jobo.world` | API base URL |
| `Timeout` | `00:00:30` | Request timeout |

## Custom HttpClient

You can provide your own `HttpClient` for advanced scenarios (e.g., Polly retry policies):

```csharp
var httpClient = new HttpClient { BaseAddress = new Uri("https://jobs-api.jobo.world") };
httpClient.DefaultRequestHeaders.Add("X-Api-Key", "your-api-key");

var client = new JoboClient(httpClient);
```

## Target Frameworks

- .NET 8.0
- .NET 6.0

## Use Cases

- **Build a job board** — Search and display jobs from 45+ ATS platforms
- **Job aggregator** — Bulk-sync millions of listings with the feed endpoint
- **ATS data pipeline** — Pull jobs from Greenhouse, Lever, Workday, etc. into your data warehouse
- **Recruitment tools** — Power candidate-facing job search experiences
- **Market research** — Analyze hiring trends across companies and industries

## Development

```bash
git clone https://github.com/Prakkie91/jobo-dotnet.git
cd jobo-dotnet

# Build
dotnet build

# Test
dotnet test

# Pack
dotnet pack -c Release
```

## Publishing to NuGet

```bash
# Build the package
dotnet pack -c Release

# Push to NuGet (replace YOUR_NUGET_API_KEY)
dotnet nuget push bin/Release/Jobo.Enterprise.Client.*.nupkg --api-key YOUR_NUGET_API_KEY --source https://api.nuget.org/v3/index.json

# Tag and push to GitHub
git tag v$(grep -oPm1 '(?<=<Version>)[^<]+' Jobo.Enterprise.Client.csproj)
git push origin main --tags
```

## Pushing to GitHub

```bash
# Initial setup (one-time)
git remote set-url origin https://github.com/Prakkie91/jobo-dotnet.git

# Push
git add -A
git commit -m "release: v$(grep -oPm1 '(?<=<Version>)[^<]+' Jobo.Enterprise.Client.csproj)"
git push origin main
```

## Links

- **Website** — [jobo.world/enterprise](https://jobo.world/enterprise/)
- **Get API Key** — [enterprise.jobo.world/api-keys](https://enterprise.jobo.world/api-keys)
- **GitHub** — [github.com/Prakkie91/jobo-dotnet](https://github.com/Prakkie91/jobo-dotnet)
- **NuGet** — [nuget.org/packages/Jobo.Enterprise.Client](https://www.nuget.org/packages/Jobo.Enterprise.Client)

## License

MIT — see [LICENSE](LICENSE).
