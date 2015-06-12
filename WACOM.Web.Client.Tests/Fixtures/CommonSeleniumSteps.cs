namespace Azure.Automation.WindowsAzurePortal
{
    using System.Linq;
    using Azure.Automation.Helpers;
    using Azure.Automation.Selenium.Extensions;
    using OpenQA.Selenium;
    using System.Net;
    using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
    using System.Collections.Generic;

    public class CommonSeleniumSteps
    {
        private static string[] environmentVars = new[] { "jQuery" };
        private static string userAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

        public static void NavigateToHomepage(IWebDriver driver)
        {
            Logger.Instance.LogAction("Navigate to home page: " + TestConfiguration.Instance.EnvironmentUrl, () =>
            {
                driver.Navigate().GoToUrl(TestConfiguration.Instance.EnvironmentUrl);
                driver.WaitForPageToLoad();
            });
        }

        public static void NavigateToURL(IWebDriver driver, string urlFragment)
        {
            string fullURLPath = TestConfiguration.Instance.EnvironmentUrl + urlFragment;
            Logger.Instance.LogAction("Navigate to URL: "+ fullURLPath, () =>
            {
                driver.Navigate().GoToUrl(fullURLPath);
                driver.WaitForPageToLoad();
            });
        }

        public static void ClickAndNavigateToSection(IWebDriver driver, string urlFragment)
        {
            Logger.Instance.LogAction(string.Format("Find '{0}' section link and navigate", urlFragment), () =>
            {
                driver.WaitForURLChange(() =>
                {
                    var sectionLinkElement = driver.FindElement(By.CssSelector(string.Format(".dev-navigation a[href*='{0}']", urlFragment)));
                    sectionLinkElement.Click();
                });
            });
        }

        public static void CheckDisqusCommentBoardIsAvailable(IWebDriver driver)
        {
            Logger.Instance.LogAction("Find disqus iframe element and switch context into it", () =>
            {
                // Expect visible waits for the frame height to be > 0px (set after the disqus board loads)
                var disqusFrameElement = driver.WaitFindElement(By.CssSelector("iframe[src*='disqus.com']"), 30, expectVisible: true);

                driver.SwitchTo().Frame(disqusFrameElement);
            });

            Logger.Instance.LogAction("Check disqus comment textbox is available", () =>
            {
                driver.WaitFindElement(By.ClassName("textarea"), 30, expectVisible: true);
            });

            Logger.Instance.LogAction("Switch driver back to top frame", () =>
            {
                driver.SwitchTo().DefaultContent();
            });
        }

        public static void CheckCoreCssStylesAreApplied(IWebDriver driver)
        {
            Logger.Instance.LogAction("Check if page header element is fixed to top of browser viewport", () =>
            {
                var headerElementComputedStyle = driver.GetElementComputedStyle("body > form > .wa-container > header");

                Assert.AreEqual("fixed", headerElementComputedStyle.Position);
                Assert.AreEqual("0px", headerElementComputedStyle.Top);
            });

            Logger.Instance.LogAction("Check if main form has the same size as the whole document (HTML element)", () =>
            {
                var htmlStyle = driver.GetElementComputedStyle("html");
                var formStyle = driver.GetElementComputedStyle("body > form");

                Assert.AreEqual(htmlStyle.Width, formStyle.Width, "Form should cover the entire width of document");
            });
        }

        public static void CheckJsEnvironmentInitialized(IWebDriver driver)
        {
            Logger.Instance.LogAction("Check jQuery, Core and Wacom JS environment variables are properly setup", () =>
            {
                var exstingVars = driver.CheckJsVariablesExist(environmentVars);
                var missingVars = environmentVars.Except(exstingVars);

                Assert.IsTrue(missingVars.Count() == 0, string.Format("'{0}' js variables were not found", string.Join("', '", missingVars)));
            });
        }

        public static string GetHTTPStatusCode(string URLPath)
        {
            // Creates an HttpWebRequest for the specified URL. 
            HttpWebRequest pageRequest = (HttpWebRequest)WebRequest.Create(URLPath);
            pageRequest.UserAgent = userAgent;
            HttpWebResponse pageResponse = null;
            try
            {
                // Sends the HttpWebRequest and waits for a response.
                pageResponse = (HttpWebResponse)pageRequest.GetResponse();
            }
            catch
            {
                //error cases like 404 would throw
                return pageResponse.StatusCode.ToString();
            }
            
            pageResponse.Close();
            return pageResponse.StatusCode.ToString();
        }

        public static bool VerifyUrlsAreAvailableAndNotRelativePath(IList<IWebElement> urls, out string failInfo)
        {
            if (urls == null || urls.Count == 0)
            {
                failInfo = "The first input argument is invalid for the fuc 'VerifyUrlsAreAvailableAndNotRelativePath'";
                return false;
            }
            foreach (IWebElement imgElement in urls)
            {
                var imageUrl = imgElement.GetAttribute("src");
                if (imageUrl == null)
                {
                    failInfo = "Element doesn't have a src attribute ";
                    return false;
                }

                if(!(imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://")))
                {
                    failInfo = "this image src url is relative path " + imageUrl;
                    return false;
                }

                if (CommonSeleniumSteps.GetHTTPStatusCode(imageUrl) != HttpStatusCode.OK.ToString())
                {
                    failInfo = "This img can not be loaded: " + imageUrl;
                    return false;
                }
            }
            failInfo = null;
            return true;
        }

        public static bool VerifyUrlsAreAvailableAndNotRelativePath(IList<IWebElement> urls, out string failInfo, string attribute = "src")
        {
            if (urls == null || urls.Count == 0)
            {
                failInfo = "The first input argument is invalid for the fuc 'VerifyUrlsAreAvailableAndNotRelativePath'";
                return false;
            }
            foreach (IWebElement imgElement in urls)
            {
                var imageUrl = imgElement.GetAttribute(attribute);
                if (imageUrl == null)
                {
                    failInfo = "Element doesn't have a "+attribute+" attribute ";
                    return false;
                }

                if (!(imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://")))
                {
                    failInfo = "this image src url is relative path " + imageUrl;
                    return false;
                }

                if (CommonSeleniumSteps.GetHTTPStatusCode(imageUrl) != HttpStatusCode.OK.ToString())
                {
                    failInfo = "This img can not be loaded: " + imageUrl;
                    return false;
                }
            }
            failInfo = null;
            return true;
        }

        public static string CheckURLRedirect(string URLPath, out string destinationUrl)
        {
            destinationUrl = string.Empty;
            HttpWebRequest pageRequest = (HttpWebRequest)WebRequest.Create(URLPath);
            pageRequest.UserAgent = userAgent;
            pageRequest.AllowAutoRedirect = false;
            HttpWebResponse pageResponse = null;
            try
            {
                // Sends the HttpWebRequest and waits for a response.
                pageResponse = (HttpWebResponse)pageRequest.GetResponse();
                destinationUrl = pageResponse.Headers["Location"];
            }
            catch
            {
                return pageResponse.StatusCode.ToString();
            }

            pageResponse.Close();
            return pageResponse.StatusCode.ToString();
        }
    }
}
