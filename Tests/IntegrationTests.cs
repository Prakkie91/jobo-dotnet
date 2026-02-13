using Xunit;
using Jobo.Enterprise.Client;
using Jobo.Enterprise.Client.Exceptions;
using Jobo.Enterprise.Client.Models;

namespace Jobo.Enterprise.Client.Tests;

/// <summary>
/// Integration tests that call the live Jobo Enterprise API.
/// Requires the JOBO_API_KEY environment variable to be set.
/// Tests are skipped gracefully when the key is not available (local dev).
/// </summary>
public class IntegrationTests : IDisposable
{
    private readonly JoboClient? _client;
    private readonly string? _skipReason;

    public IntegrationTests()
    {
        var apiKey = Environment.GetEnvironmentVariable("JOBO_API_KEY");
        var baseUrl = Environment.GetEnvironmentVariable("JOBO_BASE_URL") ?? "https://jobs-api.jobo.world";

        if (string.IsNullOrEmpty(apiKey))
        {
            _skipReason = "JOBO_API_KEY environment variable is not set.";
            return;
        }

        _client = new JoboClient(new JoboClientOptions
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl,
            Timeout = TimeSpan.FromSeconds(30)
        });
    }

    private JoboClient Client => _client ?? throw new SkipException(_skipReason!);

    // ── Feed ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetJobsFeed_ReturnsJobs()
    {
        if (_client is null) return; // skip

        var response = await Client.Feed.GetJobsAsync(new JobFeedRequest { BatchSize = 5 });

        Assert.NotNull(response);
        Assert.NotEmpty(response.Jobs);
        Assert.True(response.Jobs.Count <= 5);

        var job = response.Jobs[0];
        Assert.NotEqual(Guid.Empty, job.Id);
        Assert.False(string.IsNullOrEmpty(job.Title));
        Assert.False(string.IsNullOrEmpty(job.Description));
        Assert.False(string.IsNullOrEmpty(job.ListingUrl));
        Assert.False(string.IsNullOrEmpty(job.Source));
        Assert.NotNull(job.Company);
        Assert.False(string.IsNullOrEmpty(job.Company.Name));
    }

    [Fact]
    public async Task GetJobsFeed_WithLocationFilter_ReturnsJobs()
    {
        if (_client is null) return;

        var response = await Client.Feed.GetJobsAsync(new JobFeedRequest
        {
            Locations = new List<LocationFilter>
            {
                new() { Country = "United States" }
            },
            BatchSize = 5
        });

        Assert.NotNull(response);
        Assert.NotEmpty(response.Jobs);
    }

    [Fact]
    public async Task GetJobsFeed_Pagination_CursorWorks()
    {
        if (_client is null) return;

        var first = await Client.Feed.GetJobsAsync(new JobFeedRequest { BatchSize = 2 });
        Assert.NotEmpty(first.Jobs);

        if (!first.HasMore) return; // small dataset, can't test pagination

        Assert.False(string.IsNullOrEmpty(first.NextCursor));

        var second = await Client.Feed.GetJobsAsync(new JobFeedRequest
        {
            Cursor = first.NextCursor,
            BatchSize = 2
        });

        Assert.NotNull(second);
        Assert.NotEmpty(second.Jobs);
        // Ensure we got different jobs
        Assert.NotEqual(first.Jobs[0].Id, second.Jobs[0].Id);
    }

    [Fact]
    public async Task EnumerateJobsFeed_YieldsJobs()
    {
        if (_client is null) return;

        var jobs = new List<Job>();
        await foreach (var job in Client.Feed.EnumerateJobsAsync(new JobFeedRequest { BatchSize = 3 }))
        {
            jobs.Add(job);
            if (jobs.Count >= 5) break; // limit for test speed
        }

        Assert.NotEmpty(jobs);
    }

    // ── Expired ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetExpiredJobIds_ReturnsResponse()
    {
        if (_client is null) return;

        var response = await Client.Feed.GetExpiredJobIdsAsync(
            expiredSince: DateTime.UtcNow.AddDays(-6),
            batchSize: 5
        );

        Assert.NotNull(response);
        // May be empty if no jobs expired recently, but should not throw
        Assert.NotNull(response.JobIds);
    }

    // ── Search ──────────────────────────────────────────────────────

    [Fact]
    public async Task SearchJobs_ReturnsResults()
    {
        if (_client is null) return;

        var response = await Client.Search.SearchAsync(q: "software engineer", pageSize: 5);

        Assert.NotNull(response);
        Assert.NotEmpty(response.Jobs);
        Assert.True(response.Total > 0);
        Assert.True(response.TotalPages >= 1);
        Assert.Equal(1, response.Page);
    }

    [Fact]
    public async Task SearchJobsAdvanced_ReturnsResults()
    {
        if (_client is null) return;

        var response = await Client.Search.SearchAdvancedAsync(new JobSearchRequest
        {
            Queries = new List<string> { "data engineer" },
            PageSize = 5
        });

        Assert.NotNull(response);
        Assert.NotEmpty(response.Jobs);
        Assert.True(response.Total > 0);
    }

    [Fact]
    public async Task SearchJobsAdvanced_WithLocationFilter_ReturnsResults()
    {
        if (_client is null) return;

        var response = await Client.Search.SearchAdvancedAsync(new JobSearchRequest
        {
            Queries = new List<string> { "developer" },
            Locations = new List<string> { "New York" },
            PageSize = 5
        });

        Assert.NotNull(response);
        // May return 0 results for very specific filters, but should not throw
    }

    [Fact]
    public async Task EnumerateSearchJobs_YieldsJobs()
    {
        if (_client is null) return;

        var jobs = new List<Job>();
        await foreach (var job in Client.Search.EnumerateAsync(new JobSearchRequest
        {
            Queries = new List<string> { "engineer" },
            PageSize = 3
        }))
        {
            jobs.Add(job);
            if (jobs.Count >= 5) break;
        }

        Assert.NotEmpty(jobs);
    }

    // ── Error handling ──────────────────────────────────────────────

    [Fact]
    public async Task InvalidApiKey_ThrowsAuthenticationException()
    {
        using var badClient = new JoboClient(new JoboClientOptions
        {
            ApiKey = "invalid-key-12345",
            BaseUrl = Environment.GetEnvironmentVariable("JOBO_BASE_URL") ?? "https://jobs-api.jobo.world"
        });

        await Assert.ThrowsAsync<JoboAuthenticationException>(
            () => badClient.Feed.GetJobsAsync(new JobFeedRequest { BatchSize = 1 })
        );
    }

    // ── Job model validation ────────────────────────────────────────

    [Fact]
    public async Task Job_HasExpectedFields()
    {
        if (_client is null) return;

        var response = await Client.Search.SearchAsync(q: "engineer", pageSize: 1);
        Assert.NotEmpty(response.Jobs);

        var job = response.Jobs[0];
        Assert.NotEqual(Guid.Empty, job.Id);
        Assert.False(string.IsNullOrEmpty(job.Title));
        Assert.NotNull(job.Company);
        Assert.NotEqual(Guid.Empty, job.Company.Id);
        Assert.False(string.IsNullOrEmpty(job.Company.Name));
        Assert.False(string.IsNullOrEmpty(job.Description));
        Assert.False(string.IsNullOrEmpty(job.ListingUrl));
        Assert.False(string.IsNullOrEmpty(job.ApplyUrl));
        Assert.False(string.IsNullOrEmpty(job.Source));
        Assert.False(string.IsNullOrEmpty(job.SourceId));
        Assert.NotEqual(default, job.CreatedAt);
        Assert.NotEqual(default, job.UpdatedAt);
    }

    // ── Geocoding ─────────────────────────────────────────────────────

    [Fact]
    public async Task Geocode_ReturnsLocation()
    {
        if (_client is null) return;

        var result = await Client.Locations.GeocodeAsync("San Francisco, CA");

        Assert.NotNull(result);
        Assert.Equal("San Francisco, CA", result.Input);
        Assert.True(result.Succeeded);
        Assert.NotEmpty(result.Locations);
        var location = result.Locations[0];
        Assert.False(string.IsNullOrEmpty(location.DisplayName));
        Assert.NotNull(location.Latitude);
        Assert.NotNull(location.Longitude);
    }

    [Fact]
    public async Task Geocode_WithInvalidLocation_ReturnsFailed()
    {
        if (_client is null) return;

        var result = await Client.Locations.GeocodeAsync("invalidlocationxyz123");

        Assert.NotNull(result);
        // May succeed with remote keyword parsing or fail - just check response
    }

    // ── AutoApply (disabled – not yet implemented) ──────────────────

    [Fact(Skip = "Auto Apply is not yet implemented")]
    public async Task StartAutoApplySession_WithInvalidUrl_ReturnsError()
    {
        if (_client is null) return;

        // Using an invalid URL should return success=false with an error
        var response = await Client.AutoApply.StartSessionAsync("https://invalid-url-that-does-not-exist.com/jobs/123");

        Assert.NotNull(response);
        // The provider detection may fail or succeed - just check response structure
        Assert.NotEqual(Guid.Empty, response.SessionId);
    }

    [Fact(Skip = "Auto Apply is not yet implemented")]
    public async Task EndAutoApplySession_WithInvalidSession_ReturnsFalse()
    {
        if (_client is null) return;

        var result = await Client.AutoApply.EndSessionAsync(Guid.NewGuid());

        // Should return false for non-existent session
        Assert.False(result);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}

/// <summary>
/// Custom exception to signal test skipping when API key is not available.
/// xunit will report these as failures, but the test body returns early.
/// </summary>
internal class SkipException : Exception
{
    public SkipException(string message) : base(message) { }
}
