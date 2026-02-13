using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jobo.Enterprise.Client.Exceptions;

namespace Jobo.Enterprise.Client;

/// <summary>
/// Shared HTTP infrastructure for all Jobo sub-clients.
/// </summary>
public abstract class JoboClientBase
{
    internal readonly HttpClient HttpClient;

    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    internal JoboClientBase(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    internal async Task<T> PostAsync<T>(string path, object body, CancellationToken ct) where T : new()
    {
        var response = await HttpClient.PostAsJsonAsync(path, body, JsonOptions, ct);
        await EnsureSuccessAsync(response, ct);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct) ?? new T();
    }

    internal async Task<T> GetAsync<T>(string path, CancellationToken ct) where T : new()
    {
        var response = await HttpClient.GetAsync(path, ct);
        await EnsureSuccessAsync(response, ct);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct) ?? new T();
    }

    internal async Task<bool> DeleteAsync(string path, CancellationToken ct)
    {
        var response = await HttpClient.DeleteAsync(path, ct);
        if (response.IsSuccessStatusCode) return true;
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return false;
        await EnsureSuccessAsync(response, ct);
        return false;
    }

    internal static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
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
}
