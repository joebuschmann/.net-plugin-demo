using System.Configuration;

namespace Host.App.Configuration
{
    public class PluginConfigurationElement : ConfigurationElement, IPluginConfigurationItem
    {
        private const string DescriptionName = "description";
        private const string BaseDirectoryName = "baseDirectory";
        private const string AppConfigFileName = "appConfigFile";
        private const string AssemblyName = "assemblyName";
        private const string TypeName = "typeName";

        [ConfigurationProperty(DescriptionName, IsRequired = true)]
        public string Description
        {
            get
            {
                return (string)this[DescriptionName];
            }
            set
            {
                this[DescriptionName] = value;
            }
        }

        [ConfigurationProperty(BaseDirectoryName, IsRequired = true)]
        public string BaseDirectory
        {
            get
            {
                return (string)this[BaseDirectoryName];
            }
            set
            {
                this[BaseDirectoryName] = value;
            }
        }

        [ConfigurationProperty(AppConfigFileName, IsRequired = false)]
        public string ConfigurationFile
        {
            get
            {
                return (string)this[AppConfigFileName];
            }
            set
            {
                this[AppConfigFileName] = value;
            }
        }

        [ConfigurationProperty(AssemblyName, IsRequired = true)]
        public string Assembly
        {
            get
            {
                return (string)this[AssemblyName];
            }
            set
            {
                this[AssemblyName] = value;
            }
        }

        [ConfigurationProperty(TypeName, IsRequired = true)]
        public string Type
        {
            get
            {
                return (string)this[TypeName];
            }
            set
            {
                this[TypeName] = value;
            }
        }
    }
}