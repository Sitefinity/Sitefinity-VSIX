using System.Collections.Generic;
using System.IO;

namespace Sitefinity_VSIX.Shared
{
    public class ConfigParser
    {
        public ConfigParser(string pathToConfigFile)
        {
            this.Parse(pathToConfigFile);
        }

        public List<Command> Commands
        {
            get;
            private set;
        }

        private void Parse(string pathToConfigFile)
        {
            using (StreamReader reader = new StreamReader(pathToConfigFile))
            {
                string content = reader.ReadToEnd();
                this.Commands = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Command>>(content);
            }
        }
    }

    public class Command
    {
        public string Title;
        public string Name;
        public List<string> Args;
        public List<Option> Options;
    }

    public class Option
    {
        public string Title;
        public string Name;
        public string DefaultValue;
    }
}
