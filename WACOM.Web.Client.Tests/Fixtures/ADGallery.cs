namespace Azure.Automation.Fixtures
{
    using Azure.Automation.WindowsAzurePortal;
    using Azure.Automation.Helpers;
    using Microsoft.Selenium.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using System.Net;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    [TestClass]
    public class ADGallery : BaseFixture
    {
        [TestMethod]
        [TestCategory(Categories.P2)]
        [Description("No missing images on AD Gallery Featured Tab")]
        public void ADGalleryFeaturedImages()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to Active directory featured tab");
                CommonSeleniumSteps.NavigateToURL(driver, "/en-us/marketplace/active-directory/#featured");

                Logger.Instance.WriteLine("STEP 2: Verify image item appears");
                ReadOnlyCollection<IWebElement> allImage = driver.FindElement(By.ClassName("wa-galleryItemContainer")).FindElements(By.CssSelector("img"));
                Assert.IsTrue(allImage.Count > 0, "There is no image on Active dirctory featured tab");

                Logger.Instance.WriteLine("STEP 3: Verify each image url starts with http or https and with a status of OK");
                string failInfo;
                Assert.IsTrue(CommonSeleniumSteps.VerifyUrlsAreAvailableAndNotRelativePath(allImage, out failInfo), failInfo);
            });
        }

        [TestMethod]
        [TestCategory(Categories.P2)]
        [Description("No missing images on AD Gallery All Tab")]
        public void ADGalleryAllImages()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to Active directory all tab");
                CommonSeleniumSteps.NavigateToURL(driver, "/en-us/marketplace/active-directory/all/");

                Logger.Instance.WriteLine("STEP 2: Verify image item appears");
                List<IWebElement> allImage = driver.FindElement(By.Id("gallery-items")).FindElements(By.CssSelector("li")).ToList();
                List<IWebElement> allVisibleImageItemsOnAlltab = allImage.ConvertAll<IWebElement>(item => item.FindElement(By.CssSelector("a[class='image']"))) ;
                Assert.IsTrue(allVisibleImageItemsOnAlltab.Count > 0, "There is no image on Active dirctory all tab");

                Logger.Instance.WriteLine("STEP 3: Verify each image url starts with http or https and with a status of OK");
                string failInfo;
                Assert.IsTrue(CommonSeleniumSteps.VerifyUrlsAreAvailableAndNotRelativePath(allVisibleImageItemsOnAlltab, out failInfo,"href"), failInfo);
            });
        }
    }
}

