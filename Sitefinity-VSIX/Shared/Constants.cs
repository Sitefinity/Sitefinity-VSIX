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

        public const string UnauthorizedErrorMessage = "Unable to create config file. Check your permissions";
        public const string UnauthorizedErrorTitle = "Unauthorized";
        public const string GeneralErrorMessage = "Unable to create config file.";
        public const string GeneralErrorTitle = "Something happened";
    }
}
