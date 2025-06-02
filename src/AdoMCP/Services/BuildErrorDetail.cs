namespace AdoMCP.Services
{
    /// <summary>
    /// Represents a build error and its associated stack trace (if any).
    /// </summary>
    public class BuildErrorDetail
    {
        public string ErrorMessage { get; set; }

        public string? StackTrace { get; set; }
    }
}
