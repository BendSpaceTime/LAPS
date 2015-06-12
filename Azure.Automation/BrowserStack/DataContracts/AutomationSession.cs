namespace Azure.Automation.BrowserStack.DataContracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class AutomationSession
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "logs")]
        public string LogsUrl { get; set; }

        [DataMember(Name = "browser")]
        public string Browser { get; set; }

        [DataMember(Name = "browser_version")]
        public string BrowserVersion { get; set; }

        [DataMember(Name = "os")]
        public string OperatingSystem { get; set; }

        [DataMember(Name = "os_version")]
        public string OperatingSystemVersion { get; set; }
    }
}
