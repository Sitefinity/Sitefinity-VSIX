namespace Sitefinity_VSIX.Shared
{
    internal class Constants
    {
        public const string DotNetCoreProcessName = "dotnet";

        public const string CLIFolderName = "cli";
        public const string CLIName = "sf.dll";
        public const string ConfigFileName = "config.json";

        public const string ResourcePackagesFolderName = "ResourcePackages";

        public const string CliReleasesUrl = "https://api.github.com/repos/Sitefinity/Sitefinity-CLI/releases";
        public const string WindowsReleaseName = "sf-cli-win-x86.zip";

        public const string ConfigGeneralErrorMessage = "Sitefinity VSIX cannot create a CLI config file in the Extensions directory on your file system.";
        public const string ConfigGeneralErrorTitle = "";

        public const string ConfigPermissionsErrorMessage = "You do not have sufficient permissions to create a CLI config file in the Extensions directory on your file system. Check the Extensions directory security properties or contact your administrator for assistance.";
        public const string ConfigPermissionsErrorTitle = "";
    }
}
