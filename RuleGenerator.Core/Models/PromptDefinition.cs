namespace RuleGenerator.Core.Models;

public sealed class PromptDefinition
{
    public string Key { get; set; } = string.Empty;

    public string SystemPrompt { get; set; } = string.Empty;
}