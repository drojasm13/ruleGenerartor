using System.Text;
using System.Text.Json;
using OpenAI.Chat;
using RuleGenerator.Core.Abstractions;
using RuleGenerator.Core.Models;

namespace RuleGenerator.AI.Services;

public sealed class AiRegexRuleGenerator : IRuleGenerator<RegexRule>
{
    private readonly ChatClient _client;

    public AiRegexRuleGenerator(string model, string apiKey)
    {
        if (string.IsNullOrWhiteSpace(model))
        {
            throw new ArgumentException("Model cannot be null or empty.", nameof(model));
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key cannot be null or empty.", nameof(apiKey));
        }

        _client = new ChatClient(model, apiKey);
    }

    public async Task<RegexRule> GenerateAsync(
        string systemMessage,
        string userMessage,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(systemMessage))
        {
            throw new ArgumentException("System message cannot be null or empty.", nameof(systemMessage));
        }

        if (string.IsNullOrWhiteSpace(userMessage))
        {
            throw new ArgumentException("User message cannot be null or empty.", nameof(userMessage));
        }

        string schema =
        """
        {
          "type": "object",
          "properties": {
            "Regex": { "type": "string" },
            "Options": {
              "type": "array",
              "items": {
                "type": "string",
                "enum": ["IgnoreCase", "Multiline", "Singleline", "CultureInvariant"]
              }
            },
            "SearchTarget": {
              "type": "string",
              "enum": ["Name", "Path", "NameOrPath"]
            },
            "IncludeFiles": { "type": "boolean" },
            "IncludeDirectories": { "type": "boolean" },
            "RequiresReview": { "type": "boolean" },
            "Explanation": { "type": "string" }
          },
          "required": [
            "Regex",
            "Options",
            "SearchTarget",
            "IncludeFiles",
            "IncludeDirectories",
            "RequiresReview",
            "Explanation"
          ],
          "additionalProperties": false
        }
        """;

        List<ChatMessage> messages =
        [
            new SystemChatMessage(systemMessage),
            new UserChatMessage(userMessage)
        ];

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "regex_rule",
                jsonSchema: BinaryData.FromBytes(Encoding.UTF8.GetBytes(schema)),
                jsonSchemaIsStrict: true)
        };

        ChatCompletion completion = await _client.CompleteChatAsync(
            messages,
            options,
            cancellationToken);

        string json = completion.Content[0].Text;

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new InvalidOperationException("The AI response was empty.");
        }

        RegexRule? result = JsonSerializer.Deserialize<RegexRule>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (result is null)
        {
            throw new InvalidOperationException("Could not deserialize the AI response.");
        }

        return result;
    }
}