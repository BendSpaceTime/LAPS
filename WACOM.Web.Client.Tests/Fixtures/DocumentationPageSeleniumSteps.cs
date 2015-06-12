namespace Azure.Automation.Fixtures
{
    using System;
    using System.Linq;
    using Azure.Automation.Helpers;
    using Azure.Automation.Selenium.Extensions;
    using OpenQA.Selenium;
    using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

    public class DocumentationPageSeleniumSteps
    {
        public static void ClickAndNavigateToFirstAvailableDocumentationByServiceIndexPage(IWebDriver driver)
        {
            Logger.Instance.LogAction("Find first services documentation link available and navigate", () =>
            {
                if (!driver.Title.StartsWith("Documentation"))
                {
                    throw new Exception("Browser must be in the documentation section landing page");
                }

                var serviceDocumentationIndexLinkElement = driver.FindElement(By.CssSelector("a[href*='documentation/services']"));
                serviceDocumentationIndexLinkElement.Click(true);

                driver.WaitForPageToLoad();
            });
        }

        public static void ClickAndNavigateToFirstAvailableDocumentationArticle(IWebDriver driver)
        {
            Logger.Instance.LogAction("Find first documentation article link available and navigate", () =>
            {
                driver.WaitForURLChange(() =>
                {
                    var articleLinkElement = driver.WaitFindElement(By.CssSelector(".section.tutorials .article-group a[href]"), 30);
                    articleLinkElement.Click(true);
                });
            });
        }

        public static void ClickDocumentationLastTocLinkAndCheckTitleIsDisplayed(IWebDriver driver)
        {
            string targetLinkId = null;
            Logger.Instance.LogAction("Find last TOC link element and click it", () =>
            {
                var lastTocLinkElement = driver.FindElement(By.CssSelector("a[href='#next-steps']"));
                lastTocLinkElement.Click();

                // Actual targets have an underscore prepended to the Id. Navigation to the title element is done via custom JS
                targetLinkId = "_" + lastTocLinkElement.GetAttribute("href").Split('#').Last();
            });

            Logger.Instance.LogAction(string.Format("Find '{0}' anchor element and check wrapping title element is visible in viewport", targetLinkId), () =>
            {
                var anchorElement = driver.FindElement(By.Id(targetLinkId));

                var titleElement = anchorElement.FindParentElement();

                Assert.IsTrue(titleElement.IsVisibleInViewport(), "Title element is not visible in the viewport");
            });
        }
    }
}
