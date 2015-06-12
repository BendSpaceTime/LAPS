namespace WACOM.Web.Client.Tests.Fixtures
{
    using Azure.Automation;
    using Azure.Automation.Fixtures;
    using Azure.Automation.WindowsAzurePortal;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestConfiguration = Azure.Automation.Helpers.TestConfiguration;

    [TestClass]
    public class XSSFixture : BaseFixture
    {
        private string[] pagesToTest = { 
                "/", 
                "/pricing/free-trial/", 
                "/en-us/solutions/big-compute/", 
                "/en-us/solutions/infrastructure/", 
                "/en-us/solutions/web/", 
                "/en-us/solutions/media/", 
                "/en-us/solutions/dev-test/", 
                "/en-us/solutions/data-management/", 
                "/en-us/solutions/identity/", 
                "/en-us/solutions/",
                "/en-us/documentation/",
                "/en-us/downloads/" };

        [TestMethod]
        [Ignore]  //Need to update unreliable test
        [TestCategory(Categories.XSS)]
        [Description("Verify that some key pages are not vulnerable to cross-site scripting attacks in the URL. Initializes a JS variable in the URL and then makes sure it's not present.")]
        public void TestXSS()
        {
            this.CreateSeleniumTestContext().Run(driver => {

                foreach (var path in pagesToTest)
                {
                    var fullUrl = string.Concat(TestConfiguration.Instance.EnvironmentUrl, path);

                    // STEP 1: Navigate to page with XSS Attempt in URL
                    XSSSeleniumSteps.NavigateToPageWithXSS(driver, fullUrl);
                   
                    // STEP 3: Check that XSS Attempt was unsuccessful
                    XSSSeleniumSteps.CheckXSSResults(driver);
                }
            });
        }
    }
}
