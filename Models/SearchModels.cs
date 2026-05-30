using System.Text.Json.Serialization;

namespace Jobo.Enterprise.Client.Models;

/// <summary>
/// Include / exclude list filter. Items are matched case-insensitively.
/// </summary>
public sealed class InclusionExclusionFilter
{
    [JsonPropertyName("include")] public List<string>? Include { get; set; }
    [JsonPropertyName("exclude")] public List<string>? Exclude { get; set; }
}

/// <summary>
/// Numeric range filter with optional min / max bounds.
/// </summary>
public sealed class RangeFilter
{
    [JsonPropertyName("min")] public int? Min { get; set; }
    [JsonPropertyName("max")] public int? Max { get; set; }
}

/// <summary>
/// Request body for the advanced search endpoint (POST /api/jobs/search).
/// </summary>
public sealed class JobSearchRequest
{
    [JsonPropertyName("queries")] public List<string>? Queries { get; set; }
    [JsonPropertyName("locations")] public List<string>? Locations { get; set; }
    [JsonPropertyName("sources")] public List<string>? Sources { get; set; }
    [JsonPropertyName("skills")] public InclusionExclusionFilter? Skills { get; set; }
    [JsonPropertyName("companies")] public InclusionExclusionFilter? Companies { get; set; }
    [JsonPropertyName("industries")] public InclusionExclusionFilter? Industries { get; set; }
    [JsonPropertyName("work_models")] public List<string>? WorkModels { get; set; }
    [JsonPropertyName("employment_types")] public List<string>? EmploymentTypes { get; set; }
    [JsonPropertyName("experience_levels")] public List<string>? ExperienceLevels { get; set; }
    [JsonPropertyName("salary_usd")] public RangeFilter? SalaryUsd { get; set; }
    [JsonPropertyName("posted_after")] public DateTime? PostedAfter { get; set; }
    [JsonPropertyName("page")] public int Page { get; set; } = 1;
    [JsonPropertyName("page_size")] public int PageSize { get; set; } = 25;
    [JsonPropertyName("include_facets")] public List<string>? IncludeFacets { get; set; }
}

/// <summary>
/// An aggregated facet count.
/// </summary>
public sealed class JobFacet
{
    [JsonPropertyName("key")] public string Key { get; set; } = string.Empty;
    [JsonPropertyName("count")] public long Count { get; set; }
}

/// <summary>
/// Response from the search endpoints.
/// </summary>
public sealed class JobSearchResponse
{
    [JsonPropertyName("jobs")] public List<Job> Jobs { get; set; } = new();
    [JsonPropertyName("total")] public long Total { get; set; }
    [JsonPropertyName("page")] public int Page { get; set; }
    [JsonPropertyName("page_size")] public int PageSize { get; set; }
    [JsonPropertyName("total_pages")] public int TotalPages { get; set; }
    [JsonPropertyName("facets")] public Dictionary<string, List<JobFacet>> Facets { get; set; } = new();
}
