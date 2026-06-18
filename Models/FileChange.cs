namespace AIPRReviewer.Models;

public class FileChange
{
    public string Filename { get; set; } = "";
    public string Status { get; set; } = "";
    public int Additions { get; set; }
    public int Deletions { get; set; }
    public string Patch { get; set; } = "";
}