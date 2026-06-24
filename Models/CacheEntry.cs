namespace AIPRReviewer.Models;

public class CacheEntry
{
    public string HeadSha { get; set; } = "";
    public string ReviewedAt { get; set; } = "";
    public string OverallVerdict { get; set; } = "";
    public string RiskLevel { get; set; } = "";
    public int IssuesCount { get; set; }
    public string OverallSummary { get; set; } = "";
}