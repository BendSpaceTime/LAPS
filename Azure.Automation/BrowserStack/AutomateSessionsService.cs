namespace Azure.Automation.BrowserStack
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using Azure.Automation.BrowserStack.DataContracts;
    using Azure.Automation.Helpers;

    public class AutomateSessionsService
    {
        private const string ListBuildsEndpoint = "https://www.browserstack.com/automate/builds.json";
        private const string ListBuildSessionsEndpointTemplate = "https://www.browserstack.com/automate/builds/{0}/sessions.json";
        private const string SessionsEndpointTemplate = "https://www.browserstack.com/automate/sessions/{0}.json";

        private WebClient webClient;

        public AutomateSessionsService()
        {
            this.webClient = new WebClient();
            this.webClient.Credentials = new NetworkCredential(TestConfiguration.Instance.BrowserStackUser, TestConfiguration.Instance.BrowserStackKey);
        }

        public string GetSessionUrl(string buildName, string sessionName)
        {
            var build = this.GetBuild(buildName);

            var session = this.GetSession(build.Id, sessionName);
            return this.TrimSessionUrl(session.LogsUrl);
        }

        public AutomationBuild GetBuild(string buildName)
        {
            buildName = this.EscapeName(buildName);
            var builds = this.RequestJsonAsListOf<BuildReference>(ListBuildsEndpoint);

            var reference = builds.SingleOrDefault(b => b.AutomationBuild.Name == buildName);
            if (reference == null)
            {
                throw new Exception("Build referece not found");
            }

            return reference.AutomationBuild;
        }

        public AutomationSession GetSession(string buildId, string sessionName)
        {
            sessionName = this.EscapeName(sessionName);

            var url = string.Format(ListBuildSessionsEndpointTemplate, buildId);
            var sessions = this.RequestJsonAsListOf<SessionReference>(url);

            SessionReference reference = sessions.First(s => s.AutomationSession.Name == sessionName);                    

            if (reference == null)
            {
                throw new Exception("Session reference not found");
            }

            return reference.AutomationSession;
        }

        public string GetSessionUrl(string sessionId)
        {
            var session = this.GetSession(sessionId);
            return this.TrimSessionUrl(session.LogsUrl);
        }

        private AutomationSession GetSession(string sessionId)
        {
            var url = string.Format(SessionsEndpointTemplate, sessionId);
            var reference = this.RequestJsonAs<SessionReference>(url);
            if (reference == null)
            {
                throw new Exception("Session reference not found");
            }

            return reference.AutomationSession;
        }

        private string EscapeName(string buildName)
        {
            return buildName.Replace("-", " ");
        }

        private T RequestJsonAs<T>(string url)
        {
            var json = this.webClient.DownloadString(url);

            var serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            return (T)serializer.ReadObject(ms);
        }

        private T[] RequestJsonAsListOf<T>(string url)
        {
            var json = this.webClient.DownloadString(url);

            var serializer = new DataContractJsonSerializer(typeof(T[]));
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            return (T[])serializer.ReadObject(ms);
        }

        private string TrimSessionUrl(string sessionLogsUrl)
        {
            return sessionLogsUrl.Replace("/logs", string.Empty);
        }
    }
}
