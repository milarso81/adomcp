using Microsoft.Extensions.Configuration;
using AdoMCP;

namespace BuildErrorDebugger;

/// <summary>
/// Console application for debugging build error retrieval with real Azure DevOps data.
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        IBuildErrorService service = new AdoSdkBuildErrorService(configuration);
        BuildErrorTool tool = new BuildErrorTool(service, configuration); if (args.Length < 2)
        {
            Console.WriteLine("Usage: BuildErrorDebugger <pullRequestId> <branchName>");
            return;
        }

        if (!int.TryParse(args[0], out int pullRequestId))
        {
            Console.WriteLine("Invalid pull request ID.");
            return;
        }

        string branchName = args[1];

        string result = await tool.GetBuildErrorsForPullRequestAsync(pullRequestId);
        Console.WriteLine(result);
    }
}
