namespace Azure.Automation.Helpers.Configuration
{
    using System.Xml.Serialization;
    using Azure.Automation.Selenium;

    public class MobileBrowser : Browser
    {
        [XmlAttribute("device")]
        public string Device { get; set; }

        [XmlAttribute("os")]
        public string OperatingSystem { get; set; }

        public override WebDriverTargetBrowser ToWebDriverTargetBrowser(string profileName)
        {
            return WebDriverTargetBrowser.CreateMobileTargetBrowser(this.Name, this.OperatingSystem, this.Device, profileName);
        }
    }
}
