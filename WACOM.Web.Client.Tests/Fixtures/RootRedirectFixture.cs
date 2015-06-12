using System;
using System.Net;
using System.Net.Http;
using Azure.Automation;
using Azure.Automation.WindowsAzurePortal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestConfiguration = Azure.Automation.Helpers.TestConfiguration;

namespace WACOM.Web.Client.Tests.Fixtures
{
    [TestClass]
    public class RootRedirectFixture
    {
        /// <summary>
        /// Navigate to http://azure.microsoft.com with different accept language headers
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.P1)]
        public void ShouldRespectLanguageHeader()
        {
            // Gets a Dictionary <culture for language header, expectedRedirect>
            var cultures = LocCurrencyHelpers.GetAllLangCulturesAndFallbacks();

            foreach (var culture in cultures)
            {
                var httpClientHandler = new HttpClientHandler {AllowAutoRedirect = false};

                using (var client = new HttpClient(httpClientHandler))
                {
                    client.BaseAddress = new Uri(TestConfiguration.Instance.EnvironmentUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                    client.DefaultRequestHeaders.Add("Accept-Language", culture.Key);
                    using (var response = client.GetAsync("/").Result)
                    {
                        Assert.AreEqual(HttpStatusCode.Found, response.StatusCode);
                        Assert.AreEqual("/" + culture.Value, response.Headers.Location.OriginalString);
                    }
                }
            }
        }
    }
}
