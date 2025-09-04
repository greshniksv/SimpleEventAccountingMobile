using System.Xml;

namespace FileRenamer
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                // Validate parameters
                if (args.Length != 2)
                {
                    Console.WriteLine("Usage: FileRenamer <path-to-csproj> <path-to-file>");
                    Console.WriteLine("Example: FileRenamer myproject.csproj dataFile.apk");
                    return 1;
                }

                string csprojPath = args[0];
                string filePath = args[1];

                // Validate file paths
                if (!File.Exists(csprojPath))
                {
                    Console.WriteLine($"Error: CSProj file not found: {csprojPath}");
                    return 1;
                }

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Error: File to rename not found: {filePath}");
                    return 1;
                }

                // Extract version from csproj
                string version = ExtractVersionFromCsproj(csprojPath);
                if (string.IsNullOrEmpty(version))
                {
                    Console.WriteLine("Error: Version tag not found in csproj file");
                    return 1;
                }

                //Console.WriteLine($"Found version: {version}");

                // Rename the file
                string newFilePath = RenameFileWithVersion(filePath, version);

                Console.Write("[ ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("OK");
                Console.ResetColor();
                Console.Write(" ] ");
                Console.Write("File renamed successfully:");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(newFilePath);
                Console.ResetColor();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        static string ExtractVersionFromCsproj(string csprojPath)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(csprojPath);

                // Look for Version in PropertyGroup (for SDK-style projects)
                XmlNode versionNode = doc.SelectSingleNode("//Project/PropertyGroup/Version");
                if (versionNode != null)
                {
                    return versionNode.InnerText.Trim();
                }

                // Look for Version in PropertyGroup with TargetFramework (for older projects)
                versionNode = doc.SelectSingleNode("//Project/PropertyGroup[TargetFramework]/Version");
                if (versionNode != null)
                {
                    return versionNode.InnerText.Trim();
                }

                // Look for AssemblyVersion (for very old projects)
                versionNode = doc.SelectSingleNode("//Project/PropertyGroup/AssemblyVersion");
                if (versionNode != null)
                {
                    return versionNode.InnerText.Trim();
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse csproj file: {ex.Message}");
            }
        }

        static string RenameFileWithVersion(string originalFilePath, string version)
        {
            string directory = Path.GetDirectoryName(originalFilePath);
            string fileName = Path.GetFileNameWithoutExtension(originalFilePath);
            string extension = Path.GetExtension(originalFilePath);

            // Clean version string for filename (replace invalid characters)
            string cleanVersion = version.Replace(' ', '_').Replace(':', '_').Replace('\\', '_').Replace('/', '_');

            // Construct new filename
            string newFileName = $"{fileName} v{cleanVersion}{extension}";
            string newFilePath = Path.Combine(directory, newFileName);

            // Check if new file already exists
            if (File.Exists(newFilePath))
            {
                throw new Exception($"Target file already exists: {newFilePath}");
            }

            // Perform the rename
            File.Move(originalFilePath, newFilePath);

            return newFilePath;
        }
    }
}