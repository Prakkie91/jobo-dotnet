namespace Jobo.Enterprise.Client;

/// <summary>
/// Client for the Jobo Enterprise API.
/// <para>
/// Access feature-specific sub-clients via properties:
/// <list type="bullet">
///   <item><see cref="Feed"/> — Bulk job feed with cursor-based pagination</item>
///   <item><see cref="Search"/> — Full-text job search with filters</item>
///   <item><see cref="Locations"/> — Geocoding and location resolution</item>
///   <item><see cref="AutoApply"/> — Automated job application form filling</item>
/// </list>
/// </para>
/// Implements <see cref="IDisposable"/> to clean up the underlying <see cref="HttpClient"/>.
/// </summary>
public sealed class JoboClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;

    /// <summary>
    /// Bulk job feed with cursor-based pagination.
    /// </summary>
    public JobsFeedClient Feed { get; }

    /// <summary>
    /// Full-text job search with filters and pagination.
    /// </summary>
    public JobsSearchClient Search { get; }

    /// <summary>
    /// Geocoding and location resolution.
    /// </summary>
    public LocationsClient Locations { get; }

    /// <summary>
    /// Automated job application form filling.
    /// </summary>
    public AutoApplyClient AutoApply { get; }

    /// <summary>
    /// Creates a new <see cref="JoboClient"/> with the specified options.
    /// </summary>
    public JoboClient(JoboClientOptions options)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(options.BaseUrl.TrimEnd('/')),
            Timeout = options.Timeout
        };
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", options.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "jobo-dotnet/2.0.0");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _ownsHttpClient = true;

        Feed = new JobsFeedClient(_httpClient);
        Search = new JobsSearchClient(_httpClient);
        Locations = new LocationsClient(_httpClient);
        AutoApply = new AutoApplyClient(_httpClient);
    }

    /// <summary>
    /// Creates a new <see cref="JoboClient"/> using an existing <see cref="HttpClient"/>.
    /// The caller is responsible for configuring headers and base address.
    /// </summary>
    public JoboClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _ownsHttpClient = false;

        Feed = new JobsFeedClient(_httpClient);
        Search = new JobsSearchClient(_httpClient);
        Locations = new LocationsClient(_httpClient);
        AutoApply = new AutoApplyClient(_httpClient);
    }

    public void Dispose()
    {
        if (_ownsHttpClient)
            _httpClient.Dispose();
    }
}
