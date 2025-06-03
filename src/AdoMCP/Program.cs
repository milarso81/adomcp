using AdoMCP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Register core services for dependency injection
builder.Services.AddSingleton<IAdoPullRequestService, AdoSdkPullRequestService>();

builder.Services.AddSingleton<PullRequestTool>();

builder.Services.AddSingleton<PullRequestChangeTool>();

// Register build error service and tool
builder.Services.AddSingleton<IBuildErrorService, AdoSdkBuildErrorService>();

builder.Services.AddSingleton<BuildErrorTool>();

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
