namespace RuleGenerator.Core.Abstractions;

public interface IRuleGenerator<T>
{
    Task<T> GenerateAsync(
        string systemMessage,
        string userMessage,
        CancellationToken cancellationToken = default);
}