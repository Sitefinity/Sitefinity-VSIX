using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Sitefinity_VSIX.Commands;
using System.Threading;
using System.Windows.Threading;
using System.IO;
using Sitefinity_VSIX.Shared;
using System.Diagnostics;

namespace Sitefinity_VSIX
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidPackageString)]
    [ProvideAutoLoad("f1536ef8-92ec-443c-9ed7-fdadf150da82")]
    public sealed class vNextPackage : AsyncPackage
    {
        private ConfigParser configParser;

        protected async override System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            if (await GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                ThreadHelper.Generic.BeginInvoke(DispatcherPriority.ContextIdle, () =>
                {
                    if (commandService != null)
                    {
                        var extentionPath = Path.GetDirectoryName(this.GetType().Assembly.Location);
                        var exePath = Path.Combine(extentionPath, Constants.CLIFolderName, Constants.CLIName);
                        var configPath = Path.Combine(extentionPath, Constants.CLIFolderName, Constants.ConfigFileName);

                        CliDownloader.SetUp(exePath);

                        var fileInfo = new FileInfo(configPath);

                        if (!fileInfo.Exists)
                        {
                            var process = new Process();
                            process.StartInfo.FileName = fileInfo.Name;
                            process.StartInfo.WorkingDirectory = fileInfo.DirectoryName;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.CreateNoWindow = true;
                            process.StartInfo.FileName = exePath;
                            process.StartInfo.Arguments = "config";
                            process.Start();
                            process.WaitForExit();
                        }
                        
                        this.configParser = new ConfigParser(configPath);
                        var dynamicCommandRootId = new CommandID(PackageGuids.guidPackageCommandSet, PackageIds.DynamicCommandId);
                        var dynamicCommand = new DynamicCommand(dynamicCommandRootId, IsValidDynamicItem, OnInvokedDynamicItem, OnBeforeQueryStatusDynamicItem);
                        commandService.AddCommand(dynamicCommand);
                    }
                });
            }
        }

        private void OnInvokedDynamicItem(object sender, EventArgs e)
        {
            DynamicCommand invokedCommand = (DynamicCommand)sender;
            var commandConfig = this.configParser.Commands.Find(c => c.Title.Equals(invokedCommand.Text));
            var currentProjectPath = VSHelpers.GetCurrentProjectPath();

            var extentionPath = Path.GetDirectoryName(this.GetType().Assembly.Location);
            var exePath = Path.Combine(extentionPath, Constants.CLIFolderName, Constants.CLIName);
            var fileInfo = new FileInfo(exePath);

            if (!fileInfo.Exists)
            {
                string message = "File 'sf.exe' does not exist!";
                VSHelpers.ShowErrorMessage(this, message, commandConfig.Title);
                return;
            }

            var args = String.Format("{0}", commandConfig.Name);

            // get arguments
            var dialog = new InputDialog(commandConfig);
            if (dialog.ShowDialog() == true)
            {
                for (int i = 0; i < dialog.ResponseText.Count; i++)
                {
                    var input = dialog.ResponseText[i];
                    if (string.IsNullOrEmpty(input) || input.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
                    {
                        string message = string.Format("Invalid argument: {0}!", commandConfig.Args[i]);
                        VSHelpers.ShowErrorMessage(this, message, commandConfig.Title);
                        return;
                    }

                    // response is argument, else - response is option
                    if (i < commandConfig.Args.Count)
                    {
                        args = String.Format("{0} \"{1}\"", args, input);
                    }
                    else
                    {
                        var optionIndex = i - commandConfig.Args.Count;
                        args = String.Format("{0} {1} \"{2}\"", args, commandConfig.Options[optionIndex].Name, input);
                    }
                }

                args = String.Format("{0} -r \"{1}\"", args, currentProjectPath);

                var process = new Process();
                process.StartInfo.FileName = fileInfo.Name;
                process.StartInfo.WorkingDirectory = fileInfo.DirectoryName;
                process.StartInfo.Arguments = args;
                process.Start();
            }
        }

        private void OnBeforeQueryStatusDynamicItem(object sender, EventArgs args)
        {
            OleMenuCommand matchedCommand = (OleMenuCommand)sender;
            matchedCommand.Enabled = true;
            matchedCommand.Visible = true;
            
            bool isRootItem = (matchedCommand.MatchedCommandId == 0);
            int commandIndex = isRootItem ? 0 : (matchedCommand.MatchedCommandId - (int)PackageIds.DynamicCommandId);
            
            matchedCommand.Text = configParser.Commands[commandIndex].Title;

            // Clear the ID because we are done with this item.  
            matchedCommand.MatchedCommandId = 0;
        }

        private bool IsValidDynamicItem(int commandId)
        {
            return PackageIds.DynamicCommandId <= commandId && commandId <= (PackageIds.DynamicCommandId + configParser.Commands.Count - 1);
        }
    }
}
