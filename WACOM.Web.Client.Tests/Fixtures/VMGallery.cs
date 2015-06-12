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
    public class VMGallery : BaseFixture
    {
        [TestMethod]
        [TestCategory(Categories.P0)]
        [Description("Load Featured Tab & verify they are clickable?")]
        public void VerifyFeaturedVMs()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                //Step 1: Load Add-ons Page, featured tab
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/marketplace/virtual-machines/");

                //Step 2:  Verify four add-ons appear
                IWebElement galleryResults = driver.WaitUntil(() => driver.FindElement(By.ClassName("wa-galleryItemContainer")), "Unable to find Gallery item list", System.TimeSpan.FromSeconds(30));
                int count = galleryResults.FindElements(By.ClassName("wa-galleryItem")).Count;
                Assert.AreEqual(12, count);
            });
        }

        [TestMethod]
        [TestCategory(Categories.P2)]
        [Description("No missing images on VM Gallery Featured Tab")]
        public void VMGalleryFeaturedImages()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to VM Gallery page");
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/marketplace/virtual-machines");

                Logger.Instance.WriteLine("STEP 2: Try to find all image elements on VM gallery tab");
                ReadOnlyCollection<IWebElement> featuredVMImages = driver.WaitUntil(() => driver.FindElement(By.ClassName("wa-galleryItemContainer")).FindElements(By.CssSelector("img[alt='']")), "Unable to find Gallery item list", System.TimeSpan.FromSeconds(30));
                Assert.AreNotEqual(0, featuredVMImages.Count,"No gallery item exists on VM gallery featured tab");

                Logger.Instance.WriteLine("STEP 3: Verify each image url starts with http or https and with a status of OK");
                string failInfo;
                Assert.IsTrue(CommonSeleniumSteps.VerifyUrlsAreAvailableAndNotRelativePath(featuredVMImages, out failInfo), failInfo);
            });
        }

        [TestMethod]
        [TestCategory(Categories.P2)]
        [Description("No missing images on VM Gallery By All Tab")]
        public void VMGalleryAllImages()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to Partners page");
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/marketplace/virtual-machines/#all");

                Logger.Instance.WriteLine("STEP 2: Try to find all image elements on All tab");
                ReadOnlyCollection<IWebElement> featuredVMImages = driver.WaitUntil(() => driver.FindElement(By.Id("result-set")).FindElements(By.CssSelector("img[alt='']")), "Unable to find Gallery item list", System.TimeSpan.FromSeconds(30));
                Assert.IsTrue(featuredVMImages.Count > 0, "No gallery item exists on All tab");

                Logger.Instance.WriteLine("STEP 3: Add the all displayed gallery items on the first page of All tab into list");
                List<IWebElement> allVisibleImageItemsOnHomePage = featuredVMImages.Where(visibleItem => visibleItem.Displayed == true).ToList();
                Assert.IsTrue(allVisibleImageItemsOnHomePage.Count > 0, "There is not any visible gallery items on first page of All tab");

                Logger.Instance.WriteLine("STEP 4: Verify each image url starts with http or https and with a status of OK");
                string failInfo;
                Assert.IsTrue(CommonSeleniumSteps.VerifyUrlsAreAvailableAndNotRelativePath(allVisibleImageItemsOnHomePage, out failInfo), failInfo);
            });
        }
    }
}
