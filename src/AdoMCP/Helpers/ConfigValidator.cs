using Microsoft.Extensions.Configuration;

namespace AdoMCP
{
    /// <summary>
    /// Provides helper methods for configuration validation.
    /// </summary>
    internal static class ConfigValidator
    {
        /// <summary>
        /// Gets a required configuration value or throws InvalidOperationException if missing or empty.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="key">The configuration key.</param>
        /// <param name="message">The exception message if missing.</param>
        /// <returns>The configuration value.</returns>
        public static string GetSetting(
            this IConfiguration configuration,
            string key,
            string message)
        {
            string? value = configuration[key];
            return string.IsNullOrWhiteSpace(value) ?
                throw new InvalidOperationException(message) : value;
        }
    }
}
