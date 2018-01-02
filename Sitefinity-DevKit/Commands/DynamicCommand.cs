using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace Sitefinity_DevKit.Commands
{
    internal sealed class DynamicCommand : OleMenuCommand
    {
        private Predicate<int> matches;

        public DynamicCommand(CommandID rootId, Predicate<int> matches, EventHandler invokeHandler, EventHandler beforeQueryStatusHandler)
            : base(invokeHandler, null, beforeQueryStatusHandler, rootId)
        {
            if (matches == null)
            {
                throw new ArgumentNullException("matches");
            }

            this.matches = matches;
        }

        public override bool DynamicItemMatch(int cmdId)
        { 
            if (this.matches(cmdId))
            {
                this.MatchedCommandId = cmdId;
                return true;
            }

            this.MatchedCommandId = 0;
            return false;
        }
    }
}
