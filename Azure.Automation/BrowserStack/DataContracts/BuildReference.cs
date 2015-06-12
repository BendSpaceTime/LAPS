namespace Azure.Automation.BrowserStack.DataContracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class BuildReference
    {
        [DataMember(Name = "automation_build")]
        public AutomationBuild AutomationBuild { get; set; }
    }
}
