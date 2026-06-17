using Octokit;
using AIPRReviewer.Models;
using DotNetEnv;

namespace AIPRReviewer.Agents;

public class FetcherAgent
{
    private readonly string _agentName = "FetcherAgent";
    private readonly GitHubClient _github;
    private readonly string _repoOwner;
    private readonly string _repoName;

    public FetcherAgent()
    {
        var token = Env.GetString("GITHUB_TOKEN");
        var repoFull = Env.GetString("GITHUB_REPO"); // format: owner/reponame

        var parts = repoFull.Split('/');
        _repoOwner = parts[0];
        _repoName = parts[1];

        _github = new GitHubClient(new ProductHeaderValue("AIPRReviewer"))
        {
            Credentials = new Credentials(token)
        };
    }

    public async Task<PRData?> RunAsync(int? prNumber = null)
    {
        Console.WriteLine($"[{_agentName}] Connecting to GitHub: {_repoOwner}/{_repoName}");

        PullRequest pr;

        if (prNumber.HasValue)
        {
            pr = await _github.PullRequest.Get(_repoOwner, _repoName, prNumber.Value);
        }
        else
        {
            var openPRs = await _github.PullRequest.GetAllForRepository(
                _repoOwner,
                _repoName,
                new PullRequestRequest { State = ItemStateFilter.Open }
            );

            if (!openPRs.Any())
            {
                Console.WriteLine($"[{_agentName}] No open PRs found.");
                return null;
            }

            pr = openPRs.First();
        }

        Console.WriteLine($"[{_agentName}] Found PR #{pr.Number}: {pr.Title}");

        var files = await _github.PullRequest.Files(_repoOwner, _repoName, pr.Number);

        var filesChanged = files.Select(f => new FileChange
        {
            Filename = f.FileName,
            Status = f.Status,
            Additions = f.Additions,
            Deletions = f.Deletions,
            Patch = f.Patch ?? ""
        }).ToList();

        Console.WriteLine($"[{_agentName}] Fetched {filesChanged.Count} changed files");

        return new PRData
        {
            PrNumber = pr.Number,
            PrTitle = pr.Title,
            PrDescription = pr.Body ?? "No description provided",
            PrAuthor = pr.User.Login,
            RepoName = $"{_repoOwner}/{_repoName}",
            FilesChanged = filesChanged
        };
    }
}