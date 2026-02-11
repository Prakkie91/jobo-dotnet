using System.Text.Json.Serialization;

namespace Jobo.Enterprise.Client.Models;

/// <summary>
/// A job listing returned by the API.
/// </summary>
public sealed class Job
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("title")] public string Title { get; set; } = string.Empty;
    [JsonPropertyName("company")] public JobCompany Company { get; set; } = new();
    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;
    [JsonPropertyName("listing_url")] public string ListingUrl { get; set; } = string.Empty;
    [JsonPropertyName("apply_url")] public string ApplyUrl { get; set; } = string.Empty;
    [JsonPropertyName("locations")] public List<JobLocation> Locations { get; set; } = new();
    [JsonPropertyName("compensation")] public JobCompensation? Compensation { get; set; }
    [JsonPropertyName("employment_type")] public string? EmploymentType { get; set; }
    [JsonPropertyName("workplace_type")] public string? WorkplaceType { get; set; }
    [JsonPropertyName("experience_level")] public string? ExperienceLevel { get; set; }
    [JsonPropertyName("source")] public string Source { get; set; } = string.Empty;
    [JsonPropertyName("source_id")] public string SourceId { get; set; } = string.Empty;
    [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
    [JsonPropertyName("date_posted")] public DateTime? DatePosted { get; set; }
    [JsonPropertyName("valid_through")] public DateTime? ValidThrough { get; set; }
    [JsonPropertyName("is_remote")] public bool IsRemote { get; set; }
}

/// <summary>
/// Company associated with a job listing.
/// </summary>
public sealed class JobCompany
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Geographic location of a job.
/// </summary>
public sealed class JobLocation
{
    [JsonPropertyName("location")] public string? Location { get; set; }
    [JsonPropertyName("city")] public string? City { get; set; }
    [JsonPropertyName("state")] public string? State { get; set; }
    [JsonPropertyName("country")] public string? Country { get; set; }
    [JsonPropertyName("latitude")] public double? Latitude { get; set; }
    [JsonPropertyName("longitude")] public double? Longitude { get; set; }
}

/// <summary>
/// Compensation details for a job.
/// </summary>
public sealed class JobCompensation
{
    [JsonPropertyName("min")] public decimal? Min { get; set; }
    [JsonPropertyName("max")] public decimal? Max { get; set; }
    [JsonPropertyName("currency")] public string? Currency { get; set; }
    [JsonPropertyName("period")] public string? Period { get; set; }
    [JsonPropertyName("raw_text")] public string? RawText { get; set; }
    [JsonPropertyName("is_estimated")] public bool IsEstimated { get; set; }
}
