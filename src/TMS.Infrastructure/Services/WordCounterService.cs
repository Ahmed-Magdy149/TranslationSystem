using Microsoft.Extensions.Logging;

namespace TMS.Infrastructure.Services;

public interface IWordCounterService
{
    int CountWords(string text);
    double EstimateHours(int wordCount, double averageSpeed = 250.0);
}

public class WordCounterService : IWordCounterService
{
    private readonly ILogger<WordCounterService> _logger;

    public WordCounterService(ILogger<WordCounterService> logger)
    {
        _logger = logger;
    }

    public int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        var wordCount = text
            .Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Length;

        _logger.LogInformation("Word count calculated: {WordCount}", wordCount);
        return wordCount;
    }

    public double EstimateHours(int wordCount, double averageSpeed = 250.0)
    {
        if (averageSpeed <= 0)
            averageSpeed = 250.0;

        var hours = Math.Round(wordCount / averageSpeed, 2);
        _logger.LogInformation("Estimated hours: {Hours} for {WordCount} words at {Speed} words/hour",
            hours, wordCount, averageSpeed);
        return hours;
    }
}
