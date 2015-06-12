namespace Azure.Automation.Fixtures
{
    using Azure.Automation.Helpers;
    using Azure.Automation.WindowsAzurePortal;
    using Microsoft.Selenium.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using System.Collections.ObjectModel;
    using System.Net;

    [TestClass]
    public class MarketPlace : BaseFixture
    {
        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Check all the items and fuctions work fine")]
        public void CheckEverythingInMarketPlace()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to marketplace page");
                CommonSeleniumSteps.NavigateToURL(driver, "/en-us/marketplace/");

                Logger.Instance.WriteLine("STEP 2: Try to find all the sections VMs, Web apps, Application services, etc");
                ReadOnlyCollection<IWebElement> allSections = driver.FindElements(By.CssSelector("section[id='gallery-featured'] ul"));

                Logger.Instance.WriteLine("STEP 3: Verify are there 5 sections");
                Assert.AreEqual(7, allSections.Count);

                Logger.Instance.WriteLine("STEP 4: Verify each section has 6 apps and each app has a valid link");
                foreach (IWebElement section in allSections)
                {
                    ReadOnlyCollection<IWebElement> apps = section.FindElements(By.CssSelector(".image"));
                    Assert.AreEqual(6, apps.Count);
                    foreach (IWebElement app in apps)
                    {
                        Assert.AreEqual(HttpStatusCode.OK.ToString(), CommonSeleniumSteps.GetHTTPStatusCode(app.GetAttribute("href")), "this link is invalid: " + app.GetAttribute("href"));
                    }
                }

                Logger.Instance.WriteLine("STEP 5: Get all the 'See all' links and verify them");
                ReadOnlyCollection<IWebElement> showAll = driver.FindElement(By.Id("gallery-featured")).FindElements(By.CssSelector(".show-all"));
                Assert.AreEqual(6, showAll.Count);
                foreach (IWebElement item in showAll)
                {
                    Assert.AreEqual(HttpStatusCode.OK.ToString(), CommonSeleniumSteps.GetHTTPStatusCode(item.GetAttribute("href")), "this 'See all' link is invalid: " + item.GetAttribute("href"));
                }

                Logger.Instance.WriteLine("STEP 6: Verify search box work on keypress of 'enter' key and pagination works");
                IWebElement searchBox = driver.FindElement(By.CssSelector("section input"));
                searchBox.SendKeys("dropbox for business");
                searchBox.SendKeys(Keys.Enter);

                ReadOnlyCollection<IWebElement> resultSet = driver.WaitUntil(() => driver.FindElement(By.Id("result-set")).FindElements(By.CssSelector("li")), "Get an empry result with keyword 'dropbox for business'");
                Assert.AreEqual(1, resultSet.Count);
                Assert.AreEqual("Dropbox for Business", resultSet[0].FindElement(By.CssSelector("a")).GetAttribute("title"));
            });
        }
    }
}

