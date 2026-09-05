# AI PR Review Agent

An AI-powered Pull Request review assistant built with **C# and .NET** that analyzes GitHub Pull Requests and turns potential code problems into actionable review feedback.

The project is designed as an automated first-pass reviewer: it can identify bugs, risky code, security concerns, maintainability issues, and improvement opportunities before human reviewers spend time on the PR.

> **Important:** This project uses API credentials for GitHub and the AI provider. Never commit real credentials, tokens, or `.env` files to source control.

---

## Why this project?

Code reviews are essential, but repetitive checks can consume a lot of engineering time. A useful automated reviewer can provide an early signal about problems such as:

- Null reference risks
- Incorrect logic
- Error-handling gaps
- Security-sensitive patterns
- Poor validation
- Maintainability problems
- Naming and readability issues
- Missing tests
- Potential edge cases

The goal is **not to replace developers or human code review**. Instead, the agent provides a fast, consistent first layer of feedback that developers can act on before or during the normal review process.

---

## What it does

The application can:

1. Connect to a GitHub repository.
2. Select a Pull Request to review.
3. Read the PR metadata and changed files.
4. Send relevant code changes to an AI reviewer.
5. Analyze the changes for bugs, risks, and improvement opportunities.
6. Produce a structured review with severity and recommendations.
7. Post the resulting feedback back to the Pull Request.

A review can identify issues ranging from **critical runtime bugs** to smaller code-quality improvements.

---

## Example

A PR containing code such as:

```csharp
public int Divide(int a, int b)
{
    return a / b;
}
```

may be flagged because `b == 0` can cause a runtime exception.

The review can explain the problem and suggest a safer implementation:

```csharp
public int Divide(int a, int b)
{
    if (b == 0)
        throw new ArgumentException("Divisor cannot be zero.", nameof(b));

    return a / b;
}
```

The same approach can be used for other classes of problems, including null handling, validation, security-sensitive code, and edge cases.

---

## High-level architecture

```text
                 GitHub Pull Request
                          |
                          v
                  +---------------+
                  | Fetcher Agent |
                  +---------------+
                          |
                          v
                  +---------------+
                  | Reviewer Agent|
                  |   AI Analysis |
                  +---------------+
                          |
                          v
                  +----------------+
                  | Commenter Agent|
                  +----------------+
                          |
                          v
                   GitHub PR Review
```

The application separates the workflow into focused responsibilities so that each stage can evolve independently.

### Fetcher Agent

Responsible for retrieving Pull Request information and changed files from GitHub.

### Reviewer Agent

Responsible for sending the relevant changes to the AI model and converting the response into structured review information.

### Commenter Agent

Responsible for publishing the review results and issue-level feedback back to GitHub.

### Orchestrator

Coordinates the complete review workflow from fetching through publishing.

---

## Key features

### Automated PR analysis

Review a Pull Request without manually copying code into an AI tool.

### AI-assisted code review

Use an AI model to reason about changed code and identify potential problems.

### Severity-based feedback

Issues can be categorized by importance so developers can focus on the most significant problems first.

### Actionable suggestions

The reviewer can provide explanations and possible fixes instead of simply saying that something is wrong.

### Inline-style feedback

Issue-level feedback can be associated with affected files or changes where supported by the review workflow.

### Developer-friendly output

The review is designed to resemble practical engineering feedback rather than a generic AI response.

---

## Technology stack

| Technology | Purpose |
|---|---|
| C# | Primary programming language |
| .NET | Application/runtime platform |
| GitHub API | Pull Request and repository integration |
| Octokit | GitHub API client |
| Anthropic / Claude | AI-powered code analysis |
| DotNetEnv | Local environment configuration |
| Newtonsoft.Json | JSON processing |

---

## Project structure

```text
ai-pr-reviewer/
│
├── Agents/
│   ├── FetcherAgent.cs
│   ├── ReviewerAgent.cs
│   └── CommenterAgent.cs
│
├── Models/
│   └── Review-related models
│
├── Program.cs
├── Orchestrator.cs
│
├── .env.example
├── .gitignore
├── ai-pr-reviewer.csproj
├── README.md
│
└── docs/
    └── PROJECT_GUIDE.md
```

The exact file structure may evolve as the project grows.

---

# Getting started

## Prerequisites

Install:

- .NET SDK compatible with the project
- A GitHub account
- Access to a GitHub repository you want to review
- An API key for the configured AI provider
- A GitHub token with the permissions required for the repository/API operations

Check your .NET installation:

```bash
dotnet --version
```

---

## Clone the repository

```bash
git clone https://github.com/sumeetshah30/ai-pr-reviewer.git
cd ai-pr-reviewer
```

---

## Configure environment variables

Create a local `.env` file based on `.env.example`.

Example:

```env
ANTHROPIC_API_KEY=your_ai_api_key
GITHUB_TOKEN=your_github_token
GITHUB_REPO=owner/repository
```

### Security

Do **not**:

- Put real API keys in `README.md`
- Commit `.env`
- Put GitHub tokens in source code
- Share secrets in screenshots
- Push credentials to GitHub
- Hard-code credentials into configuration files

Use secret-management facilities such as GitHub Actions Secrets, Azure Key Vault, or another secure secret store for production/CI environments.

---

## Restore dependencies

```bash
dotnet restore
```

---

## Build

```bash
dotnet build
```

---

## Run

To review the default Pull Request selection:

```bash
dotnet run
```

To explicitly review a Pull Request:

```bash
dotnet run -- 2
```

Replace `2` with the Pull Request number you want to analyze.

---

# Ways to use the project

The agent can be used in several different workflows.

## 1. Developer self-review

A developer can run the agent before requesting human review.

```text
Developer creates PR
        ↓
AI review
        ↓
Developer fixes issues
        ↓
Human review
```

This can reduce avoidable review comments and shorten the feedback loop.

---

## 2. Automated first-pass review

Run the reviewer whenever a Pull Request is opened or updated.

A CI/CD workflow could eventually trigger:

```text
Pull Request opened/updated
          ↓
     AI PR Reviewer
          ↓
     Review feedback
          ↓
   Developer addresses issues
          ↓
      Human review
```

This makes AI review part of the normal engineering workflow.

---

## 3. Security-focused review

The system can be extended with specialized prompts/rules for detecting patterns such as:

- Unsafe input handling
- Missing validation
- Sensitive data exposure
- Authentication/authorization mistakes
- Injection risks
- Insecure configuration
- Dangerous API usage

AI feedback should still be treated as an additional signal, not a replacement for dedicated security testing.

---

## 4. Code quality and maintainability

The reviewer can also focus on:

- Naming
- Duplication
- Complexity
- Error handling
- Readability
- Testability
- Maintainability
- API design

This is particularly useful for large teams where maintaining consistent standards is difficult.

---

## 5. Learning and education

The project can be used as a learning tool for developers.

A beginner can submit intentionally flawed code and study:

- Why the code is problematic
- What edge case was missed
- How the code can be improved
- What tests should be added
- Why one implementation is safer than another

---

## 6. Open-source projects

Maintainers can use an automated reviewer as an initial filter for incoming contributions.

For example:

```text
Contributor PR
      ↓
Automated AI review
      ↓
Potential issues highlighted
      ↓
Maintainer performs final review
```

This can be useful when maintainers have limited time.

---

## 7. Multiple review modes

The same architecture can support different review profiles in the future.

Examples:

```text
General Review
Security Review
Performance Review
Testing Review
Architecture Review
.NET/C# Review
API Review
Database Review
```

The review mode could be selected based on repository, branch, labels, or workflow configuration.

---

# What makes the project useful?

The value of the project is not simply calling an AI API.

It demonstrates how an AI capability can be integrated into a real developer workflow:

```text
External platform
       ↓
GitHub API
       ↓
Application orchestration
       ↓
AI reasoning
       ↓
Structured result
       ↓
Developer feedback
```

This makes the project a practical example of combining:

- C#
- .NET
- API integration
- GitHub automation
- AI/LLM integration
- Structured data processing
- Software engineering workflows

---

# Current limitations

AI-generated reviews are not guaranteed to be correct.

The reviewer may occasionally:

- Miss a real issue
- Report a low-value issue
- Suggest an unsuitable fix
- Misunderstand application-specific requirements
- Flag intentional behavior as a problem

For this reason, the recommended workflow is:

> **AI review → developer validation → human review → tests/CI**

The agent should assist engineering judgment rather than replace it.

---

# Future improvements

Potential improvements include:

- GitHub Actions integration
- Automatic review on PR creation/update
- Review status checks
- Configurable severity thresholds
- Repository-specific review instructions
- Custom coding standards
- Security-focused review mode
- Test coverage analysis
- Duplicate issue suppression
- Review history
- Pull Request risk scoring
- Multiple AI providers
- Local model support
- Dashboard for review metrics
- Configurable review prompts
- Better inline GitHub review comments
- Unit and integration test coverage

---

# Example CI/CD direction

A future GitHub Actions workflow could look like:

```text
              Pull Request
                    |
                    v
             GitHub Actions
                    |
                    v
            AI PR Review Agent
                    |
          +---------+---------+
          |                   |
          v                   v
    Review comments       Status/check
          |                   |
          +---------+---------+
                    |
                    v
             Human approval
                    |
                    v
                 Merge
```

This allows AI-assisted review to become part of a larger DevOps workflow.

---

# Recommended production practices

For real organizational use:

1. Store secrets in a dedicated secret manager.
2. Use least-privilege GitHub permissions.
3. Avoid sending unnecessary sensitive source code to external AI services.
4. Define repository-specific review policies.
5. Keep human approval in the merge process.
6. Add automated tests alongside AI review.
7. Monitor AI review quality and false positives.
8. Log operational information without logging credentials or sensitive payloads.

---

# Contributing

Contributions are welcome.

A typical contribution workflow is:

```bash
git checkout -b feature/my-change
```

Make the change, test it, then:

```bash
git add .
git commit -m "Add my change"
git push -u origin feature/my-change
```

Open a Pull Request and use the project itself to review the change.

---

# Project goal

This project was built to explore a practical question:

> **What would a useful AI-powered first-pass code reviewer look like when integrated directly into a developer workflow?**

The project focuses on making AI review useful, structured, and actionable while keeping human developers in control of the final engineering decision.

---

## Author

**Sumeet Shah**

Software Engineer | C# | .NET | Angular | Azure

GitHub: https://github.com/sumeetshah30

---

## License

See the repository for the applicable license information.
