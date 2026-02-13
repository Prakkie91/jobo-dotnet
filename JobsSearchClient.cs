using System.Runtime.CompilerServices;
using System.Web;
using Jobo.Enterprise.Client.Models;

namespace Jobo.Enterprise.Client;

/// <summary>
/// Sub-client for the Jobs Search endpoints (GET /api/jobs, POST /api/jobs/search).
/// Access via <see cref="JoboClient.Search"/>.
/// </summary>
public sealed class JobsSearchClient : JoboClientBase
{
    internal JobsSearchClient(HttpClient httpClient) : base(httpClient) { }

    /// <summary>
    /// Search jobs using simple query parameters (GET /api/jobs).
    /// </summary>
    public async Task<JobSearchResponse> SearchAsync(
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

        return await GetAsync<JobSearchResponse>($"/api/jobs?{query}", cancellationToken);
    }

    /// <summary>
    /// Search jobs using the advanced body-based endpoint (POST /api/jobs/search).
    /// </summary>
    public async Task<JobSearchResponse> SearchAdvancedAsync(
        JobSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<JobSearchResponse>("/api/jobs/search", request, cancellationToken);
    }

    /// <summary>
    /// Enumerate all search results, automatically handling page-based pagination.
    /// </summary>
    public async IAsyncEnumerable<Job> EnumerateAsync(
        JobSearchRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var page = 1;
        while (true)
        {
            request.Page = page;
            var response = await SearchAdvancedAsync(request, cancellationToken);
            foreach (var job in response.Jobs)
                yield return job;

            if (page >= response.TotalPages) break;
            page++;
        }
    }
}
