# Project Guide — AI PR Review Agent

This document provides a practical overview of the AI PR Review Agent for developers who want to understand, run, extend, or integrate the project.

---

## 1. Project purpose

The AI PR Review Agent is an automated first-pass Pull Request reviewer.

It connects a GitHub Pull Request to an AI code-review workflow and publishes useful feedback back to the Pull Request.

The intended role is:

```text
AI reviewer = fast first-pass feedback
Human reviewer = final engineering decision
```

The system should therefore be considered an assistant rather than an autonomous merge authority.

---

## 2. Core workflow

The application follows a simple pipeline:

```text
PR
 |
 v
Fetch changes
 |
 v
Analyze code with AI
 |
 v
Structure review
 |
 v
Publish feedback
```

The orchestration layer coordinates these stages.

---

## 3. Main components

### FetcherAgent

The Fetcher Agent obtains Pull Request information and changed files from GitHub.

Its responsibilities include:

- Identifying the target Pull Request
- Retrieving PR metadata
- Retrieving changed files
- Preparing information for the review stage

It should remain focused on GitHub retrieval rather than AI reasoning.

---

### ReviewerAgent

The Reviewer Agent is responsible for AI-powered analysis.

Typical review concerns include:

- Bugs
- Runtime risks
- Null handling
- Validation
- Security concerns
- Maintainability
- Testing
- Code quality
- Edge cases

The reviewer should produce structured information that can be consumed by the publishing stage.

---

### CommenterAgent

The Commenter Agent takes the review result and publishes feedback to GitHub.

Its responsibilities include:

- Posting the main review
- Posting issue-level feedback
- Associating feedback with relevant changed files where supported

Keeping publishing separate from AI analysis makes the system easier to test and modify.

---

### Orchestrator

The Orchestrator coordinates the agents.

Conceptually:

```text
Orchestrator
    |
    +--> FetcherAgent
    |
    +--> ReviewerAgent
    |
    +--> CommenterAgent
```

This keeps the application flow easy to understand.

---

## 4. Running a review

After configuring the environment:

```bash
dotnet restore
dotnet build
```

Run the application:

```bash
dotnet run
```

To target a specific PR:

```bash
dotnet run -- <PR_NUMBER>
```

Example:

```bash
dotnet run -- 2
```

---

## 5. Environment configuration

The application expects environment-based configuration.

Use placeholders such as:

```env
ANTHROPIC_API_KEY=your_ai_api_key
GITHUB_TOKEN=your_github_token
GITHUB_REPO=owner/repository
```

Never put real credentials in this document.

### Local development

A `.env` file can be used locally when it is excluded from Git.

### CI/CD

For CI/CD, prefer platform secret storage such as:

- GitHub Actions Secrets
- Azure Key Vault
- Environment-level secret stores
- Enterprise secret-management platforms

---

## 6. Security considerations

### Protect API keys

AI provider keys can incur cost and should be treated as sensitive credentials.

### Protect GitHub tokens

Use the minimum permissions required for the intended GitHub operations.

### Do not expose secrets

Never place credentials in:

- Source files
- README files
- Documentation
- Screenshots
- Commit messages
- Pull Request descriptions
- Logs

### Review data privacy

Before using the agent with private or sensitive repositories, understand the data-processing policies of the configured AI provider and organization.

Only send the code and context necessary for the review.

---

## 7. Recommended review categories

A useful reviewer can be organized around several categories.

### Correctness

Look for:

- Incorrect logic
- Exceptions
- Null handling
- Invalid assumptions
- Boundary conditions

### Security

Look for:

- Unsafe input
- Injection risks
- Authorization problems
- Credential exposure
- Insecure configuration

### Performance

Look for:

- Unnecessary expensive operations
- Repeated database/API calls
- Inefficient loops
- Excessive allocations

### Maintainability

Look for:

- Complex methods
- Duplication
- Poor naming
- Hard-to-test code
- Unclear abstractions

### Testing

Look for:

- Missing tests
- Missing edge cases
- Regression risks
- Incorrect test assumptions

---

## 8. Review quality

Not every AI suggestion should automatically become a code change.

A good developer workflow is:

```text
AI identifies issue
       ↓
Developer verifies issue
       ↓
Developer checks application context
       ↓
Developer decides whether change is appropriate
       ↓
Tests validate the change
```

This is particularly important for architectural and stylistic recommendations.

---

## 9. Testing strategy

The project can be tested using deliberately flawed Pull Requests.

Useful test cases include:

### Null reference

```csharp
public string GetUserName(string userName)
{
    return userName.ToUpper();
}
```

Potential concern:

- `userName` can be null.

---

### Division by zero

```csharp
public int Divide(int a, int b)
{
    return a / b;
}
```

Potential concern:

- `b == 0`.

---

### Missing validation

```csharp
public void CreateUser(string email)
{
    // Process input directly
}
```

Potential concerns depend on the surrounding application.

---

### Security-sensitive code

Test with controlled examples of unsafe input handling and verify whether the reviewer recognizes the risk.

---

## 10. Extending the project

Potential extension points include:

### Additional review agents

Examples:

```text
SecurityAgent
PerformanceAgent
TestingAgent
ArchitectureAgent
```

The orchestrator could run specialized reviewers and combine their findings.

---

### Configurable review profiles

For example:

```text
General
Security
Performance
.NET
API
Database
Testing
```

A repository could select the appropriate profile.

---

### CI/CD integration

A GitHub Actions workflow could trigger the application whenever a PR is opened or updated.

The workflow could eventually:

1. Start the reviewer.
2. Analyze the PR.
3. Publish comments.
4. Publish a status/check.
5. Optionally fail a check for configured critical issues.

Any automatic blocking policy should be carefully validated to avoid false positives.

---

## 11. Production architecture

A larger deployment could look like:

```text
GitHub
  |
  | Pull Request event
  v
Webhook / GitHub Actions
  |
  v
Review Service
  |
  +---- GitHub API
  |
  +---- AI Provider
  |
  v
Review Result
  |
  +---- PR comments
  |
  +---- Status check
  |
  +---- Metrics/logging
```

This separates the current command-line workflow from a future continuously running service.

---

## 12. Observability

For production usage, useful metrics include:

- Number of PRs reviewed
- Review duration
- Number of findings
- Findings by severity
- Developer acceptance rate
- False-positive rate
- AI/API failures
- GitHub API failures

Avoid logging:

- API keys
- Access tokens
- Full sensitive source files
- Sensitive user information

---

## 13. Practical use cases

### Small development teams

Use the agent as an additional review layer when reviewers are busy.

### Large engineering teams

Use it to apply consistent first-pass checks across repositories.

### Open-source projects

Use it to provide automated feedback on external contributions.

### Learning environments

Use intentional bugs to demonstrate how different coding problems are detected.

### Internal engineering platforms

Integrate specialized review profiles into an organization's existing development workflow.

---

## 14. What the agent should not do

The system should not be treated as:

- A replacement for human review
- A guaranteed security scanner
- A compiler or static analyzer replacement
- A substitute for unit/integration tests
- An autonomous merge decision-maker

AI review is strongest when combined with traditional engineering controls.

---

## 15. Development philosophy

The project follows a simple principle:

> Automate repetitive analysis while keeping engineering judgment with developers.

A successful implementation should make developers faster without creating blind trust in automated output.

---

## 16. Suggested roadmap

### Phase 1 — Current foundation

- GitHub PR retrieval
- AI code analysis
- Structured findings
- GitHub feedback

### Phase 2 — Automation

- GitHub Actions
- Automatic PR triggers
- Status checks
- Configurable severity thresholds

### Phase 3 — Specialized reviews

- Security review
- Performance review
- Testing review
- .NET-specific review

### Phase 4 — Enterprise capabilities

- Repository policies
- Central configuration
- Metrics
- Review history
- Multiple AI providers
- Secure secret management
- Organization-wide deployment

---

## 17. Final principle

The best use of AI code review is not:

```text
AI says it is correct → merge
```

It is:

```text
AI finds potential issues
        ↓
Developer evaluates them
        ↓
Automated tests run
        ↓
Human reviewer approves
        ↓
Merge
```

That approach keeps automation useful while preserving software-engineering accountability.
