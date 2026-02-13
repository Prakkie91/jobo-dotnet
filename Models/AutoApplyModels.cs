using System.Text.Json.Serialization;

namespace Jobo.Enterprise.Client.Models;

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
/// A field answer for auto-apply.
/// </summary>
public sealed class FieldAnswer
{
    [JsonPropertyName("field_id")] public string FieldId { get; set; } = string.Empty;
    [JsonPropertyName("value")] public string? Value { get; set; }
    [JsonPropertyName("values")] public List<string>? Values { get; set; }
    [JsonPropertyName("files")] public List<FieldAnswerFile>? Files { get; set; }
}

/// <summary>
/// A file upload answer for auto-apply.
/// </summary>
public sealed class FieldAnswerFile
{
    [JsonPropertyName("file_name")] public string FileName { get; set; } = string.Empty;
    [JsonPropertyName("content_type")] public string ContentType { get; set; } = string.Empty;
    [JsonPropertyName("data")] public string Data { get; set; } = string.Empty; // Base64 encoded
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
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("error")] public string? Error { get; set; }
    [JsonPropertyName("current_url")] public string? CurrentUrl { get; set; }
    [JsonPropertyName("is_terminal")] public bool IsTerminal { get; set; }
    [JsonPropertyName("validation_errors")] public List<ValidationError> ValidationErrors { get; set; } = new();
    [JsonPropertyName("fields")] public List<FormFieldInfo> Fields { get; set; } = new();
}

/// <summary>
/// Validation error from auto-apply.
/// </summary>
public sealed class ValidationError
{
    [JsonPropertyName("field_id")] public string FieldId { get; set; } = string.Empty;
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Information about a form field in auto-apply.
/// </summary>
public sealed class FormFieldInfo
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
    [JsonPropertyName("label")] public string? Label { get; set; }
    [JsonPropertyName("required")] public bool Required { get; set; }
    [JsonPropertyName("placeholder")] public string? Placeholder { get; set; }
    [JsonPropertyName("options")] public List<FieldOption>? Options { get; set; }
    [JsonPropertyName("validations")] public FieldValidations? Validations { get; set; }
}

/// <summary>
/// Option for a select/radio field.
/// </summary>
public sealed class FieldOption
{
    [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;
    [JsonPropertyName("label")] public string? Label { get; set; }
}

/// <summary>
/// Validations for a form field.
/// </summary>
public sealed class FieldValidations
{
    [JsonPropertyName("min_length")] public int? MinLength { get; set; }
    [JsonPropertyName("max_length")] public int? MaxLength { get; set; }
    [JsonPropertyName("pattern")] public string? Pattern { get; set; }
}
