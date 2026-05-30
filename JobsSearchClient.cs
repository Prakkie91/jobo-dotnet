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
    /// <param name="q">Free-text search query.</param>
    /// <param name="location">Location string filter.</param>
    /// <param name="sources">Comma-separated source identifiers.</param>
    /// <param name="workModel">Comma-separated work models ("remote", "hybrid", "onsite").</param>
    /// <param name="employmentType">Comma-separated employment types.</param>
    /// <param name="experienceLevel">Comma-separated experience levels.</param>
    /// <param name="postedAfter">Only jobs posted after this UTC timestamp.</param>
    /// <param name="minSalaryUsd">Minimum salary (USD) filter.</param>
    /// <param name="maxSalaryUsd">Maximum salary (USD) filter.</param>
    /// <param name="skills">Comma-separated required skills.</param>
    /// <param name="industries">Comma-separated company industries.</param>
    /// <param name="includeFacets">Comma-separated facets to compute. Pass "" to skip facets entirely.</param>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Results per page (1–100).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JobSearchResponse> SearchAsync(
        string? q = null,
        string? location = null,
        string? sources = null,
        string? workModel = null,
        string? employmentType = null,
        string? experienceLevel = null,
        DateTime? postedAfter = null,
        int? minSalaryUsd = null,
        int? maxSalaryUsd = null,
        string? skills = null,
        string? industries = null,
        string? includeFacets = null,
        int page = 1,
        int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        if (!string.IsNullOrEmpty(q)) query["q"] = q;
        if (!string.IsNullOrEmpty(location)) query["location"] = location;
        if (!string.IsNullOrEmpty(sources)) query["sources"] = sources;
        if (!string.IsNullOrEmpty(workModel)) query["work_model"] = workModel;
        if (!string.IsNullOrEmpty(employmentType)) query["employment_type"] = employmentType;
        if (!string.IsNullOrEmpty(experienceLevel)) query["experience_level"] = experienceLevel;
        if (postedAfter.HasValue) query["posted_after"] = postedAfter.Value.ToUniversalTime().ToString("O");
        if (minSalaryUsd.HasValue) query["min_salary_usd"] = minSalaryUsd.Value.ToString();
        if (maxSalaryUsd.HasValue) query["max_salary_usd"] = maxSalaryUsd.Value.ToString();
        if (!string.IsNullOrEmpty(skills)) query["skills"] = skills;
        if (!string.IsNullOrEmpty(industries)) query["industries"] = industries;
        if (includeFacets is not null) query["include_facets"] = includeFacets;
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
    /// Facets are disabled while paginating.
    /// </summary>
    public async IAsyncEnumerable<Job> EnumerateAsync(
        JobSearchRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        request.IncludeFacets = new List<string>();
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
