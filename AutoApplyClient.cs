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
    /// Run the full auto-apply flow end-to-end against a stored profile.
    /// </summary>
    /// <param name="profileId">An existing auto-apply profile id.</param>
    /// <param name="applyUrl">The apply URL from the job listing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="RunAutoApplyResponse"/> summarizing the run.</returns>
    public async Task<RunAutoApplyResponse> RunAsync(
        Guid profileId,
        string applyUrl,
        CancellationToken cancellationToken = default)
    {
        var request = new RunAutoApplyRequest { ProfileId = profileId, ApplyUrl = applyUrl };
        return await PostAsync<RunAutoApplyResponse>("/api/auto-apply/run", request, cancellationToken);
    }

    /// <summary>
    /// End an auto-apply session.
    /// </summary>
    /// <param name="sessionId">The session ID to end.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the session was successfully ended, false if not found.</returns>
    public async Task<bool> EndSessionAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        return await DeleteAsync($"/api/auto-apply/sessions/{sessionId}", cancellationToken);
    }

    // ── Profiles ─────────────────────────────────────────────────────

    /// <summary>
    /// Create a new applicant profile (POST /api/auto-apply/profiles).
    /// </summary>
    public async Task<AutoApplyProfileResponse> CreateProfileAsync(
        AutoApplyProfileRequest profile,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<AutoApplyProfileResponse>("/api/auto-apply/profiles", profile, cancellationToken);
    }

    /// <summary>
    /// List all profiles for the authenticated user (GET /api/auto-apply/profiles).
    /// </summary>
    public async Task<List<AutoApplyProfileResponse>> ListProfilesAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<List<AutoApplyProfileResponse>>("/api/auto-apply/profiles", cancellationToken);
    }

    /// <summary>
    /// Get a specific profile by id (GET /api/auto-apply/profiles/{id}).
    /// </summary>
    public async Task<AutoApplyProfileResponse> GetProfileAsync(
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<AutoApplyProfileResponse>($"/api/auto-apply/profiles/{profileId}", cancellationToken);
    }

    /// <summary>
    /// Update an existing profile (PUT /api/auto-apply/profiles/{id}).
    /// </summary>
    public async Task<AutoApplyProfileResponse> UpdateProfileAsync(
        Guid profileId,
        AutoApplyProfileRequest profile,
        CancellationToken cancellationToken = default)
    {
        return await PutAsync<AutoApplyProfileResponse>($"/api/auto-apply/profiles/{profileId}", profile, cancellationToken);
    }

    /// <summary>
    /// Delete a profile (DELETE /api/auto-apply/profiles/{id}).
    /// </summary>
    /// <returns>True if the profile was deleted, false if not found.</returns>
    public async Task<bool> DeleteProfileAsync(
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        return await DeleteAsync($"/api/auto-apply/profiles/{profileId}", cancellationToken);
    }
}
