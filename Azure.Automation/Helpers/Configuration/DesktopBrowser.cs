namespace Azure.Automation.Helpers.Configuration
{
    using System.Xml.Serialization;
    using Azure.Automation.Selenium;

    public class DesktopBrowser : Browser
    {
        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("os")]
        public string OperatingSystem { get; set; }

        [XmlAttribute("os_version")]
        public string OperatingSystemVersion { get; set; }

        public override WebDriverTargetBrowser ToWebDriverTargetBrowser(string profileName)
        {
            return WebDriverTargetBrowser.CreateDesktopTargetBrowser(this.Name, this.Version, this.OperatingSystem, this.OperatingSystemVersion, profileName);
        }
    }
}
