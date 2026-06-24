# AI PR Review Agent

A 3-agent system in C# that automatically reviews GitHub Pull Requests using Claude.

## What it does

When you run this against a PR, three agents work in sequence:

1. **FetcherAgent** — connects to GitHub via Octokit, pulls the PR diff and metadata
2. **ReviewerAgent** — sends the diff to Claude, gets back a structured JSON code review
3. **CommenterAgent** — posts the review as comments directly on the GitHub PR

Three independent "agents" run one after another. **FetcherAgent** talks to GitHub and pulls the code changes from a Pull Request. **ReviewerAgent** takes those changes, builds a prompt, sends it to Claude, and turns Claude's answer into structured data (not just text — actual `Severity`, `IssueType`, `Verdict` fields your code can use). **CommenterAgent** takes that structured data and posts it back onto the real GitHub PR as comments. An **Orchestrator** simply calls these three agents in order and stops cleanly if any step fails. That's the entire system — no magic, no hidden framework, just three classes calling an API each and passing data forward.

## Full Architecture Diagram

```mermaid
flowchart TD
    A[You run: dotnet run 1] --> B[Program.cs]
    B --> C[Orchestrator.cs]

    C --> D[FetcherAgent]
    D -->|"GitHub REST API<br/>via Octokit"| E[(GitHub)]
    E -->|"PR title, description,<br/>author, file diffs"| D
    D -->|"PRData object"| C

    C --> F[ReviewerAgent]
    F -->|"Builds prompt with<br/>full diff + JSON schema"| G[(Claude API)]
    G -->|"Raw text response<br/>JSON inside"| F
    F -->|"Parses JSON into<br/>ReviewResult object"| C

    C --> H[CommenterAgent]
    H -->|"Formats review as<br/>Markdown comment"| E
    H -->|"Posts main comment +<br/>1 comment per CRITICAL/MAJOR issue"| E

    C --> I[Console: Done!]


## Real example

Ran this against a test PR with an intentional bug (unguarded division). The agent:
- Correctly flagged it as **CRITICAL**
- Identified it as the likely intentional bug
- Suggested an exact code fix with a guard clause
- Also caught 4 secondary style/design issues unprompted

See PR #1 in this repo for the live comment thread.

## Real failures I hit and fixed

- **JSON casing mismatch**: Claude returns snake_case keys (`overall_verdict`) but C# 
  models use PascalCase (`OverallVerdict`). Newtonsoft.Json doesn't auto-map these — 
  had to add explicit `[JsonProperty]` attributes to every model field.
- **Unhandled API exceptions**: the Claude SDK throws on billing/auth errors instead of 
  returning a graceful error object. Wrapped the API call in try/catch with a fallback 
  `ReviewResult` so the pipeline never crashes — it degrades gracefully and tells you why.
- **Empty/binary file diffs**: some PR files have no `.Patch` content (binary files, 
  lockfiles). Added a null/empty check before sending to Claude to avoid wasting tokens 
  on unreviewable content.
- **GitHub rate limits**: added a 500ms delay between posting multiple issue comments 
  to avoid hitting secondary rate limits on the Issues API.

## Tech stack

- C# / .NET 10
- Anthropic.SDK (Claude API)
- Octokit (GitHub API)
- Newtonsoft.Json
- DotNetEnv

## Setup

1. Clone the repo
2. Copy `.env.example` to `.env` and fill in:
   - `ANTHROPIC_API_KEY` — from console.anthropic.com
   - `GITHUB_TOKEN` — classic PAT with `repo` scope
   - `GITHUB_REPO` — format: `owner/reponame`
3. `dotnet restore`
4. `dotnet run` — reviews the latest open PR
5. `dotnet run <PR_number>` — reviews a specific PR

## Architecture decisions

- Each agent is a standalone class with a single `RunAsync` method — easy to test 
  or swap independently
- Orchestrator wires them together in sequence; if any agent returns null, the 
  pipeline stops cleanly instead of cascading failures
- All Claude responses are parsed defensively — a malformed response never crashes 
  the app, it falls back to a placeholder review with the raw output for debugging