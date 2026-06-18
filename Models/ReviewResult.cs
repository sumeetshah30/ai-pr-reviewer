namespace AIPRReviewer.Models;

public class ReviewResult
{
    public string OverallSummary { get; set; } = "";
    public string OverallVerdict { get; set; } = "";
    public string RiskLevel { get; set; } = "";
    public List<ReviewIssue> Issues { get; set; } = new();
    public string PositiveFeedback { get; set; } = "";
    public List<string> Suggestions { get; set; } = new();
}