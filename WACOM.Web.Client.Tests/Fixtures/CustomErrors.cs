using System;
using Azure.Automation;
using Azure.Automation.Fixtures;
using Azure.Automation.WindowsAzurePortal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace WACOM.Web.Client.Tests.Fixtures
{
    [TestClass]
    public class CustomErrors : BaseFixture
    {
        [TestMethod]
        [TestCategory(Categories.P1)]
        [Ignore]
        [Description("Expecting to see a custom 500 error page when we hit the test error path")]
        public void ShouldShowCustom500()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                // Navigate to error page
                CommonSeleniumSteps.NavigateToURL(driver, "/xx-yy/xx-yy/throws/");

                // Find canonical
                var canonical = driver.FindElement(By.CssSelector("link[rel='canonical']"));

                // Verify canonical exists
                Assert.IsNotNull(canonical);

                // Verify canonical is on this domain
                Assert.IsTrue(canonical.GetAttribute("href").Contains(Azure.Automation.Helpers.TestConfiguration.Instance.EnvironmentUrl));
            });
        }
    }
}
