using System.Text.Json.Serialization;

namespace Jobo.Enterprise.Client.Models;

/// <summary>
/// Request body for the advanced search endpoint (POST /api/jobs/search).
/// </summary>
public sealed class JobSearchRequest
{
    [JsonPropertyName("queries")] public List<string>? Queries { get; set; }
    [JsonPropertyName("locations")] public List<string>? Locations { get; set; }
    [JsonPropertyName("sources")] public List<string>? Sources { get; set; }
    [JsonPropertyName("is_remote")] public bool? IsRemote { get; set; }
    [JsonPropertyName("posted_after")] public DateTime? PostedAfter { get; set; }
    [JsonPropertyName("page")] public int Page { get; set; } = 1;
    [JsonPropertyName("page_size")] public int PageSize { get; set; } = 25;
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
}
