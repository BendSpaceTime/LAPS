namespace Azure.Automation.WindowsAzurePortal
{
    using System.Linq;
    using System.Collections.Generic;
    using Azure.Automation.Helpers;
    using Azure.Automation.Selenium.Extensions;
    using Microsoft.Selenium.Utilities;
    using OpenQA.Selenium;
    using System.Net;
    using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
    using Azure.Automation.Helpers.JsonData;
    using System;

    public class LocCurrencyHelpers
    {
        public static List<string> GetAllLangCultures()
        {
            JCultures culture = JsonHelper.ExtractDataFromJson<JCultures>("/api/cache/cultures/");
            List<string> languages = new List<string>();
            foreach (LData data in culture.Data.Cultures)
            {
                if (data.IsDisplayed.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    languages.Add(data.Slug);
                }
            }
            return languages;
        }

        public static void ChangeLangByCulture(string culture, IWebDriver driver)
        {
            //Click to expand table
            driver.WaitUntil(() => driver.FindElement(By.CssSelector("a[class='current-locale']")), "Unable to find Lang Selector");
            driver.FindElement(By.CssSelector("div[class='locale-selector'] span")).Click();

            driver.WaitUntil(() => driver.FindElement(By.CssSelector("div[class='locale-selection-panel site-flag site-flag-lang']")), "Unable to expand language list");
            driver.FindElement(By.XPath("//li/a[contains(@data-loc,'" + culture + "')]")).Click();
        }
        public static void ChangeCurrencyByName(string curName, IWebDriver driver)
        {
            //Click to expand table
            driver.WaitUntil(() => driver.FindElement(By.ClassName("current-default-currency")), "Unable to find Currency Selector");
            driver.FindElement(By.ClassName("current-default-currency")).Click();

            driver.WaitUntil(() => driver.FindElement(By.CssSelector("div[class='currency-selection-panel site-flag site-flag-lang']")), "Unable to expand currency list");
            driver.FindElement(By.LinkText(curName)).Click();
        }
        public static void ResetCookiesForLocale(IWebDriver driver)
        {
            driver.Manage().Cookies.DeleteAllCookies();
        }

        public static Dictionary<string, string> GetAllLangCulturesAndFallbacks()
        {
            var culture = JsonHelper.ExtractDataFromJson<JCultures>("/api/cache/cultures/");
            var languages = new Dictionary<string, string>();
            foreach (var data in culture.Data.Cultures)
            {
                if (data.IsSupported.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    languages.Add(data.Slug, data.Slug);
                }
                else
                {
                    languages.Add(data.Slug, data.FallbackCultureSlugs.First());   
                }
            }

            return languages;
        }
    }
}
