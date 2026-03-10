namespace RuleGenerator.Core.Models;

public sealed class PromptSettings
{
    public Dictionary<string, string> Prompts { get; set; } = new();
}