using System.Text.Json.Serialization;

namespace Jobo.Enterprise.Client.Models;

// ── Sessions ─────────────────────────────────────────────────────────

/// <summary>
/// Request to start an auto-apply session.
/// </summary>
public sealed class StartAutoApplySessionRequest
{
    [JsonPropertyName("apply_url")] public string ApplyUrl { get; set; } = string.Empty;
}

/// <summary>
/// Request to set answers for an auto-apply session.
/// </summary>
public sealed class SetAutoApplyAnswersRequest
{
    [JsonPropertyName("session_id")] public Guid SessionId { get; set; }
    [JsonPropertyName("answers")] public List<FieldAnswer> Answers { get; set; } = new();
}

/// <summary>
/// An answer to set on a specific form field.
/// </summary>
public sealed class FieldAnswer
{
    [JsonPropertyName("field_id")] public string FieldId { get; set; } = string.Empty;
    /// <summary>snake_case FieldType matching the FormFieldInfo (e.g. "text", "select").</summary>
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
    [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;
    [JsonPropertyName("typeahead_selection")] public string? TypeaheadSelection { get; set; }
    [JsonPropertyName("clear_first")] public bool ClearFirst { get; set; } = true;
    [JsonPropertyName("handler_type")] public string? HandlerType { get; set; }
}

/// <summary>
/// Response from an auto-apply session operation.
/// </summary>
public sealed class AutoApplySessionResponse
{
    [JsonPropertyName("session_id")] public Guid SessionId { get; set; }
    [JsonPropertyName("provider_id")] public string ProviderId { get; set; } = string.Empty;
    [JsonPropertyName("provider_display_name")] public string ProviderDisplayName { get; set; } = string.Empty;
    [JsonPropertyName("success")] public bool Success { get; set; }
    /// <summary>snake_case ApplyFlowStatus (e.g. "form_ready", "submitted").</summary>
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("error")] public string? Error { get; set; }
    [JsonPropertyName("current_url")] public string? CurrentUrl { get; set; }
    [JsonPropertyName("is_terminal")] public bool IsTerminal { get; set; }
    [JsonPropertyName("validation_errors")] public List<ValidationError> ValidationErrors { get; set; } = new();
    [JsonPropertyName("fields")] public List<FormFieldInfo> Fields { get; set; } = new();
}

/// <summary>
/// A validation error displayed on the application form.
/// </summary>
public sealed class ValidationError
{
    [JsonPropertyName("field_id")] public string? FieldId { get; set; }
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Information about a form field discovered on an application page.
/// </summary>
public sealed class FormFieldInfo
{
    [JsonPropertyName("field_id")] public string FieldId { get; set; } = string.Empty;
    /// <summary>snake_case FieldType (e.g. "text", "text_area", "select").</summary>
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
    [JsonPropertyName("label")] public string Label { get; set; } = string.Empty;
    [JsonPropertyName("is_required")] public bool IsRequired { get; set; }
    [JsonPropertyName("options")] public List<FieldOption> Options { get; set; } = new();
    [JsonPropertyName("handler_type")] public string? HandlerType { get; set; }
}

/// <summary>
/// A single option in a select, radio group, or checkbox group.
/// </summary>
public sealed class FieldOption
{
    [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;
    [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;
}

// ── Run ──────────────────────────────────────────────────────────────

/// <summary>
/// Request to run the full auto-apply flow against a stored profile.
/// </summary>
public sealed class RunAutoApplyRequest
{
    [JsonPropertyName("profile_id")] public Guid ProfileId { get; set; }
    [JsonPropertyName("apply_url")] public string ApplyUrl { get; set; } = string.Empty;
}

/// <summary>
/// Response from a full auto-apply run.
/// </summary>
public sealed class RunAutoApplyResponse
{
    [JsonPropertyName("session_id")] public Guid SessionId { get; set; }
    [JsonPropertyName("profile_id")] public Guid ProfileId { get; set; }
    [JsonPropertyName("apply_url")] public string ApplyUrl { get; set; } = string.Empty;
    [JsonPropertyName("provider_id")] public string ProviderId { get; set; } = string.Empty;
    [JsonPropertyName("provider_display_name")] public string ProviderDisplayName { get; set; } = string.Empty;
    [JsonPropertyName("success")] public bool Success { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("error")] public string? Error { get; set; }
    [JsonPropertyName("steps_completed")] public int StepsCompleted { get; set; }
    [JsonPropertyName("fields_filled")] public int FieldsFilled { get; set; }
    [JsonPropertyName("duration_ms")] public long DurationMs { get; set; }
    [JsonPropertyName("step_log")] public List<AutoApplyStepLog> StepLog { get; set; } = new();
}

/// <summary>
/// A single step in a full auto-apply run.
/// </summary>
public sealed class AutoApplyStepLog
{
    [JsonPropertyName("step")] public int Step { get; set; }
    [JsonPropertyName("action")] public string Action { get; set; } = string.Empty;
    [JsonPropertyName("fields_count")] public int FieldsCount { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("error")] public string? Error { get; set; }
    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }
}

// ── Profiles ─────────────────────────────────────────────────────────

/// <summary>
/// Applicant profile used by auto-apply sessions (create/update body).
/// </summary>
public class AutoApplyProfileRequest
{
    [JsonPropertyName("name")] public string Name { get; set; } = "Default";

    // Personal
    [JsonPropertyName("first_name")] public string FirstName { get; set; } = string.Empty;
    [JsonPropertyName("last_name")] public string LastName { get; set; } = string.Empty;
    [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
    [JsonPropertyName("phone")] public string Phone { get; set; } = string.Empty;
    [JsonPropertyName("linkedin_url")] public string? LinkedinUrl { get; set; }
    [JsonPropertyName("website_url")] public string? WebsiteUrl { get; set; }
    [JsonPropertyName("portfolio_url")] public string? PortfolioUrl { get; set; }

    // Address
    [JsonPropertyName("address_line1")] public string? AddressLine1 { get; set; }
    [JsonPropertyName("address_line2")] public string? AddressLine2 { get; set; }
    [JsonPropertyName("city")] public string? City { get; set; }
    [JsonPropertyName("state")] public string? State { get; set; }
    [JsonPropertyName("zip_code")] public string? ZipCode { get; set; }
    [JsonPropertyName("country")] public string? Country { get; set; }

    // Resume
    [JsonPropertyName("resume_text")] public string? ResumeText { get; set; }
    [JsonPropertyName("resume_file_path")] public string? ResumeFilePath { get; set; }
    [JsonPropertyName("cover_letter_template")] public string? CoverLetterTemplate { get; set; }

    // Work Authorization / EEO
    [JsonPropertyName("work_authorization")] public string? WorkAuthorization { get; set; }
    [JsonPropertyName("requires_sponsorship")] public bool? RequiresSponsorship { get; set; }
    [JsonPropertyName("gender")] public string? Gender { get; set; }
    [JsonPropertyName("ethnicity")] public string? Ethnicity { get; set; }
    [JsonPropertyName("veteran_status")] public string? VeteranStatus { get; set; }
    [JsonPropertyName("disability_status")] public string? DisabilityStatus { get; set; }

    // Salary / Availability
    [JsonPropertyName("desired_salary")] public string? DesiredSalary { get; set; }
    [JsonPropertyName("salary_expectation_currency")] public string? SalaryExpectationCurrency { get; set; }
    [JsonPropertyName("available_start_date")] public string? AvailableStartDate { get; set; }
    [JsonPropertyName("willing_to_relocate")] public bool? WillingToRelocate { get; set; }

    // Education
    [JsonPropertyName("highest_degree")] public string? HighestDegree { get; set; }
    [JsonPropertyName("field_of_study")] public string? FieldOfStudy { get; set; }
    [JsonPropertyName("university")] public string? University { get; set; }
    [JsonPropertyName("graduation_year")] public string? GraduationYear { get; set; }

    // Experience
    [JsonPropertyName("years_of_experience")] public string? YearsOfExperience { get; set; }
    [JsonPropertyName("current_job_title")] public string? CurrentJobTitle { get; set; }
    [JsonPropertyName("current_company")] public string? CurrentCompany { get; set; }

    // Custom Q&A
    [JsonPropertyName("custom_answers")] public Dictionary<string, string>? CustomAnswers { get; set; }
}

/// <summary>
/// An auto-apply profile as returned by the API.
/// </summary>
public sealed class AutoApplyProfileResponse : AutoApplyProfileRequest
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
}
