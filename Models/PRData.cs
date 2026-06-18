namespace AIPRReviewer.Models;

public class PRData
{
    public int PrNumber { get; set; }
    public string PrTitle { get; set; } = "";
    public string PrDescription { get; set; } = "";
    public string PrAuthor { get; set; } = "";
    public string RepoName { get; set; } = "";
    public List<FileChange> FilesChanged { get; set; } = new();
}