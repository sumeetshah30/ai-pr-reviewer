using AIPRReviewer.Agents;

namespace AIPRReviewer;

public class Orchestrator
{
    private readonly FetcherAgent _fetcher;
    private readonly ReviewerAgent _reviewer;
    private readonly CommenterAgent _commenter;

    public Orchestrator()
    {
        Console.WriteLine("=== AI PR Review Agent (C#) Starting ===\n");

        _fetcher = new FetcherAgent();
        _reviewer = new ReviewerAgent();
        _commenter = new CommenterAgent();
    }

    public async Task RunAsync(int? prNumber = null)
    {
        Console.WriteLine("--- Step 1: Fetching PR from GitHub ---");
        var prData = await _fetcher.RunAsync(prNumber);

        if (prData == null)
        {
            Console.WriteLine("No PR found. Exiting.");
            return;
        }

        Console.WriteLine("\n--- Step 2: Reviewing with Claude ---");
        var reviewData = await _reviewer.RunAsync(prData);

        if (reviewData == null)
        {
            Console.WriteLine("Review failed. Exiting.");
            return;
        }

        Console.WriteLine("\n--- Step 3: Posting to GitHub ---");
        var success = await _commenter.RunAsync(reviewData);

        Console.WriteLine($"\n=== Done! PR #{prData.PrNumber} reviewed. ===");
    }
}