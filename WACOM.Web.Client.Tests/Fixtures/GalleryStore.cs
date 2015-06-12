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
    using System.Linq;

    [TestClass]
    public class GalleryStore : BaseFixture
    {
        [TestMethod]
        [TestCategory(Categories.P0)]
        [Description("Load Add-ons page and verify four add-on items appear for Featured Tab")]
        public void VerifyFeaturedItemsCountForAddonPage()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                //Step 1: Load Add-ons Page, featured tab
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/marketplace/application-services/#featured");

                //Step 2:  Verify four add-ons appear
                IWebElement galleryResults = driver.WaitUntil(() => driver.FindElement(By.ClassName("wa-galleryItemContainer")), "Unable to find Gallery item list", System.TimeSpan.FromSeconds(30));
                int count = galleryResults.FindElements(By.ClassName("wa-galleryItem")).Count;
                Assert.AreEqual(12, count);
            });
        }

        [TestMethod]
        [TestCategory(Categories.P2)]
        [Description("No missing images on Store Gallery All Tab")]
        public void StoreGalleryAllImages()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to Store Gallery All Tab");
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/marketplace/application-services/all/");

                Logger.Instance.WriteLine("STEP 2: Verify add-ons appears");
                ReadOnlyCollection<IWebElement> allAddons = driver.WaitUntil(() => driver.FindElement(By.Id("gallery-items")).FindElements(By.CssSelector("li a[class='image']")), "Unable to find Gallery item list", System.TimeSpan.FromSeconds(30));
                Assert.AreNotEqual(0, allAddons.Count, "No gallery item exists on Store Gallery All tab");


                Logger.Instance.WriteLine("STEP 3: Add the all displayed gallery items on the first page of Store Gallery All tab into list");
                List<IWebElement> allVisibleImageItemsOnAlltab = allAddons.Where(visibleItem => visibleItem.Displayed == true).ToList();
                Assert.IsTrue(allVisibleImageItemsOnAlltab.Count > 0, "There is not any visible gallery items on first page of Store Gallery All tab");

                Logger.Instance.WriteLine("STEP 4: Verify each image url starts with http or https and with a status of OK");
                string failInfo;
                Assert.IsTrue(CommonSeleniumSteps.VerifyUrlsAreAvailableAndNotRelativePath(allVisibleImageItemsOnAlltab, out failInfo,"href"), failInfo);
            });
        }


        [TestMethod]
        [TestCategory(Categories.P2)]
        [Description("No missing images on Store Gallery Free Tab")]
        public void StoreGalleryFreeImages()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to Store Gallery Free Tab");
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/marketplace/application-services/#free");

                Logger.Instance.WriteLine("STEP 2: Verify add-ons appears");
                ReadOnlyCollection<IWebElement> allAddons = driver.WaitUntil(() => driver.FindElement(By.ClassName("wa-galleryItemContainer")).FindElements(By.CssSelector("img[alt='']")), "Unable to find Gallery item list", System.TimeSpan.FromSeconds(30));
                Assert.AreNotEqual(0, allAddons.Count, "No gallery item exists on Store Gallery Free Tab");


                Logger.Instance.WriteLine("STEP 3: Add the all displayed gallery items on the first page of Store Gallery Free Tab into list");
                List<IWebElement> allVisibleImageItemsOnAlltab = allAddons.Where(visibleItem => visibleItem.Displayed == true).ToList();
                Assert.IsTrue(allVisibleImageItemsOnAlltab.Count > 0, "There is not any visible gallery items on first page of Store Gallery Free Tab");

                Logger.Instance.WriteLine("STEP 4: Verify each image url starts with http or https and with a status of OK");
                string failInfo;
                Assert.IsTrue(CommonSeleniumSteps.VerifyUrlsAreAvailableAndNotRelativePath(allVisibleImageItemsOnAlltab, out failInfo), failInfo);
            });
        }


        [TestMethod]
        [TestCategory(Categories.P2)]
        [Description("No missing images on Store Gallery Featured Tab")]
        public void StoreGalleryFeaturedImages()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to Store Gallery Featured Tab");
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/marketplace/application-services/#featured");

                Logger.Instance.WriteLine("STEP 2: Verify add-ons appears");
                ReadOnlyCollection<IWebElement> allAddons = driver.WaitUntil(() => driver.FindElement(By.ClassName("wa-galleryItemContainer")).FindElements(By.CssSelector("a img[alt='']")), "Unable to find Gallery item list", System.TimeSpan.FromSeconds(30));
                Assert.AreNotEqual(0, allAddons.Count, "No gallery item exists on Store Gallery Featured Tab");

                Logger.Instance.WriteLine("STEP 3: Verify each image url starts with http or https and with a status of OK");
                string failInfo;
                Assert.IsTrue(CommonSeleniumSteps.VerifyUrlsAreAvailableAndNotRelativePath(allAddons, out failInfo), failInfo);
            });
        }


    }

}
