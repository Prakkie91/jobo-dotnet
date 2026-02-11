using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Jobo.Enterprise.Client.Exceptions;
using Jobo.Enterprise.Client.Models;

namespace Jobo.Enterprise.Client;

/// <summary>
/// Client for the Jobo Enterprise Jobs API.
/// Implements <see cref="IDisposable"/> to clean up the underlying <see cref="HttpClient"/>.
/// </summary>
public sealed class JoboClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// Creates a new <see cref="JoboClient"/> with the specified options.
    /// </summary>
    public JoboClient(JoboClientOptions options)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(options.BaseUrl.TrimEnd('/')),
            Timeout = options.Timeout
        };
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", options.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "jobo-dotnet/1.0.0");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _ownsHttpClient = true;
    }

    /// <summary>
    /// Creates a new <see cref="JoboClient"/> using an existing <see cref="HttpClient"/>.
    /// The caller is responsible for configuring headers and base address.
    /// </summary>
    public JoboClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _ownsHttpClient = false;
    }

    // ── Feed endpoints ──────────────────────────────────────────────

    /// <summary>
    /// Fetch a single batch of jobs from the feed.
    /// </summary>
    /// <param name="request">Feed request with filters, cursor, and batch size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="JobFeedResponse"/> with jobs, cursor, and pagination flag.</returns>
    public async Task<JobFeedResponse> GetJobsFeedAsync(
        JobFeedRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/feed/jobs", request, JsonOptions, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<JobFeedResponse>(JsonOptions, cancellationToken)
               ?? new JobFeedResponse();
    }

    /// <summary>
    /// Enumerate all jobs from the feed, automatically handling cursor-based pagination.
    /// </summary>
    public async IAsyncEnumerable<Job> EnumerateJobsFeedAsync(
        JobFeedRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = null;
        while (true)
        {
            request.Cursor = cursor;
            var response = await GetJobsFeedAsync(request, cancellationToken);
            foreach (var job in response.Jobs)
                yield return job;

            if (!response.HasMore) break;
            cursor = response.NextCursor;
        }
    }

    /// <summary>
    /// Fetch a single batch of expired job IDs.
    /// </summary>
    /// <param name="expiredSince">UTC timestamp. Max 7 days in the past.</param>
    /// <param name="cursor">Pagination cursor from a previous response.</param>
    /// <param name="batchSize">Number of IDs per batch (1–10000). Defaults to 1000.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ExpiredJobIdsResponse> GetExpiredJobIdsAsync(
        DateTime expiredSince,
        string? cursor = null,
        int batchSize = 1000,
        CancellationToken cancellationToken = default)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["expired_since"] = expiredSince.ToUniversalTime().ToString("O");
        query["batch_size"] = batchSize.ToString();
        if (!string.IsNullOrEmpty(cursor))
            query["cursor"] = cursor;

        var response = await _httpClient.GetAsync($"/api/feed/jobs/expired?{query}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ExpiredJobIdsResponse>(JsonOptions, cancellationToken)
               ?? new ExpiredJobIdsResponse();
    }

    /// <summary>
    /// Enumerate all expired job IDs, automatically handling cursor-based pagination.
    /// </summary>
    public async IAsyncEnumerable<Guid> EnumerateExpiredJobIdsAsync(
        DateTime expiredSince,
        int batchSize = 1000,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = null;
        while (true)
        {
            var response = await GetExpiredJobIdsAsync(expiredSince, cursor, batchSize, cancellationToken);
            foreach (var id in response.JobIds)
                yield return id;

            if (!response.HasMore) break;
            cursor = response.NextCursor;
        }
    }

    // ── Search endpoints ────────────────────────────────────────────

    /// <summary>
    /// Search jobs using simple query parameters (GET /api/jobs).
    /// </summary>
    public async Task<JobSearchResponse> SearchJobsAsync(
        string? q = null,
        string? location = null,
        string? sources = null,
        bool? remote = null,
        DateTime? postedAfter = null,
        int page = 1,
        int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        if (!string.IsNullOrEmpty(q)) query["q"] = q;
        if (!string.IsNullOrEmpty(location)) query["location"] = location;
        if (!string.IsNullOrEmpty(sources)) query["sources"] = sources;
        if (remote.HasValue) query["remote"] = remote.Value.ToString().ToLowerInvariant();
        if (postedAfter.HasValue) query["posted_after"] = postedAfter.Value.ToUniversalTime().ToString("O");
        query["page"] = page.ToString();
        query["page_size"] = pageSize.ToString();

        var response = await _httpClient.GetAsync($"/api/jobs?{query}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<JobSearchResponse>(JsonOptions, cancellationToken)
               ?? new JobSearchResponse();
    }

    /// <summary>
    /// Search jobs using the advanced body-based endpoint (POST /api/jobs/search).
    /// </summary>
    public async Task<JobSearchResponse> SearchJobsAdvancedAsync(
        JobSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/jobs/search", request, JsonOptions, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<JobSearchResponse>(JsonOptions, cancellationToken)
               ?? new JobSearchResponse();
    }

    /// <summary>
    /// Enumerate all search results, automatically handling page-based pagination.
    /// </summary>
    public async IAsyncEnumerable<Job> EnumerateSearchJobsAsync(
        JobSearchRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;
        while (true)
        {
            request.Page = page;
            var response = await SearchJobsAdvancedAsync(request, cancellationToken);
            foreach (var job in response.Jobs)
                yield return job;

            if (page >= response.TotalPages) break;
            page++;
        }
    }

    // ── Error handling ──────────────────────────────────────────────

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode) return;

        var body = await response.Content.ReadAsStringAsync(ct);
        var status = (int)response.StatusCode;
        var detail = TryExtractDetail(body);
        var message = !string.IsNullOrEmpty(detail) ? $"HTTP {status}: {detail}" : $"HTTP {status}";

        throw status switch
        {
            401 => new JoboAuthenticationException(message, status, detail, body),
            429 => new JoboRateLimitException(
                message,
                response.Headers.TryGetValues("Retry-After", out var values)
                    ? int.TryParse(values.FirstOrDefault(), out var ra) ? ra : null
                    : null,
                status, detail, body),
            400 => new JoboValidationException(message, status, detail, body),
            >= 500 => new JoboServerException(message, status, detail, body),
            _ => new JoboException(message, status, detail, body)
        };
    }

    private static string? TryExtractDetail(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.TryGetProperty("detail", out var detail) ? detail.GetString() : null;
        }
        catch
        {
            return body;
        }
    }

    public void Dispose()
    {
        if (_ownsHttpClient)
            _httpClient.Dispose();
    }
}
