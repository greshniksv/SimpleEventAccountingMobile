using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.RegularExpressions;

namespace VersionUpdater
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Load configuration
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                string csprojFilePath = config["CsprojFilePath"];

                if (string.IsNullOrEmpty(csprojFilePath))
                {
                    PrintError("CsprojFilePath not found in configuration");
                    return;
                }

                // Resolve relative path
                if (!Path.IsPathRooted(csprojFilePath))
                {
                    csprojFilePath = Path.GetFullPath(csprojFilePath);
                }

                // Check if file exists
                if (!File.Exists(csprojFilePath))
                {
                    PrintError($"File not found: {csprojFilePath}");
                    return;
                }

                // Read .csproj file content
                string csprojContent = await File.ReadAllTextAsync(csprojFilePath);

                // Find version tag
                var versionMatch = Regex.Match(csprojContent,
                    @"<Version>(\d+)\.(\d+)\.(\d+)(?:\.(\d+))?</Version>");

                if (!versionMatch.Success)
                {
                    PrintWarning("Version tag not found. Looking for alternative patterns...");

                    // Try alternative patterns
                    versionMatch = Regex.Match(csprojContent,
                        @"<PackageVersion>(\d+)\.(\d+)\.(\d+)(?:\.(\d+))?</PackageVersion>");

                    if (!versionMatch.Success)
                    {
                        versionMatch = Regex.Match(csprojContent,
                            @"<AssemblyVersion>(\d+)\.(\d+)\.(\d+)(?:\.(\d+))?</AssemblyVersion>");
                    }
                }

                if (!versionMatch.Success)
                {
                    PrintError("No version tag found in .csproj file");
                    return;
                }

                // Parse current version
                int major = int.Parse(versionMatch.Groups[1].Value);
                int minor = int.Parse(versionMatch.Groups[2].Value);
                int patch = int.Parse(versionMatch.Groups[3].Value);
                int build = versionMatch.Groups[4].Success ? int.Parse(versionMatch.Groups[4].Value) : 0;

                // Increment minor version
                patch++;
                build = 0; // Reset build version

                string newVersion = $"{major}.{minor}.{patch}";
                string newVersionTag = versionMatch.Value.Replace(
                    versionMatch.Groups[0].Value.Split('>')[1].Split('<')[0],
                    newVersion
                );

                // Replace version in content
                string updatedContent = csprojContent.Replace(versionMatch.Value, newVersionTag);

                // Write updated content
                await File.WriteAllTextAsync(csprojFilePath, updatedContent, Encoding.UTF8);

                // Print success message
                PrintSuccess($"Updated version '{newVersion}' for '{Path.GetFileName(csprojFilePath)}'");
            }
            catch (Exception ex)
            {
                PrintError($"Error: {ex.Message}");
                PrintError($"Stack trace: {ex.StackTrace}");
            }
        }

        // Ubuntu-style console output methods
        static void PrintSuccess(string message)
        {
            Console.Write("[ ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("OK");
            Console.ResetColor();
            Console.Write(" ] ");
            Console.WriteLine($"{message}");
        }

        static void PrintWarning(string message)
        {
            Console.Write("[ ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("WARN");
            Console.ResetColor();
            Console.Write(" ] ");
            Console.WriteLine($"{message}");
        }

        static void PrintError(string message)
        {
            Console.Write("[ ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("ERR");
            Console.ResetColor();
            Console.Write(" ] ");
            Console.WriteLine($"{message}");
        }
    }

    // Configuration class
    public class AppConfig
    {
        public string CsprojFilePath { get; set; }
        public string LogLevel { get; set; }
    }
}