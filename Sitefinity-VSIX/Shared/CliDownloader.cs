using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.IO.Compression;
using System;
using System.Reflection;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Sitefinity_VSIX.Shared
{
    internal class CliDownloader
    {
        public static void SetUp(string cliPath)
        {
            // Get current version
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            // Get CLI latest version for this major
            var releasesResponse = Get(Constants.CliReleasesUrl);
            var releases = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Release>>(releasesResponse);
            var latestRelease = releases.Where(x => !x.Prerelease && ParseReleaseMajorVersion(x.Version).Major == currentVersion.Major).OrderByDescending(x => x.PublishDate).FirstOrDefault();

            if (latestRelease != null)
            {
                var latestReleaseVersion = ParseReleaseMajorVersion(latestRelease.Version);

                // check if cli is already downloaded or its current version is lower than the latest release version
                if (File.Exists(cliPath))
                {
                    // get cli version
                    var versionInfo = FileVersionInfo.GetVersionInfo(cliPath);
                    Version cliVersion = Version.Parse(versionInfo.FileVersion);

                    if (cliVersion.CompareTo(latestReleaseVersion) < 0)
                    {
                        HandleCliDownloadAndExtraction(latestRelease, cliPath);
                    }
                }
                else
                {
                    HandleCliDownloadAndExtraction(latestRelease, cliPath);
                }
            }
        }

        private static string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            // Error occurs if this is not set because of GitHub API
            request.UserAgent = "some agent";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private static void Download(string downloadUri, string targetLocation)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(downloadUri, targetLocation);
            }
        }

        private static void HandleCliDownloadAndExtraction(Release release, string cliPath)
        {     
            var asset = release.Assets.Where(x => x.Name == Constants.WindowsReleaseName).FirstOrDefault();
            if (asset != null)
            {
                var cliDirectoryName = Path.GetDirectoryName(cliPath);

                if (Directory.Exists(cliDirectoryName))
                {
                    Directory.Delete(cliDirectoryName, true);
                }

                Directory.CreateDirectory(cliDirectoryName);
                var releaseZipPath = Path.Combine(cliDirectoryName, Constants.WindowsReleaseName);
                Download(asset.DownloadUrl, releaseZipPath);

                // extract and delete the zip
                ZipFile.ExtractToDirectory(releaseZipPath, cliDirectoryName);
                File.Delete(releaseZipPath);
            }
        }

        private static Version ParseReleaseMajorVersion(string version)
        {
            var regex = new Regex(@"(\d+\.)(\d+\.)(\d+\.)(\d)");
            return Version.Parse(regex.Match(version).Value);
        }
    }

    internal class Release
    {
        [JsonProperty(PropertyName = "tag_name")]
        public string Version;

        [JsonProperty(PropertyName = "published_at")]
        public DateTime PublishDate;

        public List<Asset> Assets;

        public bool Prerelease;
    }

    internal class Asset
    {
        public string Name;

        [JsonProperty(PropertyName = "browser_download_url")]
        public string DownloadUrl;
    }
}