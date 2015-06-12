namespace Azure.Automation.Fixtures
{
    using Azure.Automation.WindowsAzurePortal;
    using Azure.Automation.Helpers;
    using Microsoft.Selenium.Utilities;
    using Azure.Automation.Selenium.Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using System.Collections.Generic;

    [TestClass]
    public class HomePageFixture : BaseFixture
    {
        [TestMethod]
        [TestCategory(Categories.Home)]
        [Description("Load the home page and check for CoreCSS styles applied to page elements")]
        public void CheckHomePageCoreCssStyles()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                // STEP 1: Navigate to homepage
                CommonSeleniumSteps.NavigateToHomepage(driver);

                // STEP 2: Check CoreCSS styles
                CommonSeleniumSteps.CheckCoreCssStylesAreApplied(driver);
            });
        }

        [TestMethod]
        [TestCategory(Categories.Prod)]
        [Description("Load the home page in prod and verify no CDN fallback")]
        public void VerifyCDNInProd()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                // STEP 1: Navigate to homepage
                CommonSeleniumSteps.NavigateToHomepage(driver);

                // STEP 2: Find an image and verify the source is from CDN
                IWebElement firstImage = driver.FindElement(By.TagName("img"));
                Assert.IsTrue(firstImage.GetAttribute("src").Contains("acom.azurecomcdn.net"), "CDN fell back; image coming from: " + firstImage.GetAttribute("src"));
            });
        }

        [TestMethod]
        [TestCategory(Categories.Home)]
        [Description("Load the home page and check JS environment is properly setup. Checks whether the jQuery, Core and Wacom global JS variables exist.")]
        public void CheckHomePageJQueryEnvironmentVars()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                // STEP 1: Navigate to homepage
                CommonSeleniumSteps.NavigateToHomepage(driver);

                // STEP 2: Check js environment
                CommonSeleniumSteps.CheckJsEnvironmentInitialized(driver);
            });
        }

        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Check existence of Sales phone number in header of Homepage")]
        public void CheckSupportPhoneNumberOnHomePage()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to homepage");
                CommonSeleniumSteps.NavigateToHomepage(driver);

                Logger.Instance.WriteLine("STEP 2: Check Sales phone number in header");
                IWebElement phoneNumber = driver.FindElement(By.XPath("//span[@data-control='geophone']"));

                Logger.Instance.WriteLine("Phone # text is: " + phoneNumber.Text); 
                Assert.IsTrue(phoneNumber.Text.Length > 0, "Phone Number is zero-length");
            });
        }

        [TestMethod]
        [TestCategory(Categories.Prod)]
        [Description("Verify HomeCss.css is loaded")]
        public void VerifyHomeCSSLoads()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to homepage");
                CommonSeleniumSteps.NavigateToHomepage(driver);

                Logger.Instance.WriteLine("STEP 2: Check existence of HomeCss.css in css links");
                var cssLinks = driver.FindElements(By.TagName("link"));
                bool isMatched = false;

                foreach (IWebElement link in cssLinks)
                {
                    string cssFileName = link.GetAttribute("href");
                    if (cssFileName.Contains("HomeCss"))
                    {
                        isMatched = true;
                        break;
                    }
                }

                Assert.IsTrue(isMatched, "Unable to find HomeCss link");
            });
        }

        [TestMethod]
        [TestCategory(Categories.P0)]
        [Description("Verify able to change to one language and back to English")]
        public void ChangeOneLanguageOnHomePage()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to homepage");
                CommonSeleniumSteps.NavigateToHomepage(driver);

                Logger.Instance.WriteLine("STEP 2: Change language to Swedish");
                driver.WaitForURLChange(() => LocCurrencyHelpers.ChangeLangByCulture("sv-se", driver));
                Assert.IsTrue(driver.Url.ToString().Contains("sv-se"), "URL is not Swedish: " + driver.Url.ToString());
                Assert.AreEqual("Molnet för det moderna företaget", driver.FindElements(By.CssSelector("div[class='wa-spacer wa-spacer-8down'] h1"))[0].Text, "Page H1 text incorrect");

                Logger.Instance.WriteLine("STEP 3: Change language back to English");
                driver.WaitForURLChange(() => LocCurrencyHelpers.ChangeLangByCulture("en-us", driver));
                Assert.IsTrue(driver.Url.ToString().Contains("en-us"), "URL is not English: " + driver.Url.ToString());
                Assert.AreEqual("The cloud for modern business", driver.FindElements(By.CssSelector("div[class='wa-spacer wa-spacer-8down'] h1"))[0].Text, "Page H1 text incorrect");

            });
        }

        [TestMethod]
        [TestCategory(Categories.P0)]
        [Description("Verify able to change to all languages")]
        public void ChangeAllLanguagesOnHomePage()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to homepage");
                CommonSeleniumSteps.NavigateToURL(driver, "/ja-jp/");

                Logger.Instance.WriteLine("STEP 2: Change each language and verify URL culture");
                List<string> langs = LocCurrencyHelpers.GetAllLangCultures();
                foreach (string lang in langs)
                {
                    driver.WaitForURLChange(() => LocCurrencyHelpers.ChangeLangByCulture(lang, driver));
                    driver.WaitForPageToLoad();
                    Logger.Instance.WriteLine(lang);
                    Assert.IsTrue(driver.Url.ToString().Contains(lang), "URL is not correct.  Expected: " + lang + "; Actual: " + driver.Url.ToString());
                    Assert.AreEqual(homePageH1TextByLang[lang], driver.FindElements(By.CssSelector("div[class='wa-spacer wa-spacer-8down'] h1"))[0].Text, "Page H1 text incorrect for: " + homePageH1TextByLang[lang]);
                }

                //cleanup back to English
                driver.WaitForURLChange(() => LocCurrencyHelpers.ChangeLangByCulture("en-us", driver));
            });
        }

        [TestMethod]
        [TestCategory(Categories.P0)]
        [Description("Verify Free Trial Currency Changes from USD to JPY in bottom section of home page")]
        public void VerifyFreeTrialCurrencyChange()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                // STEP 1: Navigate to HomePage
                LocCurrencyHelpers.ResetCookiesForLocale(driver);
                CommonSeleniumSteps.NavigateToHomepage(driver);

                // STEP 2: Find Price for Free Trial Credit
                IWebElement priceSpan = driver.FindElement(By.CssSelector("section.wa-sectionGetStartedNext")).FindElement(By.CssSelector("span[class='stored-price no-convert price-data'][data-pricekey='monetaryCredit.freeTrial']"));

                //STEP 3: Verify price is $200 USD
                Assert.AreEqual("$200", priceSpan.Text);

                Logger.Instance.WriteLine("Switch to Japanese Yen");
                LocCurrencyHelpers.ChangeCurrencyByName("Japanese Yen (¥)", driver);

                driver.WaitForPageToLoad();
                Logger.Instance.WriteLine("Verify price changed to Yen");
                priceSpan = driver.FindElement(By.CssSelector("section.wa-sectionGetStartedNext")).FindElement(By.CssSelector("span[class='stored-price no-convert price-data'][data-pricekey='monetaryCredit.freeTrial']"));
                Assert.AreEqual("¥20,500", priceSpan.Text);

                //clean up and set back to USD
                LocCurrencyHelpers.ChangeCurrencyByName("US Dollar ($)", driver);
            });
        }

        public Dictionary<string, string> homePageH1TextByLang = new Dictionary<string, string>(){
            {"en-us","The cloud for modern business"},
            {"cs-cz","Moderní podnikový cloud"},
            {"da-dk","Skyen for moderne forretninger"},
            {"de-de","Die Cloud für ein modernes Geschäft"},
            {"en-in","The cloud for modern business"},
            {"en-gb","The cloud for modern business"},
            {"es-es","La nube de la nueva empresa"},
            {"fi-fi","The cloud for modern business"},
            {"fr-fr","Le cloud pour les entreprises modernes"},
            {"el-gr","The cloud for modern business"},
            {"it-it","Il cloud per aziende moderne"},
            {"hu-hu","A modern cégek felhője"},
            {"nl-nl","De cloud voor moderne bedrijven"},
            {"nb-no","The cloud for modern business"},
            {"pl-pl","Rozwiązania w chmurze dla nowoczesnych firm"},
            {"pt-br","A nuvem para a empresa moderna"},
            {"pt-pt","A nuvem para as empresas modernas"},
            {"sv-se","Molnet för det moderna företaget"},
            {"ro-ro","The cloud for modern business"},
            {"tr-tr","Modern iş hayatı için bulut"},
            {"uk-ua","The cloud for modern business"},
            {"ru-ru","Облако для современного бизнеса"},
            {"ja-jp","最新のビジネスに対応したクラウド"},
            {"ko-kr","현대 비즈니스에 적합한 클라우드"},
            {"zh-cn","用于新式业务的云"},
            {"zh-tw","現代化商務的雲端"}
        };
    }
}
