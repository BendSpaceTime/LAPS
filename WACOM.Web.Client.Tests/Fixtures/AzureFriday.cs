namespace Azure.Automation.Fixtures
{
    using Azure.Automation.WindowsAzurePortal;
    using Azure.Automation.Helpers;
    using Microsoft.Selenium.Utilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using System.Net;

    [TestClass]
    public class VideoAzureFriday : BaseFixture
    {
        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Verify Azure Friday video is available")]
        public void AzureFridayVideoAvailability()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to AzureFriday");
                string azureFriURL = Azure.Automation.Helpers.TestConfiguration.Instance.EnvironmentUrl + "/en-us/documentation/videos/azure-friday/";
                CommonSeleniumSteps.NavigateToURL(driver, "/en-us/documentation/videos/azure-friday/");

                Logger.Instance.WriteLine("STEP 2: Verify Video image thumbnail");
                IWebElement videoImageLink = driver.FindElement(By.ClassName("wa-video-thumbnail-mega"));
                string imageUrl = videoImageLink.FindElement(By.TagName("img")).GetAttribute("src");
                //verify image is good
                Assert.AreEqual(HttpStatusCode.OK.ToString(), CommonSeleniumSteps.GetHTTPStatusCode(azureFriURL), "HTTP for thumbnail code not 200");
                videoImageLink.Click();

                Logger.Instance.WriteLine("STEP 3: Verify clicking on image brings up iframe");
                IWebElement videoIframe = driver.WaitUntil(() => driver.FindElement(By.CssSelector("div.wa-videoWrapper > iframe")), "Video iframe not loaded", System.TimeSpan.FromSeconds(30));

                Logger.Instance.WriteLine("STEP 4: Verify Video image link is available with HTTP 200"); 
                string videoURL = videoIframe.GetAttribute("src");

                Assert.AreEqual(HttpStatusCode.OK.ToString(), CommonSeleniumSteps.GetHTTPStatusCode(videoURL), "HTTP code for video not 200");
            });
        }
    }
}
