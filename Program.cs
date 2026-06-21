using AIPRReviewer;
using DotNetEnv;

Env.Load();

int? prNumber = args.Length > 0 ? int.Parse(args[0]) : null;

var orchestrator = new Orchestrator();
await orchestrator.RunAsync(prNumber);