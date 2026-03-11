namespace RuleGenerator.Core.Models;

public sealed class SqlRule
{
    public string Query { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;

    public bool RequiresReview { get; set; }
}