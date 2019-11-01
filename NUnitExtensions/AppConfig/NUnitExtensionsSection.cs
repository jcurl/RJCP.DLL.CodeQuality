namespace NUnit.Framework.AppConfig
{
    using System.Configuration;

    internal class NUnitExtensionsSection : ConfigurationSection
    {
        private static readonly object s_SettingsLock = new object();
        private static NUnitExtensionsSection s_Settings;

        public static NUnitExtensionsSection Settings
        {
            get
            {
                if (s_Settings == null) {
                    lock (s_SettingsLock) {
                        if (s_Settings == null) {
                            s_Settings = ConfigurationManager.GetSection("NUnitExtensions") as AppConfig.NUnitExtensionsSection;
                        }
                    }
                }
                return s_Settings;
            }
        }

        [ConfigurationProperty("deploy")]
        public Deploy Deploy
        {
            get { return (Deploy)this["deploy"]; }
        }
    }
}
