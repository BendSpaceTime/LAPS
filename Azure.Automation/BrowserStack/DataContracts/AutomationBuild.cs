namespace Azure.Automation.BrowserStack.DataContracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class AutomationBuild
    {
        private const string BrowserStackBuildBaseUrl = "https://www.browserstack.com/automate/builds/";

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "hashed_id")]
        public string Id { get; set; }

        [IgnoreDataMember]
        public string Url { 
            get { return BrowserStackBuildBaseUrl + this.Id; } 
        }
    }
}
