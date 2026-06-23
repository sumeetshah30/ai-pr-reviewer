using Newtonsoft.Json;

namespace AIPRReviewer.Models;

public class ReviewResult
{
    [JsonProperty("overall_summary")]
    public string OverallSummary { get; set; } = "";

    [JsonProperty("overall_verdict")]
    public string OverallVerdict { get; set; } = "";

    [JsonProperty("risk_level")]
    public string RiskLevel { get; set; } = "";

    [JsonProperty("issues")]
    public List<ReviewIssue> Issues { get; set; } = new();

    [JsonProperty("positive_feedback")]
    public string PositiveFeedback { get; set; } = "";

    [JsonProperty("suggestions")]
    public List<string> Suggestions { get; set; } = new();
}