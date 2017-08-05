using System.Configuration;
using Host.Contract;

namespace Host.App
{
    public class ApplicationConfigurationSection : ConfigurationSection
    {
        private const string ApplicationsName = "applications";

        private static readonly ApplicationConfigurationSection _instance
            = ConfigurationManager.GetSection("applicationConfiguration") as ApplicationConfigurationSection;

        public static ApplicationConfigurationSection Instance
        {
            get
            {
                return _instance;
            }
        }

        [ConfigurationProperty(ApplicationsName)]
        public ApplicationConfigurationCollection Applications
        {
            get { return (ApplicationConfigurationCollection)this[ApplicationsName]; }
        }
    }

    [ConfigurationCollection(typeof(ApplicationConfigurationElement),
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ApplicationConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ApplicationConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ApplicationConfigurationElement).Description;
        }
    }

    public class ApplicationConfigurationElement : ConfigurationElement
    {
        private const string DescriptionName = "description";
        private const string BaseDirectoryName = "baseDirectory";
        private const string ConfigurationFileName = "configurationFile";
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

        [ConfigurationProperty(ConfigurationFileName, IsRequired = false)]
        public string ConfigurationFile
        {
            get
            {
                return (string)this[ConfigurationFileName];
            }
            set
            {
                this[ConfigurationFileName] = value;
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
        [SubclassTypeValidator(typeof(PluginService))]
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
