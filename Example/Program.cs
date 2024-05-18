using Chase.CrashReporter;

namespace Example;

internal static class Program
{
    public static void Main(string[] args)
    {
        CrashReport.Reporter = CrashHandleBuilder.Create()
            .WriteToFile("crash-reports") // The directory will be created if it does not exist.
            .IncludeStackTrace() // Include the stack trace in the crash report.
            .UseApplicationName("Example") // The name of the application.
            .UseApplicationVersion(new Version(1, 0, 0, 0)) // The version of the application.
            .WriteToConsole() // Write the crash report to the console.
            .WriteToDebug() // Write the crash report to the debug log (this only is used in development environment).
            .UseGithubRepository("Drew-Chase", "Chase.CrashReporter") // The GitHub repository URL, this will be used for the issues page.
            .UseProcessHandles() // Create a hook to catch unhandled exceptions.
            .Build(); // Build the crash report object and set the singleton pattern.

        /*
         * Here is an example on how to throw an exception and generate a crash report.
         * This is just an example and should not be used in production code.
         */
        CrashReport.Report(new IOException("An error occurred while reading the file.")); // Report an exception.
        CrashReport.Report(null); // Report a null exception.
        CrashReport.ReportAndExit(exception: null, exitCode: 89); // Report a null exception and exit the application with an exit code of 89.
        File.ReadAllText("non-existent-file.txt"); // This will throw an exception and generate a crash report.
    }
}