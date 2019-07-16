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
            var dllPath = cliPath.Replace(".exe", ".dll");
            if (!File.Exists(dllPath))
            {
                throw new FileNotFoundException();
            }

            this.vsixVersion = Assembly.GetExecutingAssembly().GetName().Version;

            var versionInfo = FileVersionInfo.GetVersionInfo(dllPath).FileVersion;

            if (!Version.TryParse(versionInfo, out this.cliVersion))
            {
                throw new InvalidOperationException();
            }
        }

        public Version VsixVersion => this.vsixVersion;
        public Version CliVersion => this.cliVersion;

        public string LicenseInfo => $"Copyright © {DateTime.Today.Year.ToString()} Progress Software Corporation and/or one of its subsidiaries or affiliates. All rights reserved.";

        private Version vsixVersion;
        private Version cliVersion;
    }
}
