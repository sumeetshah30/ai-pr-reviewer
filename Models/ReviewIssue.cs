namespace AIPRReviewer.Models;

public class ReviewIssue
{
    public string Filename { get; set; } = "";
    public string Severity { get; set; } = "";
    public string IssueType { get; set; } = "";
    public string LineDescription { get; set; } = "";
    public string Comment { get; set; } = "";
}