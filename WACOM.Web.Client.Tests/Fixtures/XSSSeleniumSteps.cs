namespace WACOM.Web.Client.Tests.Fixtures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Azure.Automation.Helpers;
    using Azure.Automation.Selenium.Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;

    public class XSSSeleniumSteps
    {
        public static void NavigateToPageWithXSS(IWebDriver driver, string pageUrl)
        {
            Logger.Instance.WriteSeparatorLine();

            Logger.Instance.Log("INFO", string.Format("Testing XSS in {0}", pageUrl));

            Logger.Instance.LogAction("Navigate to page with XSS Attempt in URL", () =>
            {
                driver.Navigate().GoToUrl(string.Concat(pageUrl, @"#'><script>var xssInjected=true</script>'"));
                driver.WaitForPageToLoad();
            }, failOnEnd: true);

            Logger.Instance.LogAction("Check jQuery, Core and Wacom JS environment variables are properly setup", () =>
            {
                string[] environmentVars = new[] { "jQuery", "Core", "Wacom" };
                driver.Wait(3, message: "Acom JS variables not found - check the test page URLs").Until((d) =>
                {
                    var exstingVars = driver.CheckJsVariablesExist(environmentVars);
                    return !environmentVars.Except(exstingVars).ToList().Any();
                });
            }, failOnEnd: true);
        }

        public static void CheckXSSResults(IWebDriver driver)
        {
            Logger.Instance.LogAction("Check that XSS Attempt was unsuccessful", () =>
            {
                Assert.AreEqual(driver.CheckJsVariablesExist("xssInjected").Count(), 0, "The XSS Attempt was successful.");
            }, failOnEnd: true);
        }
    }
}
