using System.Text.Json;
using RuleGenerator.AI.Services;
using RuleGenerator.Core.Abstractions;
using RuleGenerator.Core.Models;

string? apiKey = "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"; //Environment.GetEnvironmentVariable("OPENAI_API_KEY");

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("OPENAI_API_KEY not set.");
    return;
}

string promptFile = Path.Combine(AppContext.BaseDirectory, "prompts.json");

Console.WriteLine($"Looking for prompt file at: {promptFile}");

if (!File.Exists(promptFile))
{
    Console.WriteLine($"Prompt file not found: {promptFile}");
    return;
}

string json = File.ReadAllText(promptFile);

PromptSettings? settings =
    JsonSerializer.Deserialize<PromptSettings>(json);

if (settings == null || settings.Prompts.Count == 0)
{
    Console.WriteLine("No prompts defined.");
    return;
}

Console.WriteLine("Available prompts:");

foreach (var key in settings.Prompts.Keys)
{
    Console.WriteLine($" - {key}");
}

Console.WriteLine();
Console.Write("Enter prompt key: ");
string? keySelected = Console.ReadLine();

if (string.IsNullOrWhiteSpace(keySelected) ||
    !settings.Prompts.ContainsKey(keySelected))
{
    Console.WriteLine("Invalid prompt key.");
    return;
}

string systemPrompt = settings.Prompts[keySelected];

Console.WriteLine();
Console.WriteLine("Enter USER request:");
string? userRequest = Console.ReadLine();

if (string.IsNullOrWhiteSpace(userRequest))
{
    Console.WriteLine("User request empty.");
    return;
}

IRuleGenerator<RegexRule> generator =
    new AiRegexRuleGenerator("gpt-4o-mini", apiKey);

RegexRule rule =
    await generator.GenerateAsync(systemPrompt, userRequest);

Console.WriteLine();
Console.WriteLine("Generated rule:");
Console.WriteLine($"Regex: {rule.Regex}");
Console.WriteLine($"Options: {string.Join(", ", rule.Options)}");
Console.WriteLine($"Target: {rule.SearchTarget}");
Console.WriteLine($"Files: {rule.IncludeFiles}");
Console.WriteLine($"Directories: {rule.IncludeDirectories}");
Console.WriteLine($"Review Required: {rule.RequiresReview}");
Console.WriteLine($"Explanation: {rule.Explanation}");