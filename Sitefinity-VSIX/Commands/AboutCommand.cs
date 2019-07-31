using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Sitefinity_VSIX.Commands
{
    public class AboutCommand : MenuCommand
    {
        public AboutCommand(EventHandler handler, CommandID command) : base(handler, command)
        {
        }

        public AboutCommand(EventHandler handler, CommandID command, string cliPath) : base(handler, command)
        {
            if (!File.Exists(cliPath))
            {
                throw new FileNotFoundException();
            }

            this.vsixVersion = Assembly.GetExecutingAssembly().GetName().Version;

            var versionInfo = FileVersionInfo.GetVersionInfo(cliPath).FileVersion;

            if (!Version.TryParse(versionInfo, out this.cliVersion))
            {
                throw new InvalidOperationException();
            }
        }

        public Version VsixVersion => this.vsixVersion;
        public Version CliVersion => this.cliVersion;

        public string LicenseInfo => $"Copyright © 2018 - {DateTime.Today.Year.ToString()} Progress Software Corporation and/or one of its subsidiaries or affiliates. All rights reserved.";

        public string AboutMessage => $"Sitefinity VSIX version {this.VsixVersion}\n\r" +
                    $"Sitefinity CLI version {this.CliVersion}\n\r\n\r" +
                    this.LicenseInfo;

        public string AboutTitle => "About Sitefinity VSIX";

        private Version vsixVersion;
        private Version cliVersion;
    }
}
