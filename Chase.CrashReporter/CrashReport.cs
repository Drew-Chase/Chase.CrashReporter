using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Chase.CrashReporter;

/// <summary>
/// Represents a crash report handler that allows reporting and handling application crashes.
/// </summary>
public partial class CrashReport
{
    /// <summary>
    /// Represents a crash report handler.
    /// </summary>
    public static CrashReport? Reporter { private get; set; }

    /// <summary>
    /// Represents the options for the CrashReportHandler.
    /// </summary>
    private readonly CrashReportHandlerOptions _options;

    /// <summary>
    /// Represents a crash report handler that captures and writes
    /// exception information to a specified output.
    /// </summary>
    internal CrashReport(CrashReportHandlerOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Writes a crash report based on the provided exception.
    /// </summary>
    /// <param name="exception">The exception to write the crash report for.</param>
    public static void Report(Exception? exception)
    {
        Reporter?.WriteReport(exception);
    }

    /// <summary>
    /// Writes a crash report and terminates the application.
    /// </summary>
    /// <param name="exception">The exception that caused the crash, or null if no exception occurred.</param>
    /// <param name="exitCode">The exit code for the application.</param>
    public static void ReportAndExit(Exception? exception, int exitCode = 1)
    {
        Reporter?.WriteReport(exception);
        Environment.Exit(exitCode);
    }

    /// <summary>
    /// Writes a crash report based on the given exception and the configured options.
    /// </summary>
    /// <param name="ex">The exception that triggered the crash report.</param>
    private void WriteReport(Exception? ex)
    {
        string formattedTime = DateTime.Now.ToString("dddd MMMM dd, yyyy - hh-mm-ss.fff tt");
        StringBuilder report = new StringBuilder();
        report.AppendLine($"Crash Report - {formattedTime}");


        report.AppendLine($"{_options.ApplicationName} Crash Report - {formattedTime}".Trim());
        if (_options.GithubRepositoryUrl is not null && Uri.TryCreate(_options.GithubRepositoryUrl, UriKind.Absolute, out Uri? githubUri))
        {
            // append /issues/new to the end of the url
            githubUri = !githubUri.AbsolutePath.EndsWith('/') ? new Uri(githubUri, $"{githubUri.AbsolutePath}/issues/new") : new Uri(githubUri, "issues/new");

            // Add the title and body to the url
            githubUri = new Uri(githubUri, $"?title={Uri.EscapeDataString($"{_options.ApplicationName} Crash Report - {formattedTime}")}&body={Uri.EscapeDataString("Please describe what you were doing when the crash occurred.")}");

            report.AppendLine($"If you believe this is a bug please report it to our issues page: {githubUri.ToString().Replace(" ", "%20")}");
        }

        report.AppendLine("\nApplication Data:");
        report.AppendLine($"\tOS: {Environment.OSVersion.VersionString}");
        report.AppendLine($"\tVersion: {_options.ApplicationVersion}");
        report.AppendLine("\nCrash Data:");
        report.AppendLine($"\tMessage: {ex?.Message}");
        report.AppendLine($"\tSource: {ex?.Source}");
        report.AppendLine($"\tData: {(ex is null ? "null" : JsonConvert.SerializeObject(ex?.Data))}");
        if (_options.WriteStackTrace)
            report.AppendLine($"Stack Trace:\n{ex?.StackTrace}");


        string output = report.ToString();
        if (_options.WriteToConsole)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(output);
            Console.ResetColor();
        }

        if (_options.WriteToFile)
        {
            string filename = $"crash-report-{formattedTime}.txt";
            if (!Directory.Exists(_options.OutputDirectory))
                Directory.CreateDirectory(_options.OutputDirectory);
            string file = Path.Combine(_options.OutputDirectory, filename);
            File.WriteAllText(file, output);
        }
#if DEBUG
        if (_options.WriteToDebugLog)
        {
            System.Diagnostics.Debug.WriteLine(output);
        }
#endif
    }
}