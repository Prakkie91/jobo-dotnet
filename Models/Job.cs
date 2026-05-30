using System.Text.Json.Serialization;

namespace Jobo.Enterprise.Client.Models;

/// <summary>
/// A job listing returned by the API.
/// </summary>
public sealed class Job
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("title")] public string Title { get; set; } = string.Empty;
    [JsonPropertyName("normalized_title")] public string? NormalizedTitle { get; set; }
    [JsonPropertyName("company")] public JobCompany Company { get; set; } = new();
    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;
    [JsonPropertyName("summary")] public string? Summary { get; set; }
    [JsonPropertyName("listing_url")] public string ListingUrl { get; set; } = string.Empty;
    [JsonPropertyName("apply_url")] public string ApplyUrl { get; set; } = string.Empty;
    [JsonPropertyName("locations")] public List<JobLocation> Locations { get; set; } = new();
    [JsonPropertyName("compensation")] public JobCompensation? Compensation { get; set; }
    [JsonPropertyName("employment_type")] public string? EmploymentType { get; set; }
    [JsonPropertyName("workplace_type")] public string? WorkplaceType { get; set; }
    [JsonPropertyName("experience_level")] public string? ExperienceLevel { get; set; }
    [JsonPropertyName("source")] public string Source { get; set; } = string.Empty;
    [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
    [JsonPropertyName("date_posted")] public DateTime? DatePosted { get; set; }
    [JsonPropertyName("valid_through")] public DateTime? ValidThrough { get; set; }
    [JsonPropertyName("qualifications")] public JobQualifications Qualifications { get; set; } = new();
    [JsonPropertyName("responsibilities")] public List<string> Responsibilities { get; set; } = new();
    [JsonPropertyName("benefits")] public List<string> Benefits { get; set; } = new();
    [JsonPropertyName("is_work_auth_required")] public bool? IsWorkAuthRequired { get; set; }
    [JsonPropertyName("is_h1b_sponsor")] public bool? IsH1bSponsor { get; set; }
    [JsonPropertyName("is_clearance_required")] public bool? IsClearanceRequired { get; set; }
}

/// <summary>
/// Company associated with a job listing.
/// </summary>
public sealed class JobCompany
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("website")] public string? Website { get; set; }
    [JsonPropertyName("logo_url")] public string? LogoUrl { get; set; }
    [JsonPropertyName("summary")] public string? Summary { get; set; }
    [JsonPropertyName("industries")] public List<string> Industries { get; set; } = new();
    [JsonPropertyName("categories")] public List<string> Categories { get; set; } = new();
    [JsonPropertyName("linkedin_url")] public string? LinkedinUrl { get; set; }
    [JsonPropertyName("crunchbase_url")] public string? CrunchbaseUrl { get; set; }
    [JsonPropertyName("details_url")] public string? DetailsUrl { get; set; }
}

/// <summary>
/// Geographic location of a job.
/// </summary>
public sealed class JobLocation
{
    [JsonPropertyName("location")] public string? Location { get; set; }
    [JsonPropertyName("city")] public string? City { get; set; }
    [JsonPropertyName("region")] public string? Region { get; set; }
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
}

/// <summary>
/// Structured qualifications split into must-have and preferred buckets.
/// </summary>
public sealed class JobQualifications
{
    [JsonPropertyName("must_have")] public QualificationBucket MustHave { get; set; } = new();
    [JsonPropertyName("preferred")] public QualificationBucket Preferred { get; set; } = new();
}

/// <summary>
/// A qualification bucket: education, certifications, and typed skills.
/// </summary>
public sealed class QualificationBucket
{
    [JsonPropertyName("education")] public List<string> Education { get; set; } = new();
    [JsonPropertyName("certifications")] public List<string> Certifications { get; set; } = new();
    [JsonPropertyName("skills")] public List<QualificationSkill> Skills { get; set; } = new();
}

/// <summary>
/// A typed skill with a name and classification ("hard" or "soft").
/// </summary>
public sealed class QualificationSkill
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("type")] public string Type { get; set; } = "hard";
}
