namespace Azure.Automation.Fixtures
{
    using Azure.Automation.Helpers;
    using Azure.Automation.Selenium.Extensions;
    using Azure.Automation.WindowsAzurePortal;
    using Microsoft.Selenium.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using System.Collections.ObjectModel;
    using System.Net;

    [TestClass]
    public class AzureStatus:BaseFixture
    {
        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Verify user can switch to History tab and switch back to Current status")]
        public void SwitchBetweenHistoryAndCurrentStatus()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to Status page");
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/status/");

                Logger.Instance.WriteLine("STEP 2: Click History tab and verify URL changed");
                IWebElement historyTab = driver.FindElement(By.CssSelector("a[data-slug='history']"));
                driver.WaitForURLChange(() => historyTab.Click());
                Assert.IsTrue(driver.Url.Contains("history"), "After click History tab URL did not change");
                historyTab.Click();

                Logger.Instance.WriteLine("STEP 3: Verify content changed after switch to History tab");
                Assert.IsTrue(driver.FindElement(By.CssSelector("div[id='wa-historyResults-container']")).Displayed,"History result tab is not visable");
                Assert.IsTrue(driver.FindElement(By.CssSelector("label[for='wa-dropdown-service']")).Displayed, "Service label is not visable in History tab");
                Assert.IsFalse(driver.FindElement(By.XPath("//td[contains(.,'App Service')]")).Displayed, "App Service is visable in History tab");

                Logger.Instance.WriteLine("STEP 4: Click Current Status tab and Verify URL changed");
                IWebElement currentTab = driver.FindElement(By.CssSelector("a[data-slug='current']"));
                driver.WaitForURLChange(() => currentTab.Click());
                Assert.IsTrue(driver.Url.Contains("current"), "After switch back to Current Status, URL did not change");
                currentTab.Click();

                Logger.Instance.WriteLine("STEP 5: Vrtify content changed after switch to Current tab");
                Assert.IsTrue(driver.FindElement(By.CssSelector("label[for='wa-dropdown-refresh']")).Displayed, "Refresh label is not visable in Current Status page");
                Assert.IsTrue(driver.FindElement(By.XPath("//td[contains(.,'App Service')]")).Displayed, "App Service is not visable in Current Status tab");
                Assert.IsFalse(driver.FindElement(By.CssSelector("div[id='wa-historyResults-container']")).Displayed,"History result tab is visable in Current Status tab");
            });
        }

        [TestMethod]
        [TestCategory(Categories.P2)]
        [Description("Verify user can close some regions in status table and could bring them back again")]
        [Ignore]
        public void CanToggleStatusRegions()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                ReadOnlyCollection<IWebElement> parsedAllRegions;
                IWebElement showAllRegionsLink;
                int totalVisableRegios = 0;

                Logger.Instance.WriteLine("STEP 1: Navigate to Status page");
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/status/");

                Logger.Instance.WriteLine("STEP 2: Verify user can turn off some regions");
                parsedAllRegions = driver.FindElements(By.CssSelector("div[data-region]"));

                parsedAllRegions[0].FindElement(By.ClassName("close")).Click();
                Assert.IsTrue(driver.WaitUntil(() => { return !parsedAllRegions[0].Displayed; }, "Element " + parsedAllRegions[0].GetAttribute("data-region") + " is not invisable"), parsedAllRegions[0].GetAttribute("data-region") + " still visable after close it.");
                parsedAllRegions[2].FindElement(By.ClassName("close")).Click();
                Assert.IsTrue(driver.WaitUntil(() => { return !parsedAllRegions[2].Displayed; }, "Element " + parsedAllRegions[2].GetAttribute("data-region") + " is not invisable"), parsedAllRegions[2].GetAttribute("data-region") + " still visable after close it.");
                parsedAllRegions[5].FindElement(By.ClassName("close")).Click();
                Assert.IsTrue(driver.WaitUntil(() => { return !parsedAllRegions[5].Displayed; }, "Element " + parsedAllRegions[5].GetAttribute("data-region") + " is not invisable"), parsedAllRegions[5].GetAttribute("data-region") + " still visable after close it.");

                Logger.Instance.WriteLine("STEP 3: Verify the closed regions could bring back to user");
                showAllRegionsLink = driver.FindElement(By.ClassName("show-all"));
                showAllRegionsLink.Click();
                Assert.IsTrue(driver.WaitUntil(() => { return parsedAllRegions[0].Displayed; }, "Element " + parsedAllRegions[0].GetAttribute("data-region") + " is not visable"), parsedAllRegions[0].GetAttribute("data-region") + " still invisable after bring it back.");
                Assert.IsTrue(driver.WaitUntil(() => { return parsedAllRegions[2].Displayed; }, "Element " + parsedAllRegions[2].GetAttribute("data-region") + " is not visable"), parsedAllRegions[2].GetAttribute("data-region") + " still invisable after bring it back.");
                Assert.IsTrue(driver.WaitUntil(() => { return parsedAllRegions[5].Displayed; }, "Element " + parsedAllRegions[5].GetAttribute("data-region") + " is not visable"), parsedAllRegions[5].GetAttribute("data-region") + " still invisable after bring it back.");
                foreach (IWebElement region in parsedAllRegions)
                {
                    if (region.Displayed) ++totalVisableRegios;
                }
                Assert.IsTrue(totalVisableRegios == parsedAllRegions.Count, "Fail to bring back all the closed regions after click ShowAllRegions");


                Logger.Instance.WriteLine("STEP 4: Verify user can close all the regions");
                foreach (IWebElement region in parsedAllRegions)
                {
                    region.FindElement(By.ClassName("close")).Click();
                    Assert.IsTrue(driver.WaitUntil(() => { return !region.Displayed; }, "Element " + region.GetAttribute("data-region") + " is not invisable"), region.GetAttribute("data-region") + " still visable after close it.");
                }

                Logger.Instance.WriteLine("STEP 5: Verify user could bring back all the closed regions");
                showAllRegionsLink.Click();
                foreach (IWebElement region in parsedAllRegions)
                {
                    Assert.IsTrue(driver.WaitUntil(() => { return region.Displayed; }, "Element " + region.GetAttribute("data-region") + " is not visable"), region.GetAttribute("data-region") + " still invisable after bring it back.");
                    --totalVisableRegios;
                }
                Assert.IsTrue(totalVisableRegios == 0, "Fail to bring back all the closed regions after click ShowAllRegions");

            });
        }

	    [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Verify the top-level Status RSS feed is working")]
        public void VerifyStatusRSSFeed()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Load Azure status page");
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/status/");

                Logger.Instance.WriteLine("STEP 2: Check for Subscribe Link");
                IWebElement subscribeLink = driver.FindElement(By.CssSelector("div h1 a"));

                string expectedRSSUrl = Azure.Automation.Helpers.TestConfiguration.Instance.EnvironmentUrl + "/en-us/status/feed/";
                Assert.AreEqual(expectedRSSUrl, subscribeLink.GetAttribute("href"), "Actual Rss feed URL is not equal to expected URL");
                Assert.AreEqual("wa-icon wa-icon-misc-rss", subscribeLink.FindElement(By.CssSelector("span")).GetAttribute("class"));

                Logger.Instance.WriteLine("STEP 3: Verify URL is available");
                Assert.AreEqual(HttpStatusCode.OK.ToString(), CommonSeleniumSteps.GetHTTPStatusCode(expectedRSSUrl),"Expected URL is not working");
            });
        }

        [TestMethod]
        [TestCategory(Categories.P2)]
        [Description("Verify the Status RSS feed for each service listed in the table is working")]
        public void VerifyServicesStatusRSSFeed()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Load Azure status page");
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/status/");

                Logger.Instance.WriteLine("STEP 2: Get all RSS feed URL");
                ReadOnlyCollection<IWebElement> allRSSUrl = driver.FindElements(By.CssSelector("a[class='rss']"));
                Assert.AreNotEqual(allRSSUrl.Count, 0, "Could not find any RSS feed in Azure status page");

                Logger.Instance.WriteLine("STEP 3: Verify RSS is available");
                foreach(IWebElement rssFeed in allRSSUrl)
                {
                    Assert.AreEqual(HttpStatusCode.OK.ToString(), CommonSeleniumSteps.GetHTTPStatusCode(rssFeed.GetAttribute("href")), "Expected feed URL is not working: " + rssFeed.GetAttribute("href"));
                }
            });
        }
    }
}
