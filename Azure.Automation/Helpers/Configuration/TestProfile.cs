namespace Azure.Automation.Helpers.Configuration
{
    using System.Xml.Serialization;

    public class BrowserProfile
    {
        [XmlElement("MobileBrowser", typeof(MobileBrowser))]
        [XmlElement("DesktopBrowser", typeof(DesktopBrowser))]
        public Browser[] Browsers { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
