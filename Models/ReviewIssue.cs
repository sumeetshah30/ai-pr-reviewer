using Newtonsoft.Json;

namespace AIPRReviewer.Models;

public class ReviewIssue
{
    [JsonProperty("filename")]
    public string Filename { get; set; } = "";

    [JsonProperty("severity")]
    public string Severity { get; set; } = "";

    [JsonProperty("issue_type")]
    public string IssueType { get; set; } = "";

    [JsonProperty("line_description")]
    public string LineDescription { get; set; } = "";

    [JsonProperty("comment")]
    public string Comment { get; set; } = "";
}