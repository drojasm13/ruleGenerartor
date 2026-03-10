namespace RuleGenerator.Core.Models;

public sealed class RegexRule
{
    public string Regex { get; set; } = string.Empty;

    public List<string> Options { get; set; } = new();

    public string SearchTarget { get; set; } = string.Empty;

    public bool IncludeFiles { get; set; }

    public bool IncludeDirectories { get; set; }

    public bool RequiresReview { get; set; }

    public string Explanation { get; set; } = string.Empty;
}