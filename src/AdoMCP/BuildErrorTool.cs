using System.Collections.Generic;
using System.Threading.Tasks;
using AdoMCP.Services;

namespace AdoMCP
{
    /// <summary>
    /// Provides build error retrieval functionality for pull requests.
    /// </summary>
    public class BuildErrorTool
    {
        private readonly IBuildErrorService _service;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildErrorTool"/> class.
        /// </summary>
        /// <param name="service">The build error service dependency.</param>
        /// <param name="configuration">The application configuration.</param>
        public BuildErrorTool(
            IBuildErrorService service,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        /// <summary>
        /// Gets build errors for a given pull request as a JSON string.
        /// </summary>
        /// <param name="pullRequestId">The pull request ID.</param>
        /// <returns>A JSON string representing the build errors.</returns>
        public async Task<string> GetBuildErrorsForPullRequestAsync(
            int pullRequestId)
        {
            // Validate required configuration
            string? organization = _configuration["Ado:Organization"];
            if (string.IsNullOrWhiteSpace(organization))
            {
                throw new System.InvalidOperationException("Azure DevOps organization is not configured. Set 'Ado:Organization' in configuration.");
            }

            // (Other config validation can be added here as needed)
            var errors = await _service.GetBuildErrorsAsync(
                organization,
                pullRequestId);
            return System.Text.Json.JsonSerializer.Serialize(errors ?? new List<string>());
        }
    }
}
