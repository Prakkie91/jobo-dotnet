using System.Text.Json.Serialization;

namespace Jobo.Enterprise.Client.Models;

/// <summary>
/// A single funding round.
/// </summary>
public sealed class CompanyFundingRound
{
    [JsonPropertyName("investment_type")] public string? InvestmentType { get; set; }
    [JsonPropertyName("announced_on")] public string? AnnouncedOn { get; set; }
    [JsonPropertyName("raised_amount")] public string? RaisedAmount { get; set; }
    [JsonPropertyName("post_money_valuation")] public string? PostMoneyValuation { get; set; }
    [JsonPropertyName("investor_count")] public int InvestorCount { get; set; }
    [JsonPropertyName("lead_investor")] public string? LeadInvestor { get; set; }
}

/// <summary>
/// A member of the company's leadership team.
/// </summary>
public sealed class CompanyLeader
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("linkedin_url")] public string? LinkedinUrl { get; set; }
    [JsonPropertyName("avatar_url")] public string? AvatarUrl { get; set; }
}

/// <summary>
/// A company rating from an external source (e.g. Glassdoor).
/// </summary>
public sealed class CompanyRating
{
    [JsonPropertyName("source")] public string? Source { get; set; }
    [JsonPropertyName("rating")] public string? Rating { get; set; }
    [JsonPropertyName("url")] public string? Url { get; set; }
    [JsonPropertyName("review_count")] public int? ReviewCount { get; set; }
}

/// <summary>
/// A press article referencing the company.
/// </summary>
public sealed class CompanyPressReference
{
    [JsonPropertyName("url")] public string? Url { get; set; }
    [JsonPropertyName("posted_on")] public string? PostedOn { get; set; }
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("publisher")] public string? Publisher { get; set; }
}

/// <summary>
/// H1B sponsorship job count for a given year.
/// </summary>
public sealed class CompanyH1bJobCount
{
    [JsonPropertyName("year")] public string? Year { get; set; }
    [JsonPropertyName("count")] public int Count { get; set; }
}

/// <summary>
/// H1B sponsorship distribution by job title.
/// </summary>
public sealed class CompanyH1bTitleDistribution
{
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("count")] public int Count { get; set; }
}

/// <summary>
/// A technology entry in the company's tech stack.
/// </summary>
public sealed class CompanyTechnology
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("categories")] public List<string> Categories { get; set; } = new();
}

/// <summary>
/// A product or piece of software the company offers / uses.
/// </summary>
public sealed class CompanyProduct
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
}

/// <summary>
/// A company acquired by this company.
/// </summary>
public sealed class CompanyAcquisition
{
    [JsonPropertyName("acquiree_name")] public string? AcquireeName { get; set; }
    [JsonPropertyName("title")] public string? Title { get; set; }
}

/// <summary>
/// An exit event for the company.
/// </summary>
public sealed class CompanyExit
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
}

/// <summary>
/// A sub-organization of the company.
/// </summary>
public sealed class CompanySubOrganization
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("ownership_type")] public string? OwnershipType { get; set; }
    [JsonPropertyName("title")] public string? Title { get; set; }
}

/// <summary>
/// A featured list the company appears on.
/// </summary>
public sealed class CompanyFeaturedList
{
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("org_num")] public int? OrgNum { get; set; }
    [JsonPropertyName("funding_total_formatted")] public string? FundingTotalFormatted { get; set; }
    [JsonPropertyName("funding_total_usd")] public long? FundingTotalUsd { get; set; }
}

/// <summary>
/// A dated event in the company's history (layoff, leadership hire, etc.).
/// </summary>
public sealed class CompanyKeyEvent
{
    [JsonPropertyName("date")] public string? Date { get; set; }
}

/// <summary>
/// An event the company appeared at.
/// </summary>
public sealed class CompanyEventAppearance
{
    [JsonPropertyName("appearance_type")] public string? AppearanceType { get; set; }
    [JsonPropertyName("event")] public string? Event { get; set; }
    [JsonPropertyName("event_starts_on")] public string? EventStartsOn { get; set; }
    [JsonPropertyName("image")] public string? Image { get; set; }
}

/// <summary>
/// A fully enriched company profile (GET /api/companies/{id}).
/// </summary>
public sealed class Company
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("legal_name")] public string? LegalName { get; set; }
    [JsonPropertyName("summary")] public string? Summary { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("website")] public string? Website { get; set; }
    [JsonPropertyName("listing_url")] public string? ListingUrl { get; set; }
    [JsonPropertyName("logo_url")] public string? LogoUrl { get; set; }

    // Socials
    [JsonPropertyName("linkedin_url")] public string? LinkedinUrl { get; set; }
    [JsonPropertyName("linkedin_company_id")] public string? LinkedinCompanyId { get; set; }
    [JsonPropertyName("twitter_url")] public string? TwitterUrl { get; set; }
    [JsonPropertyName("facebook_url")] public string? FacebookUrl { get; set; }
    [JsonPropertyName("instagram_url")] public string? InstagramUrl { get; set; }
    [JsonPropertyName("angellist_url")] public string? AngellistUrl { get; set; }
    [JsonPropertyName("youtube_url")] public string? YoutubeUrl { get; set; }
    [JsonPropertyName("github_url")] public string? GithubUrl { get; set; }
    [JsonPropertyName("g2_url")] public string? G2Url { get; set; }
    [JsonPropertyName("crunchbase_url")] public string? CrunchbaseUrl { get; set; }

    // Location
    [JsonPropertyName("headquarters_location")] public string? HeadquartersLocation { get; set; }
    [JsonPropertyName("headquarters_region")] public string? HeadquartersRegion { get; set; }
    [JsonPropertyName("headquarters_regions")] public List<string> HeadquartersRegions { get; set; } = new();
    [JsonPropertyName("country_code")] public string? CountryCode { get; set; }
    [JsonPropertyName("continent")] public string? Continent { get; set; }
    [JsonPropertyName("phone_number")] public string? PhoneNumber { get; set; }
    [JsonPropertyName("email_address")] public string? EmailAddress { get; set; }

    // Basic facts
    [JsonPropertyName("founding_year")] public string? FoundingYear { get; set; }
    [JsonPropertyName("company_size")] public string? CompanySize { get; set; }
    [JsonPropertyName("revenue")] public string? Revenue { get; set; }
    [JsonPropertyName("is_agency")] public bool IsAgency { get; set; }
    [JsonPropertyName("industries")] public List<string> Industries { get; set; } = new();
    [JsonPropertyName("primary_industry")] public string? PrimaryIndustry { get; set; }
    [JsonPropertyName("categories")] public List<string> Categories { get; set; } = new();
    [JsonPropertyName("naics_codes")] public List<string> NaicsCodes { get; set; } = new();

    // Status
    [JsonPropertyName("operating_status")] public string? OperatingStatus { get; set; }
    [JsonPropertyName("ipo_status")] public string? IpoStatus { get; set; }
    [JsonPropertyName("company_type")] public string? CompanyType { get; set; }
    [JsonPropertyName("stock_symbol")] public string? StockSymbol { get; set; }
    [JsonPropertyName("stock_exchange")] public string? StockExchange { get; set; }
    [JsonPropertyName("is_acquired")] public bool IsAcquired { get; set; }
    [JsonPropertyName("acquired_by_company")] public string? AcquiredByCompany { get; set; }
    [JsonPropertyName("parent_company_url")] public string? ParentCompanyUrl { get; set; }

    // Funding
    [JsonPropertyName("funding_stage")] public string? FundingStage { get; set; }
    [JsonPropertyName("total_funding")] public string? TotalFunding { get; set; }
    [JsonPropertyName("funds_total_formatted")] public string? FundsTotalFormatted { get; set; }
    [JsonPropertyName("investors")] public List<string> Investors { get; set; } = new();
    [JsonPropertyName("funding_rounds")] public List<CompanyFundingRound> FundingRounds { get; set; } = new();

    // People / culture
    [JsonPropertyName("founders")] public List<string> Founders { get; set; } = new();
    [JsonPropertyName("leadership")] public List<CompanyLeader> Leadership { get; set; } = new();
    [JsonPropertyName("leadership_hires")] public List<CompanyKeyEvent> LeadershipHires { get; set; } = new();
    [JsonPropertyName("layoffs")] public List<CompanyKeyEvent> Layoffs { get; set; } = new();
    [JsonPropertyName("ratings")] public List<CompanyRating> Ratings { get; set; } = new();
    [JsonPropertyName("press_references")] public List<CompanyPressReference> PressReferences { get; set; } = new();
    [JsonPropertyName("h1b_annual_job_counts")] public List<CompanyH1bJobCount> H1bAnnualJobCounts { get; set; } = new();
    [JsonPropertyName("h1b_title_distribution")] public List<CompanyH1bTitleDistribution> H1bTitleDistribution { get; set; } = new();

    // Products & technology
    [JsonPropertyName("technology_list")] public List<string> TechnologyList { get; set; } = new();
    [JsonPropertyName("tech_stack")] public List<CompanyTechnology> TechStack { get; set; } = new();
    [JsonPropertyName("products")] public List<CompanyProduct> Products { get; set; } = new();
    [JsonPropertyName("software_used")] public List<CompanyProduct> SoftwareUsed { get; set; } = new();
    [JsonPropertyName("research_focus_areas")] public List<string> ResearchFocusAreas { get; set; } = new();

    // Relationships
    [JsonPropertyName("acquisitions")] public List<CompanyAcquisition> Acquisitions { get; set; } = new();
    [JsonPropertyName("exits")] public List<CompanyExit> Exits { get; set; } = new();
    [JsonPropertyName("subsidiary_list")] public List<string> SubsidiaryList { get; set; } = new();
    [JsonPropertyName("sub_organizations")] public List<CompanySubOrganization> SubOrganizations { get; set; } = new();
    [JsonPropertyName("featured_lists")] public List<CompanyFeaturedList> FeaturedLists { get; set; } = new();
    [JsonPropertyName("event_appearances")] public List<CompanyEventAppearance> EventAppearances { get; set; } = new();

    // Investor profile
    [JsonPropertyName("investor_types")] public List<string> InvestorTypes { get; set; } = new();

    [JsonPropertyName("page_rank")] public double? PageRank { get; set; }
}
