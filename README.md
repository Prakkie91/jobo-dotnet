<img src="https://raw.githubusercontent.com/Prakkie91/jobo-dotnet/main/jobo-logo.png" alt="Jobo" width="120" />

# Jobo Enterprise — .NET Client

**Access millions of job listings, geocode locations, and automate job applications — all from a single API.**

[![NuGet](https://img.shields.io/nuget/v/Jobo.Enterprise.Client)](https://www.nuget.org/packages/Jobo.Enterprise.Client)
[![.NET](https://img.shields.io/badge/.NET-6.0%20%7C%208.0-blue)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

---

## Features

| Sub-client          | Property           | Description                                              |
| ------------------- | ------------------ | -------------------------------------------------------- |
| **Jobs Feed**       | `client.Feed`      | Bulk job feed with cursor-based pagination (45+ ATS)     |
| **Jobs Search**     | `client.Search`    | Full-text search with location, work-model, and source filters |
| **Companies**       | `client.Companies` | Enriched company profiles and per-company job listings   |
| **Locations**       | `client.Locations` | Geocode location strings into structured coordinates     |
| **Auto Apply**      | `client.AutoApply` | Automate job applications: sessions, stored profiles, one-shot runs |

> **Get your API key** → [enterprise.jobo.world/api-keys](https://enterprise.jobo.world/api-keys)

---

## Installation

```bash
dotnet add package Jobo.Enterprise.Client
```

## Quick Start

```csharp
using Jobo.Enterprise.Client;
using Jobo.Enterprise.Client.Models;

using var client = new JoboClient(new JoboClientOptions { ApiKey = "your-api-key" });

// Search for jobs
var results = await client.Search.SearchAsync(q: "software engineer", location: "San Francisco");
foreach (var job in results.Jobs)
    Console.WriteLine($"{job.Title} at {job.Company.Name}");

// Geocode a location
var geo = await client.Locations.GeocodeAsync("London, UK");
Console.WriteLine($"{geo.Locations[0].DisplayName}: {geo.Locations[0].Latitude}, {geo.Locations[0].Longitude}");
```

## Authentication

```csharp
var client = new JoboClient(new JoboClientOptions { ApiKey = "your-api-key" });
```

## Dependency Injection

```csharp
using Jobo.Enterprise.Client.Extensions;

builder.Services.AddJoboClient(options =>
{
    options.ApiKey = builder.Configuration["Jobo:ApiKey"]!;
    options.Timeout = TimeSpan.FromSeconds(60);
});
```

Then inject and use sub-clients:

```csharp
public class MyService(JoboClient jobo)
{
    public async Task DoWorkAsync()
    {
        var results = await jobo.Search.SearchAsync(q: "engineer");
        var geo = await jobo.Locations.GeocodeAsync("Berlin, DE");
    }
}
```

---

## Jobs Feed — `client.Feed`

Bulk-sync millions of active jobs using cursor-based pagination.

### Fetch a batch

```csharp
var response = await client.Feed.GetJobsAsync(new JobFeedRequest
{
    Locations = [
        new LocationFilter { Country = "US", Region = "California" },
        new LocationFilter { Country = "US", City = "New York" }
    ],
    Sources = ["greenhouse", "workday"],
    WorkModels = ["remote", "hybrid"],
    BatchSize = 1000
});

Console.WriteLine($"Got {response.Jobs.Count} jobs, HasMore={response.HasMore}");
```

### Auto-paginate all jobs

```csharp
await foreach (var job in client.Feed.EnumerateJobsAsync(new JobFeedRequest
{
    Sources = ["greenhouse"],
    BatchSize = 1000
}))
{
    await SaveToDatabaseAsync(job);
}
```

### Expired job IDs

```csharp
await foreach (var jobId in client.Feed.EnumerateExpiredJobIdsAsync(DateTime.UtcNow.AddDays(-1)))
{
    await MarkAsExpiredAsync(jobId);
}
```

---

## Jobs Search — `client.Search`

Full-text search with filters and page-based pagination.

### Simple search

```csharp
var results = await client.Search.SearchAsync(
    q: "data scientist",
    location: "New York",
    sources: "greenhouse,lever",
    workModel: WorkModel.Remote, // or just "remote"
    minSalaryUsd: 120000,
    pageSize: 50
);

Console.WriteLine($"Found {results.Total} jobs across {results.TotalPages} pages");
```

> **Closed value sets.** Parameters with a fixed set of accepted values expose
> their known values as `const string` fields for discoverability — `WorkModel`,
> `EmploymentType`, `ExperienceLevel`, `CompensationPeriod`, and `SkillType`.
> The methods still take `string`, so passing the literal (e.g. `"remote"`) is
> always valid too.

### Advanced search (multiple queries, filters & facets)

```csharp
var results = await client.Search.SearchAdvancedAsync(new JobSearchRequest
{
    Queries = ["machine learning engineer", "ML engineer", "AI engineer"],
    Locations = ["San Francisco", "New York"],
    Sources = ["greenhouse", "lever", "ashby"],
    WorkModels = ["remote", "hybrid"],
    Skills = new InclusionExclusionFilter { Include = ["python", "pytorch"], Exclude = ["php"] },
    SalaryUsd = new RangeFilter { Min = 150000 },
    IncludeFacets = ["source", "experience_level"],
    PageSize = 100
});

// Inspect aggregated facet counts
foreach (var (facet, buckets) in results.Facets)
foreach (var bucket in buckets)
    Console.WriteLine($"{facet}: {bucket.Key} ({bucket.Count})");
```

### Auto-paginate all results

```csharp
await foreach (var job in client.Search.EnumerateAsync(new JobSearchRequest
{
    Queries = ["backend engineer"],
    Locations = ["London"],
    PageSize = 100
}))
{
    Console.WriteLine($"{job.Title} — {job.Company.Name}");
}
```

---

## Companies — `client.Companies`

Fetch enriched company profiles and list jobs scoped to a single company.

```csharp
// Full enriched profile (this endpoint is public — no API key required)
var company = await client.Companies.GetAsync(companyId);
Console.WriteLine($"{company.Name} — {company.Website}");

// Jobs for that company, newest first
var jobs = await client.Companies.GetJobsAsync(companyId, page: 1, pageSize: 25);
foreach (var job in jobs.Jobs)
    Console.WriteLine($"{job.Title} ({job.WorkplaceType})");
```

---

## Locations — `client.Locations`

Geocode location strings into structured data with coordinates.

```csharp
var result = await client.Locations.GeocodeAsync("San Francisco, CA");

foreach (var location in result.Locations)
    Console.WriteLine($"{location.DisplayName}: {location.Latitude}, {location.Longitude}");
```

---

## Auto Apply — `client.AutoApply`

Automate job applications with form field discovery and filling.

### Interactive session flow

```csharp
// Start a session
var session = await client.AutoApply.StartSessionAsync(job.ApplyUrl);

Console.WriteLine($"Provider: {session.ProviderDisplayName}");
Console.WriteLine($"Fields: {session.Fields.Count}");

// Fill in fields — Type must match the discovered FormFieldInfo.Type
var answers = new List<FieldAnswer>
{
    new() { FieldId = "first_name", Type = "text", Value = "John" },
    new() { FieldId = "last_name", Type = "text", Value = "Doe" },
    new() { FieldId = "email", Type = "text", Value = "john@example.com" },
};

var result = await client.AutoApply.SetAnswersAsync(session.SessionId, answers);

if (result.IsTerminal)
    Console.WriteLine("Application submitted!");

// Clean up
await client.AutoApply.EndSessionAsync(session.SessionId);
```

### Stored profiles & one-shot runs

Save an applicant profile once, then run the whole flow end-to-end against any apply URL.

```csharp
// Create a reusable profile
var profile = await client.AutoApply.CreateProfileAsync(new AutoApplyProfileRequest
{
    Name = "Primary",
    FirstName = "John",
    LastName = "Doe",
    Email = "john@example.com",
    Phone = "+1 555 0100",
    WorkAuthorization = "us_citizen"
});

// Run the full auto-apply flow against a job in one call
var run = await client.AutoApply.RunAsync(profile.Id, job.ApplyUrl);
Console.WriteLine($"Success={run.Success} status={run.Status} fields={run.FieldsFilled}");

// Manage profiles
var all = await client.AutoApply.ListProfilesAsync();
await client.AutoApply.UpdateProfileAsync(profile.Id, new AutoApplyProfileRequest { /* ... */ });
await client.AutoApply.DeleteProfileAsync(profile.Id);
```

---

## Error Handling

```csharp
using Jobo.Enterprise.Client.Exceptions;

try
{
    var results = await client.Search.SearchAsync(q: "engineer");
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
```

## Supported ATS Sources (45+)

| Category           | Sources                                                                                                                                       |
| ------------------ | --------------------------------------------------------------------------------------------------------------------------------------------- |
| **Enterprise ATS** | `workday`, `smartrecruiters`, `icims`, `successfactors`, `oraclecloud`, `taleo`, `dayforce`, `csod`, `adp`, `ultipro`, `paycom`               |
| **Tech & Startup** | `greenhouse`, `lever_co`, `ashby`, `workable`, `workable_jobs`, `rippling`, `polymer`, `gem`, `pinpoint`, `homerun`                           |
| **Mid-Market**     | `bamboohr`, `breezy`, `jazzhr`, `recruitee`, `personio`, `jobvite`, `teamtailor`, `comeet`, `trakstar`, `zoho`                                |
| **SMB & Niche**    | `gohire`, `recooty`, `applicantpro`, `hiringthing`, `careerplug`, `hirehive`, `kula`, `careerpuck`, `talnet`, `jobscore`                      |
| **Specialized**    | `freshteam`, `isolved`, `joincom`, `eightfold`, `phenompeople`                                                                                |

## Configuration

| Property  | Default                       | Description          |
| --------- | ----------------------------- | -------------------- |
| `ApiKey`  | _required_                    | Your API key         |
| `BaseUrl` | `https://jobs-api.jobo.world` | API base URL         |
| `Timeout` | `00:00:30`                    | Request timeout      |

## Custom HttpClient

```csharp
var httpClient = new HttpClient { BaseAddress = new Uri("https://jobs-api.jobo.world") };
httpClient.DefaultRequestHeaders.Add("X-Api-Key", "your-api-key");

var client = new JoboClient(httpClient);
// client.Feed, client.Search, client.Companies, client.Locations, client.AutoApply are all available
```

## Use Cases

- **Build a job board** — Search and display jobs from 45+ ATS platforms
- **Job aggregator** — Bulk-sync millions of listings with the feed endpoint
- **ATS data pipeline** — Pull jobs from Greenhouse, Lever, Workday, etc. into your data warehouse
- **Recruitment tools** — Power candidate-facing job search experiences
- **Auto-apply automation** — Automate job applications at scale
- **Location intelligence** — Geocode and normalize job locations

## Target Frameworks

- .NET 8.0
- .NET 6.0

## Links

- **Website** — [jobo.world/enterprise](https://jobo.world/enterprise/)
- **Get API Key** — [enterprise.jobo.world/api-keys](https://enterprise.jobo.world/api-keys)
- **GitHub** — [github.com/Prakkie91/jobo-dotnet](https://github.com/Prakkie91/jobo-dotnet)
- **NuGet** — [nuget.org/packages/Jobo.Enterprise.Client](https://www.nuget.org/packages/Jobo.Enterprise.Client)

## License

MIT — see [LICENSE](LICENSE).
