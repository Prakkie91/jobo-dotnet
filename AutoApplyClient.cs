using Jobo.Enterprise.Client.Models;

namespace Jobo.Enterprise.Client;

/// <summary>
/// Sub-client for the Auto Apply endpoints (POST /api/auto-apply/*, DELETE /api/auto-apply/*).
/// Access via <see cref="JoboClient.AutoApply"/>.
/// </summary>
public sealed class AutoApplyClient : JoboClientBase
{
    internal AutoApplyClient(HttpClient httpClient) : base(httpClient) { }

    /// <summary>
    /// Start a new auto-apply session for a job posting.
    /// </summary>
    /// <param name="applyUrl">The apply URL from the job listing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="AutoApplySessionResponse"/> with session details and form fields.</returns>
    public async Task<AutoApplySessionResponse> StartSessionAsync(
        string applyUrl,
        CancellationToken cancellationToken = default)
    {
        var request = new StartAutoApplySessionRequest { ApplyUrl = applyUrl };
        return await PostAsync<AutoApplySessionResponse>("/api/auto-apply/start", request, cancellationToken);
    }

    /// <summary>
    /// Set answers for an active auto-apply session.
    /// </summary>
    /// <param name="sessionId">The session ID from StartSessionAsync.</param>
    /// <param name="answers">List of field answers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An <see cref="AutoApplySessionResponse"/> with updated session state.</returns>
    public async Task<AutoApplySessionResponse> SetAnswersAsync(
        Guid sessionId,
        List<FieldAnswer> answers,
        CancellationToken cancellationToken = default)
    {
        var request = new SetAutoApplyAnswersRequest
        {
            SessionId = sessionId,
            Answers = answers
        };
        return await PostAsync<AutoApplySessionResponse>("/api/auto-apply/set-answers", request, cancellationToken);
    }

    /// <summary>
    /// End an auto-apply session.
    /// </summary>
    /// <param name="sessionId">The session ID to end.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the session was successfully ended.</returns>
    public async Task<bool> EndSessionAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        return await DeleteAsync($"/api/auto-apply/sessions/{sessionId}", cancellationToken);
    }
}
