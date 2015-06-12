namespace Azure.Automation.Helpers.Configuration
{
    using System.Xml.Serialization;
      
    public class BrowserProfiles
    {
        [XmlElement("BrowserProfile")]  
        public BrowserProfile[] Profiles { get; set; }
    }
}
