namespace Azure.Automation.BrowserStack.DataContracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class SessionReference
    {
        [DataMember(Name = "automation_session")]
        public AutomationSession AutomationSession { get; set; }
    }
}
