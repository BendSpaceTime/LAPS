namespace Azure.Automation.Fixtures
{
    using Azure.Automation.WindowsAzurePortal;
    using Azure.Automation.Helpers;
    using Azure.Automation.Selenium.Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Selenium.Utilities;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net;

    [TestClass]
    public class PricingCalculator : BaseFixture
    {
        public List<string> GetPricingPagesNames(IWebDriver driver)
        {
            List<string> names = new List<string>();
            IWebElement pricingMenu = driver.FindElement(By.ClassName("pricing-calc-nav"));
            var pricingPageElements = pricingMenu.FindElements(By.TagName("li"));
            foreach (IWebElement page in pricingPageElements)
            {
                //Split string to remove the 'active' class of currently selected calculator
                names.Add(page.GetAttribute("class").Split(' ')[0]);
            }
            return names;
        }

        [TestMethod]
        [TestCategory(Categories.P0)]
        [Description("Load each Pricing Calculator Service via Nav bar")]
        public void NavigateToPricingCalculatorForEachService()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                // STEP 1: Navigate to Pricing Calculator page
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/pricing/calculator/");

                // STEP 2: Get each pricing calculator nav item from menu and navigate to it
                var pricingPages = GetPricingPagesNames(driver);
                foreach (string calcName in pricingPages)
                {
                    IWebElement calcPage = driver.FindElement(By.ClassName(calcName)).FindElement(By.TagName("a"));
                    driver.WaitForURLChange(() => calcPage.Click());

                    // STEP 3: Verify a calc slider appears
                    driver.WaitUntil(() => driver.FindElement(By.ClassName("slider-div")), "No price slider appears on page");
                }

            });
        }
    }

}
