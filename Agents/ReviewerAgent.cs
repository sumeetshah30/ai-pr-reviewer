using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using AIPRReviewer.Models;
using Newtonsoft.Json;
using DotNetEnv;

namespace AIPRReviewer.Agents;

public class ReviewerAgent
{
    private readonly string _agentName = "ReviewerAgent";
    private readonly AnthropicClient _client;

    public ReviewerAgent()
    {
        var apiKey = Env.GetString("ANTHROPIC_API_KEY");
        _client = new AnthropicClient(apiKey);
    }

    private string BuildPrompt(PRData prData)
    {
        var filesText = "";
        foreach (var file in prData.FilesChanged)
        {
            if (!string.IsNullOrEmpty(file.Patch))
            {
                filesText += $"\n--- File: {file.Filename} ---\n";
                filesText += $"Status: {file.Status}\n";
                filesText += $"Lines added: {file.Additions} | Lines removed: {file.Deletions}\n\n";
                filesText += $"Diff:\n{file.Patch}\n\n";
            }
        }

        var jsonFormat = @"{
  ""overall_summary"": ""2-3 sentence summary of what this PR does"",
  ""overall_verdict"": ""APPROVE or REQUEST_CHANGES or COMMENT"",
  ""risk_level"": ""LOW or MEDIUM or HIGH"",
  ""issues"": [
    {
      ""filename"": ""exact filename from the diff"",
      ""severity"": ""CRITICAL or MAJOR or MINOR or SUGGESTION"",
      ""issue_type"": ""BUG or SECURITY or PERFORMANCE or STYLE or LOGIC"",
      ""line_description"": ""describe which part of code has the issue"",
      ""comment"": ""detailed explanation of the issue and how to fix it""
    }
  ],
  ""positive_feedback"": ""what was done well in this PR"",
  ""suggestions"": [""suggestion 1"", ""suggestion 2""]
}";

        var prompt = "You are an expert code reviewer. Review this Pull Request carefully.\n\n";
        prompt += $"PR TITLE: {prData.PrTitle}\n";
        prompt += $"PR DESCRIPTION: {prData.PrDescription}\n";
        prompt += $"AUTHOR: {prData.PrAuthor}\n\n";
        prompt += $"CHANGED FILES:\n{filesText}\n\n";
        prompt += "Review this code and return ONLY a JSON response (no other text) in this exact format:\n\n";
        prompt += jsonFormat;
        prompt += "\n\nBe specific, helpful, and constructive. Focus on real issues.";

        return prompt;
    }

    public async Task<(PRData prData, ReviewResult review)?> RunAsync(PRData? prData)
    {
        if (prData == null)
        {
            Console.WriteLine($"[{_agentName}] No PR data received. Stopping.");
            return null;
        }

        Console.WriteLine($"[{_agentName}] Sending PR #{prData.PrNumber} to Claude...");

        var prompt = BuildPrompt(prData);

        var messages = new List<Message>
        {
            new Message(RoleType.User, prompt)
        };

        var parameters = new MessageParameters
        {
            Messages = messages,
            MaxTokens = 2000,
            Model = "claude-sonnet-4-6",
            Stream = false,
            Temperature = 0.7m
        };

        var result = await _client.Messages.GetClaudeMessageAsync(parameters);

        var rawResponse = result.Message.ToString() ?? "";
        Console.WriteLine($"[{_agentName}] Got response from Claude");

        try
        {
            var cleanJson = rawResponse
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            var review = JsonConvert.DeserializeObject<ReviewResult>(cleanJson);

            if (review == null) throw new Exception("Deserialization returned null");

            Console.WriteLine($"[{_agentName}] Review parsed successfully");
            Console.WriteLine($"[{_agentName}] Verdict: {review.OverallVerdict}");
            Console.WriteLine($"[{_agentName}] Issues found: {review.Issues.Count}");

            return (prData, review);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{_agentName}] WARNING: Failed to parse Claude response");
            Console.WriteLine($"[{_agentName}] Error: {ex.Message}");
            Console.WriteLine($"[{_agentName}] Raw: {rawResponse[..Math.Min(200, rawResponse.Length)]}");

            var fallbackReview = new ReviewResult
            {
                OverallSummary = "Review parsing failed — manual review needed",
                OverallVerdict = "COMMENT",
                RiskLevel = "UNKNOWN",
                Issues = new List<ReviewIssue>(),
                PositiveFeedback = "",
                Suggestions = new List<string>
                {
                    $"Raw Claude output (first 500 chars): {rawResponse[..Math.Min(500, rawResponse.Length)]}"
                }
            };

            return (prData, fallbackReview);
        }
    }
}