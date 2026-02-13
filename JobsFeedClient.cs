using System.Runtime.CompilerServices;
using System.Web;
using Jobo.Enterprise.Client.Models;

namespace Jobo.Enterprise.Client;

/// <summary>
/// Sub-client for the Jobs Feed endpoints (POST /api/feed/jobs, GET /api/feed/jobs/expired).
/// Access via <see cref="JoboClient.Feed"/>.
/// </summary>
public sealed class JobsFeedClient : JoboClientBase
{
    internal JobsFeedClient(HttpClient httpClient) : base(httpClient) { }

    /// <summary>
    /// Fetch a single batch of jobs from the feed.
    /// </summary>
    /// <param name="request">Feed request with filters, cursor, and batch size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="JobFeedResponse"/> with jobs, cursor, and pagination flag.</returns>
    public async Task<JobFeedResponse> GetJobsAsync(
        JobFeedRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<JobFeedResponse>("/api/feed/jobs", request, cancellationToken);
    }

    /// <summary>
    /// Enumerate all jobs from the feed, automatically handling cursor-based pagination.
    /// </summary>
    public async IAsyncEnumerable<Job> EnumerateJobsAsync(
        JobFeedRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? cursor = null;
        while (true)
        {
            request.Cursor = cursor;
            var response = await GetJobsAsync(request, cancellationToken);
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

        return await GetAsync<ExpiredJobIdsResponse>($"/api/feed/jobs/expired?{query}", cancellationToken);
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
}
