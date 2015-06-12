namespace Azure.Automation.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Singleton pattern code grouping")]
    public class TestConfiguration
    {
        #region Static access

        private static TestConfiguration instance;

        private List<string> configurationLog = new List<string>();

        public static TestConfiguration Instance
        {
            get
            {
                return instance = instance ?? new TestConfiguration();
            }

            protected set
            {
                instance = value;
            }
        }

        #endregion

        public TestConfiguration()
        {
            this.EnvironmentUrl = this.LoadSetting("EnvironmentUrl", true);
            this.BrowserStackKey = this.LoadSetting("BrowserStackKey");
            this.BrowserStackUser = this.LoadSetting("BrowserStackUser", true);
            this.BrowserProfiles = this.LoadSetting("BrowserProfiles", true);
            this.Project = this.LoadSetting("Project");
            this.SeleniumServerHubUrl = this.LoadSetting("SeleniumServerHubUrl");
            this.IncludeUnsupportedBrowsers = bool.Parse(this.LoadSetting("IncludeUnsupportedBrowsers", true));
            this.LogTimings = bool.Parse(this.LoadSetting("LogTimings"));
            this.ProxyAddress = this.LoadSetting("ProxyAddress");
            this.DisableElementNavigation = bool.Parse(this.LoadSetting("DisableElementNavigation"));
        }

        public string EnvironmentUrl { get; private set; }

        public string BrowserStackUser { get; private set; }

        public string BrowserStackKey { get; private set; }

        public string BrowserProfiles { get; private set; }

        public string Project { get; private set; }

        public string SeleniumServerHubUrl { get; private set; }

        public bool IncludeUnsupportedBrowsers { get; private set; }

        public bool LogTimings { get; private set; }

        public string ProxyAddress { get; set; }

        public bool DisableElementNavigation { get; set; }

        public void LogSettings()
        {
            foreach (var line in this.configurationLog)
            {
                Logger.Instance.Log("CONFIGURATION", line);
            }
        }

        private string LoadSetting(string settingName, bool logSetting = false)
        {
            var value = ConfigurationManager.AppSettings[settingName];

            if (logSetting)
            {
                this.configurationLog.Add(string.Format("{0}: '{1}'", settingName, value));
            }

            return value;
        }
    }
}
