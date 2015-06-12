namespace Azure.Automation.Helpers.Configuration
{
    using System.Xml.Serialization;
    using Azure.Automation.Selenium;

    public abstract class Browser
    {
        [XmlAttributeAttribute("name")]
        public string Name { get; set; }

        public abstract WebDriverTargetBrowser ToWebDriverTargetBrowser(string profileName);            
    }
}
