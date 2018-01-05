using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.IO.Compression;
using System;

namespace Sitefinity_DevKit.Shared
{
    internal class CliDownloader
    {
        public static void SetUp(string exePath)
        {
            // check if cli is already downloaded
            if (File.Exists(exePath))
            {
                return;
            }

            // construct the release name
            var releaseZipName = Constants.WindowsReleaseName;
            if (Environment.Is64BitOperatingSystem)
            {
                releaseZipName = string.Format("{0}{1}.zip", releaseZipName, "64");
            }
            else
            {
                releaseZipName = string.Format("{0}{1}.zip", releaseZipName, "86");
            }

            var response = Get(Constants.CliLatestReleaseUrl);
            var latestReleaseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Release>(response);
            var assestsUrl = latestReleaseObject.Assets_Url;

            response = Get(assestsUrl);
            var windowsAsset = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Asset>>(response).Where(x => x.Name == releaseZipName).FirstOrDefault();
            var downloadUrl = windowsAsset.Browser_Download_Url;

            // download the release zip file
            var cliDirectoryName = Path.GetDirectoryName(exePath);
            if (Directory.Exists(cliDirectoryName))
            {
                Directory.Delete(cliDirectoryName, true);
            }

            Directory.CreateDirectory(cliDirectoryName);
            var releaseZipPath = Path.Combine(cliDirectoryName, releaseZipName);
            Download(downloadUrl, releaseZipPath);

            // extract and delete the zip
            ZipFile.ExtractToDirectory(releaseZipPath, cliDirectoryName);
            File.Delete(releaseZipPath);
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
    }

    internal class Release
    {
        public string Assets_Url;
    }

    internal class Asset
    {
        public string Name;
        public string Browser_Download_Url;
    }
}
