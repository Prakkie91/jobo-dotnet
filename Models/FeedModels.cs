using System.Text.Json.Serialization;

namespace Jobo.Enterprise.Client.Models;

/// <summary>
/// Structured location filter for the feed endpoint.
/// </summary>
public sealed class LocationFilter
{
    [JsonPropertyName("country")] public string? Country { get; set; }
    [JsonPropertyName("region")] public string? Region { get; set; }
    [JsonPropertyName("city")] public string? City { get; set; }
}

/// <summary>
/// Request body for the jobs feed endpoint (POST /api/feed/jobs).
/// </summary>
public sealed class JobFeedRequest
{
    [JsonPropertyName("locations")] public List<LocationFilter>? Locations { get; set; }
    [JsonPropertyName("sources")] public List<string>? Sources { get; set; }
    [JsonPropertyName("is_remote")] public bool? IsRemote { get; set; }
    [JsonPropertyName("posted_after")] public DateTime? PostedAfter { get; set; }
    [JsonPropertyName("cursor")] public string? Cursor { get; set; }
    [JsonPropertyName("batch_size")] public int BatchSize { get; set; } = 1000;
}

/// <summary>
/// Response from the jobs feed endpoint.
/// </summary>
public sealed class JobFeedResponse
{
    [JsonPropertyName("jobs")] public List<Job> Jobs { get; set; } = new();
    [JsonPropertyName("next_cursor")] public string? NextCursor { get; set; }
    [JsonPropertyName("has_more")] public bool HasMore { get; set; }
}

/// <summary>
/// Response from the expired job IDs endpoint.
/// </summary>
public sealed class ExpiredJobIdsResponse
{
    [JsonPropertyName("job_ids")] public List<Guid> JobIds { get; set; } = new();
    [JsonPropertyName("next_cursor")] public string? NextCursor { get; set; }
    [JsonPropertyName("has_more")] public bool HasMore { get; set; }
}
