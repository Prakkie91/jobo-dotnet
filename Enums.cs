namespace Jobo.Enterprise.Client;

// Closed value sets for the Jobo Enterprise Jobs API.
//
// These are not C# enums: the API exchanges plain snake_case strings, and the
// client methods take `string` parameters so any raw value remains valid. Each
// class below simply exposes the known values as `const string` fields for
// discoverability and autocomplete — e.g. `WorkModel.Remote` instead of
// remembering the literal "remote".

/// <summary>Where the job is performed (<c>work_model</c> / <c>workplace_type</c>).</summary>
public static class WorkModel
{
    public const string Remote = "remote";
    public const string Hybrid = "hybrid";
    public const string Onsite = "onsite";
}

/// <summary>Nature of the engagement (<c>employment_type</c>).</summary>
public static class EmploymentType
{
    public const string FullTime = "full_time";
    public const string PartTime = "part_time";
    public const string Contract = "contract";
    public const string Internship = "internship";
    public const string Temporary = "temporary";
}

/// <summary>Seniority of the role (<c>experience_level</c>).</summary>
public static class ExperienceLevel
{
    public const string Entry = "entry";
    public const string Mid = "mid";
    public const string Senior = "senior";
    public const string Lead = "lead";
    public const string Executive = "executive";
}

/// <summary>Period a compensation range refers to (<c>compensation.period</c>).</summary>
public static class CompensationPeriod
{
    public const string Hour = "hour";
    public const string Day = "day";
    public const string Week = "week";
    public const string Month = "month";
    public const string Year = "year";
}

/// <summary>Classification of a qualification skill (<c>skills[].type</c>).</summary>
public static class SkillType
{
    public const string Hard = "hard";
    public const string Soft = "soft";
}
