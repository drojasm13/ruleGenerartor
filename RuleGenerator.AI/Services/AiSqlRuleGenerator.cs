using System.Text;
using System.Text.Json;
using OpenAI.Chat;
using RuleGenerator.Core.Abstractions;
using RuleGenerator.Core.Models;

namespace RuleGenerator.AI.Services;

public sealed class AiSqlRuleGenerator : IRuleGenerator<SqlRule>
{
    private readonly ChatClient _client;

    public AiSqlRuleGenerator(string model, string apiKey)
    {
        _client = new ChatClient(model, apiKey);
    }

    public async Task<SqlRule> GenerateAsync(
        string systemMessage,
        string userMessage,
        CancellationToken cancellationToken = default)
    {
        string schema =
        """
        {
          "type": "object",
          "properties": {
            "Query": { "type": "string" },
            "Explanation": { "type": "string" },
            "RequiresReview": { "type": "boolean" }
          },
          "required": ["Query","Explanation","RequiresReview"],
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
                jsonSchemaFormatName: "sql_rule",
                jsonSchema: BinaryData.FromBytes(Encoding.UTF8.GetBytes(schema)),
                jsonSchemaIsStrict: true)
        };

        ChatCompletion completion =
            await _client.CompleteChatAsync(messages, options, cancellationToken);

        string json = completion.Content[0].Text;

        SqlRule? result =
            JsonSerializer.Deserialize<SqlRule>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (result == null)
            throw new InvalidOperationException("Invalid SQL rule response.");

        return result;
    }
}