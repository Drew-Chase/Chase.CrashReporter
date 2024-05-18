using System.Diagnostics;

namespace Chase.CrashReporter;

/// <summary>
/// Represents a builder class for configuring and building a CrashReport object.
/// </summary>
public class CrashHandleBuilder(CrashReportHandlerOptions options)
{
    /// <summary>
    /// Represents the options for creating a crash report handle.
    /// </summary>
    private readonly CrashReportHandlerOptions _options = options;

    /// <summary>
    /// Creates a new instance of the <see cref="CrashHandleBuilder"/> class with the provided options.
    /// </summary>
    /// <param name="options">The <see cref="CrashReportHandlerOptions"/> to configure the builder.</param>
    /// <returns>A new instance of the <see cref="CrashHandleBuilder"/> class.</returns>
    public static CrashHandleBuilder Create(CrashReportHandlerOptions? options = null)
    {
        return new CrashHandleBuilder(options ?? new CrashReportHandlerOptions());
    }

    /// <summary>
    /// Enable including the stack trace in the crash report.
    /// </summary>
    /// <returns>A reference to the <see cref="CrashHandleBuilder"/> instance.</returns>
    public CrashHandleBuilder IncludeStackTrace()
    {
        _options.WriteStackTrace = true;
        return this;
    }

    /// <summary>
    /// Sets the application name for the crash report.
    /// </summary>
    /// <param name="name">The name of the application.</param>
    /// <returns>The <see cref="CrashHandleBuilder"/> instance.</returns>
    public CrashHandleBuilder UseApplicationName(string name)
    {
        _options.ApplicationName = name;
        return this;
    }

    /// <summary>
    /// Configures the application version for crash reporting.
    /// </summary>
    /// <param name="version">The application version.</param>
    /// <returns>The current instance of <see cref="CrashHandleBuilder"/>.</returns>
    public CrashHandleBuilder UseApplicationVersion(Version version)
    {
        _options.ApplicationVersion = version;
        return this;
    }

    /// <summary>
    /// Enables writing the crash report to the console.
    /// </summary>
    /// <returns>A reference to the current <see cref="CrashHandleBuilder"/> instance.</returns>
    public CrashHandleBuilder WriteToConsole()
    {
        _options.WriteToConsole = true;
        return this;
    }

    /// <summary>
    /// Sets the option to write the crash report to the debug log.
    /// </summary>
    /// <returns>The <see cref="CrashHandleBuilder"/> instance.</returns>
    public CrashHandleBuilder WriteToDebug()
    {
        _options.WriteToDebugLog = true;
        return this;
    }

    /// <summary>
    /// Method to set the GitHub repository URL for the crash report.
    /// This is used to provide a link to the issues page in the crash report.
    /// </summary>
    /// <param name="author">The author of the GitHub repository.</param>
    /// <param name="repository">The name of the GitHub repository.</param>
    /// <returns>The current instance of <see cref="CrashHandleBuilder"/> with the updated GitHub repository URL.</returns>
    public CrashHandleBuilder UseGithubRepository(string author, string repository)
    {
        _options.GithubRepositoryUrl = $"https://github.com/{author}/{repository}";
        return this;
    }

    /// <summary>
    /// Writes the crash report to a file.
    /// The directory will be automatically created when a report is written.
    /// </summary>
    /// <param name="directory">The directory where the file will be saved. If not provided, it will use the base directory of the application.</param>
    /// <returns>The current instance of the <see cref="CrashHandleBuilder"/>.</returns>
    public CrashHandleBuilder WriteToFile(string directory = "")
    {
        _options.OutputDirectory = directory;
        _options.WriteToFile = true;
        return this;
    }

    /// <summary>
    /// Configures the process handles for handling unhandled exceptions.
    /// </summary>
    /// <returns>The current instance of <see cref="CrashHandleBuilder"/>.</returns>
    public CrashHandleBuilder UseProcessHandles()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception exception)
                CrashReport.Report(exception);
            else
                CrashReport.Report(null);
        };

        return this;
    }


    /// <summary>
    /// Builds a <see cref="CrashReport"/> instance with the specified options.
    /// </summary>
    /// <returns>A new instance of <see cref="CrashReport"/>.</returns>
    public CrashReport Build()
    {
        return new CrashReport(_options);
    }
}