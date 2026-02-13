using System.Text.Json.Serialization;

namespace Jobo.Enterprise.Client.Models;

/// <summary>
/// Response from the geocode endpoint.
/// </summary>
public sealed class GeocodeResultItem
{
    /// <summary>
    /// The original input location string.
    /// </summary>
    [JsonPropertyName("input")] public string Input { get; set; } = string.Empty;

    /// <summary>
    /// Whether the location was successfully geocoded.
    /// </summary>
    [JsonPropertyName("succeeded")] public bool Succeeded { get; set; }

    /// <summary>
    /// One or more resolved locations for this input.
    /// </summary>
    [JsonPropertyName("locations")] public List<GeocodedLocation> Locations { get; set; } = new();

    /// <summary>
    /// How the location was resolved: cache, pattern_parse, geocoder, llm, remote_keyword.
    /// </summary>
    [JsonPropertyName("method")] public string? Method { get; set; }

    /// <summary>
    /// Error message if geocoding failed.
    /// </summary>
    [JsonPropertyName("error")] public string? Error { get; set; }
}

/// <summary>
/// A resolved/geocoded location.
/// </summary>
public sealed class GeocodedLocation
{
    /// <summary>
    /// Display name of the location.
    /// </summary>
    [JsonPropertyName("display_name")] public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// City name.
    /// </summary>
    [JsonPropertyName("city")] public string? City { get; set; }

    /// <summary>
    /// State/region name.
    /// </summary>
    [JsonPropertyName("region")] public string? Region { get; set; }

    /// <summary>
    /// Country name.
    /// </summary>
    [JsonPropertyName("country")] public string? Country { get; set; }

    /// <summary>
    /// Two-letter country code.
    /// </summary>
    [JsonPropertyName("country_code")] public string? CountryCode { get; set; }

    /// <summary>
    /// Latitude coordinate.
    /// </summary>
    [JsonPropertyName("latitude")] public double? Latitude { get; set; }

    /// <summary>
    /// Longitude coordinate.
    /// </summary>
    [JsonPropertyName("longitude")] public double? Longitude { get; set; }
}
