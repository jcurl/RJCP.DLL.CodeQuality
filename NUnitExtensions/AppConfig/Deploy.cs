namespace NUnit.Framework.AppConfig
{
    using System.Configuration;

    internal sealed class Deploy : ConfigurationElement
    {
        [ConfigurationProperty("workDir", IsRequired = false, DefaultValue = "")]
        public string DeployWorkDirectory
        {
            get { return (string)this["workDir"]; }
        }

        [ConfigurationProperty("useCwd", IsRequired = false, DefaultValue = false)]
        public bool UseCurrentDirectory
        {
            get { return (bool)this["useCwd"]; }
        }

        [ConfigurationProperty("force", IsRequired = false, DefaultValue = false)]
        public bool Force
        {
            get { return (bool)this["force"]; }
        }
    }
}
