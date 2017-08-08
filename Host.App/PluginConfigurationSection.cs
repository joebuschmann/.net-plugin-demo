using System.Configuration;
using Host.Contract;

namespace Host.App
{
    public class PluginConfigurationSection : ConfigurationSection
    {
        private const string PluginsName = "plugins";

        private static PluginConfigurationSection _instance = null;

        public static PluginConfigurationSection Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = ConfigurationManager.GetSection("pluginConfiguration") as PluginConfigurationSection;
                }

                return _instance;
            }
        }

        [ConfigurationProperty(PluginsName)]
        public PluginConfigurationCollection Plugins
        {
            get { return (PluginConfigurationCollection)this[PluginsName]; }
        }
    }

    [ConfigurationCollection(typeof(PluginConfigurationElement),
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class PluginConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PluginConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as PluginConfigurationElement).Description;
        }
    }

    public class PluginConfigurationElement : ConfigurationElement
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
