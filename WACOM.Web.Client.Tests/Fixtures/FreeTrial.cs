namespace Azure.Automation.Fixtures
{
    using Azure.Automation.WindowsAzurePortal;
    using Azure.Automation.Helpers;
    using Azure.Automation.Selenium.Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Selenium.Utilities;
    using OpenQA.Selenium;
    using System.Net;

    [TestClass]
    public class FreeTrial : BaseFixture
    {
        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Navigate to Free Trial from Header Button")]
        public void NavigateToFreeTrialFromHeader()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                // STEP 1: Navigate to Homepage
                CommonSeleniumSteps.NavigateToHomepage(driver);

                // STEP 2: Find top right Free Trial button and click
                IWebElement trialButton = driver.FindElement(By.ClassName("wa-button-freeTrial"));
                driver.WaitForURLChange(() => trialButton.Click());

                //STEP 3: Verify page title
                Assert.IsTrue(driver.Title.Contains("FREE Trial"));
            });
        }

        [TestMethod]
        [TestCategory(Categories.P0)]
        [Description("Verify Free Trial main UI elements are present (Slideshow, Try now button, etc)")]
        public void VerifyFreeTrialPageUIElements()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to Free Trial");
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/pricing/free-trial/");

                Logger.Instance.WriteLine("STEP 2: Find slideshow control");
                driver.WaitUntil(() => driver.FindElement(By.ClassName("slideshow-control")), "Slideshow not found on page");

                Logger.Instance.WriteLine("STEP 3: Find Try it now button");
                IWebElement button = driver.WaitUntil(() => driver.FindElement(By.CssSelector(".wa-button-primary")), "Try now button not found on page");
                Assert.AreEqual("Try it now", button.Text);

                Logger.Instance.WriteLine("STEP 4: Find Buy now link");
                driver.WaitForURLChange(() => driver.FindElement(By.LinkText("Or buy now")).Click());

            });
        }
        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Verify user can do free trial sign up with Try It Now button")]
        public void VerifySignUpWithTryNow()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to Free Trial");
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/pricing/free-trial/");

                Logger.Instance.WriteLine("STEP 2: Find Try it now button");
                IWebElement button = driver.WaitUntil(() => driver.FindElement(By.CssSelector(".wa-button-primary")), "Try now button not found on page");
                Assert.AreEqual("Try it now", button.Text);

                Logger.Instance.WriteLine("STEP3: Verify link target is valid");
                Assert.AreEqual(@"https://account.windowsazure.com/signup?offer=ms-azr-0044p", button.GetAttribute("href"));
                Assert.AreEqual(HttpStatusCode.OK.ToString(), CommonSeleniumSteps.GetHTTPStatusCode("https://account.windowsazure.com/signup?offer=ms-azr-0044p"));
            });
        }
        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Verify can purchase with Buy Now link on Free Trial page")]
        public void VerifyPurchaseWithBuyNow()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to Free Trial");
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/pricing/free-trial/");

                Logger.Instance.WriteLine("STEP 2: Find and click Buy now link");
                driver.WaitForURLChange(() => driver.FindElement(By.LinkText("Or buy now")).Click());

                Logger.Instance.WriteLine("STEP3: Verify link target is valid");
                Assert.IsTrue(driver.Url.ToString().Contains("en-us/pricing/purchase-options/"), "URL is not correct for Buy now link.");
            });
        }

        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Navigate to Free Trial from Home Page Banner")]
        public void NavigateToFreeTrialFromHomePageBanner()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                // STEP 1: Navigate to Homepage
                CommonSeleniumSteps.NavigateToHomepage(driver);

                // STEP 2: Find Try For Free link in banner
                IWebElement trialLink = driver.FindElement(By.LinkText("Try for free"));
                driver.WaitForURLChange(() => trialLink.Click());

                //STEP 3: Verify page title
                Assert.IsTrue(driver.Title.Contains("FREE Trial"));
            });
        }

        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Verify Free Trial Credit Price is $200")]
        public void VerifyFreeTrialCreditPrice()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                // STEP 1: Navigate to Free Trial Page
                LocCurrencyHelpers.ResetCookiesForLocale(driver);
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/pricing/free-trial/");

                // STEP 2: Find Price for Free Trial Credit
                IWebElement priceSpan = driver.FindElement(By.CssSelector("section.wa-sectionFreeTrialB span[class='stored-price no-convert price-data'][data-pricekey='monetaryCredit.freeTrial']"));

                //STEP 3: Verify price is $200
                Assert.AreEqual("$200", priceSpan.Text);
            });
        }

        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Verify Free Trial Credit Price is $200 in Pricing Details")]
        public void VerifyCreditPriceInPricingDetails()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                // STEP 1: Navigate to Homepage
                LocCurrencyHelpers.ResetCookiesForLocale(driver);
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/pricing/details/virtual-machines/");

                // STEP 2: Find Price for Free Trial Credit
                IWebElement priceSpan = driver.FindElement(By.CssSelector("span[data-pricekey='freeTrial']"));

                //STEP 3: Verify price is $200
                Assert.AreEqual("$200", priceSpan.Text);
            });
        }

        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Verify Free Trial Credit Price updates for currency change")]
        public void VerifyChangeCreditPriceCurrency()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("Navigate to Homepage");
                LocCurrencyHelpers.ResetCookiesForLocale(driver);
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/pricing/free-trial/");

                Logger.Instance.WriteLine("Switch to British Pounds");
                LocCurrencyHelpers.ChangeCurrencyByName("British Pound (£)", driver);
                driver.WaitForPageToLoad();

                // Find Price for Free Trial Credit
                IWebElement priceSpan = driver.FindElement(By.CssSelector("div[class='wa-content wa-content-3up wa-content-memberOffers'] span[data-pricekey='monetaryCredit.freeTrial']"));

                Logger.Instance.WriteLine("Verify price chaged to Pounds and is £125");
                Assert.AreEqual("£125", priceSpan.Text);

                LocCurrencyHelpers.ChangeCurrencyByName("US Dollar ($)", driver);
                driver.WaitForPageToLoad();
                // Find Price for Free Trial Credit again
                priceSpan = driver.FindElement(By.CssSelector("div[class='wa-content wa-content-3up wa-content-memberOffers'] span[data-pricekey='monetaryCredit.freeTrial']"));

                //STEP 3: Verify price is $200 USD
                Assert.AreEqual("$200", priceSpan.Text);
            });
        }

        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Check existence of Sales phone number in body of Free Trial page")]
        public void CheckSupportPhoneNumberOnFreeTrial()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to Free Trial");
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/pricing/free-trial/");

                Logger.Instance.WriteLine("STEP 2: Check Sales phone number in body");
                IWebElement phoneNumber = driver.WaitUntil(() => driver.FindElement(By.CssSelector("[data-control='geophone']")), "Unable to find phone number");
                Assert.IsTrue(phoneNumber.Text.Length > 0, "Phone Number is zero-length");
            });
        }

        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Navigate to Free Trial FAQs and verify Expand/Collapse behavior")]
        public void FreeTrialFAQExpandCollapse()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to Free Trial");
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/pricing/free-trial/");

                // STEP 2: Navigate to FAQs
                driver.WaitForURLChange(() => driver.FindElement(By.LinkText("Frequently Asked Questions")).Click());

                //STEP 3: Verify FAQs Expand
                driver.FindElement(By.ClassName("expand")).Click();
                IWebElement FAQanswer = driver.FindElement(By.ClassName("wa-faq-answer"));
                Assert.AreEqual("block", FAQanswer.GetCssValue("display"));

                //STEP 4: Verify FAQs Collapse
                driver.FindElement(By.ClassName("collapse")).Click();
                FAQanswer = driver.FindElement(By.ClassName("wa-faq-answer"));
                Assert.AreEqual("none", FAQanswer.GetCssValue("display"));
            });
        }

        [TestMethod]
        [TestCategory(Categories.P2)]
        [Description("Verify animated slideshow on Free Trial page functioning")]
        public void SlideshowControlOnFreeTrial()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to Free Trial");
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/pricing/free-trial/");

                Logger.Instance.WriteLine("STEP 2: Find slideshow control");
                IWebElement slideshow = driver.FindElement(By.ClassName("slideshow-control"));
                int count = slideshow.FindElements(By.TagName("div")).Count;
                Assert.AreEqual(4, count, "Slide show does not have 4 items");

                Logger.Instance.WriteLine("STEP 3: Verify slideshow animation");
                for (int i = 0; i < count; i++)
                {
                    string className = "s-" + i.ToString();
                    driver.WaitUntil(() => slideshow.FindElement(By.ClassName(className)).Displayed, "Slideshow control not animating to slide # " + i.ToString(), System.TimeSpan.FromSeconds(30));
                }

                Logger.Instance.WriteLine("STEP 4: Verify clicking on each animation slide");
                var slideshowBulletLinks = driver.FindElement(By.ClassName("s-ctrl")).FindElements(By.TagName("a"));
                foreach (IWebElement link in slideshowBulletLinks)
                {
                    string selectedClassName = link.FindParentElement().GetAttribute("class").Substring(0, 3);
                    link.Click();
                    //Verify content is displayed
                    driver.WaitUntil(() => slideshow.FindElement(By.ClassName(selectedClassName)).Displayed, "Slideshow control not changing onclick to slide # " + selectedClassName);
                }

            });
        }

    }

}


