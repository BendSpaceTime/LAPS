namespace Azure.Automation.Fixtures
{
    using Azure.Automation.Helpers;
    using Azure.Automation.WindowsAzurePortal;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using System.Collections.Generic;
    using System.Net;
    using System.Linq;

    [TestClass]
    public class PricingDetails : BaseFixture
    {
        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Verify a default region that has pricing for the new visitor")]
        public void DefaultRegionDisplayedForNewVisitor()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Clean up cookies");
                LocCurrencyHelpers.ResetCookiesForLocale(driver);
                Logger.Instance.WriteLine("STEP 2: Navigate to Virtual machines");
                CommonSeleniumSteps.NavigateToURL(driver, "/en-us/pricing/details/virtual-machines/");

                IWebElement region = driver.FindElement(By.CssSelector("select[id='wa-dropdown-region']+span"));
                Assert.AreEqual("Central US", region.Text, "Default region is not available on VM page");
                Assert.IsTrue(region.Displayed, "Central US is not visiable");

                LocCurrencyHelpers.ResetCookiesForLocale(driver);
                Logger.Instance.WriteLine("Navigate to Website");
                CommonSeleniumSteps.NavigateToURL(driver, "/en-us/pricing/details/websites/");
                region = driver.FindElement(By.CssSelector("select[id='wa-dropdown-region']+span"));
                Assert.AreEqual("Central US", region.Text, "Default region is not available on Website page");
                Assert.IsTrue(region.Displayed, "Central US is not visiable");
            });
        }

        [TestMethod]
        [TestCategory(Categories.P0)]
        [Description("Check the HTTP status code for all the pricing details pages in English ")]
        public void CheckAllPricingDetailsPageInEnglish()
        {
            Logger.Instance.WriteLine("STEP 1: Get all pricing details pages");
            List<string> allUrls = new List<string>();
            allUrls.AddRange(PricingDetailsHelper.GetAllURLsFromPricingDetail().Values);
            Logger.Instance.WriteLine("STEP 2: Verify each page return a good status");
            foreach (string url in allUrls)
            {
                Logger.Instance.WriteLine("Checking: "+url);
                Assert.AreEqual(HttpStatusCode.OK.ToString(), CommonSeleniumSteps.GetHTTPStatusCode(url),"This Url status code is not OK: "+url);
            }
        }

        [TestMethod]
        [TestCategory(Categories.P0)]
        [Description("Check the HTTP status code for all the pricing details pages in all languages ")]
        public void CheckAllPricingDetailsPageInAllLanguages()
        {
            Logger.Instance.WriteLine("STEP 1: Get all pricing details pages");
            List<string> allUrls = new List<string>();
            allUrls.AddRange(PricingDetailsHelper.GetAllURLsFromPricingDetail().Values);
            Logger.Instance.WriteLine("STEP 2: Get all languages");
            List<string> allLanguages = LocCurrencyHelpers.GetAllLangCultures();
            Logger.Instance.WriteLine("STEP 3: Verify each page return a good status");
            foreach (string url in allUrls)
            {
                foreach (string culture in allLanguages)
                {
                    Logger.Instance.WriteLine("Checking: " + url.Replace("en-us", culture));
                   Assert.AreEqual(HttpStatusCode.OK.ToString(), CommonSeleniumSteps.GetHTTPStatusCode(url.Replace("en-us", culture)), "This Url status code is not OK: " + url.Replace("en-us", culture));
                }
            }
        }

        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Always have the region dropdown snap to a valid region on doc ready")]
        public void ShouldAlwaysLoadWithValidRegion()
        {
            // no region dropdowns here
            var skipMe = new[]
            {
                "/pricing/details/storsimple/",
                "/pricing/",
                "/pricing/details/services/app-service/",
                "/pricing/details/websites/",
                "/"
            };

            this.CreateSeleniumTestContext().Run(driver =>
            {
                foreach (var url in PricingDetailsHelper.GetAllRelativeURLsFromPricingDetail().Values.Except(skipMe))
                {
                    // Region settings are saved in local storage
                    // web driver data; urls have no local storage so its always first run scenario

                    // Navigate to Pricing details page
                    CommonSeleniumSteps.NavigateToURL(driver, "/en-us" + url);

                    // Find selected option in region dropdown
                    var region = driver.FindElement(By.CssSelector("#wa-dropdown-region option:checked"));

                    // Verify region is not 'fake-disabled'
                    Assert.IsFalse(region.GetAttribute("class").Contains("fake-disabled"));
                }
            });
        }
    }
}

