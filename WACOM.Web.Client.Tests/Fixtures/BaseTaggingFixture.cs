namespace WACOM.Web.Client.Tests.Fixtures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Azure.Automation.Fixtures;
    using Azure.Automation.Helpers;
    using Azure.Automation.Selenium.Extensions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OpenQA.Selenium;

    public class BaseTaggingFixture : BaseFixture
    {
        private static FiddlerProxy fiddlerProxy;
        protected static readonly List<string> requestList = new List<string>();

        private static void OnRequestHandler(string requestUrl)
        {
            BaseTaggingFixture.requestList.Add(requestUrl);
        }

        protected static void BaseClassInitialize()
        {
            BaseTaggingFixture.fiddlerProxy = new FiddlerProxy(BaseTaggingFixture.OnRequestHandler);
            BaseTaggingFixture.fiddlerProxy.StartProxy();
        }

        protected static void BaseClassCleanup()
        {
            BaseTaggingFixture.fiddlerProxy.DoQuit();
        }

        protected void RunTaggingTests(string url, TaggingAssertions assertions)
        {
            this.CreateSeleniumTestContext(useProxy: true).Run(
                driver =>
                    {
                        driver.WaitForPageToLoad(240); // set a timeout of 240 since first request takes longer due to proxy initialization

                        var fullURLPath = Azure.Automation.Helpers.TestConfiguration.Instance.EnvironmentUrl + url;

                        foreach (var assertion in assertions.Assertions)
                        {
                            this.NavigateToUrl(driver, fullURLPath);
                            this.ClickElement(driver, assertion);
                            this.AssertTagExists(driver, assertion, false);
                        }

                        var failedAssertions = assertions.Assertions.Where(a => a.Result == TaggingAssertionResult.Failed).ToList();

                        if (failedAssertions.Any())
                        {
                            var failedAssertionsStr = string.Empty;
                            foreach (var a in failedAssertions)
                            {
                                failedAssertionsStr += a.ElementDisplayName + "|";
                            }

                            Assert.Fail("Failed assertions: {0}", failedAssertionsStr);
                        }
                    });
        }

        protected void AssertTagExists(IWebDriver driver, TaggingAssertion assertion, bool logRequests = true)
        {
            var clonedRequestList = new List<string>();

            try
            {
                driver.Wait(5, message: "Tag not found").Until(d =>
                {
                    clonedRequestList = new List<string>(BaseTaggingFixture.requestList);

                    assertion.Result = clonedRequestList.Count(req => req.Contains(assertion.ExpectedTag)) == 1
                           ? TaggingAssertionResult.Successful
                           : TaggingAssertionResult.Failed;

                    return assertion.Result == TaggingAssertionResult.Successful;
                });
            }
            catch (Exception)
            {
            }

            if (logRequests)
            {
                this.LogRequests(clonedRequestList);
            }

            if (assertion.Result == TaggingAssertionResult.Successful)
            {
                Logger.Instance.Log("SUCCESS", string.Format("Tag {0} found on request {1}", assertion.ExpectedTag, clonedRequestList.Single(a => a.Contains(assertion.ExpectedTag))));
            }
            else
            {
                Logger.Instance.Log("FAIL", string.Format("Tag {0} not found", assertion.ExpectedTag));
            }
        }

        protected void LogRequests(List<string> requests)
        {
            var requestsStr = string.Empty;
            if (requests.Any())
            {
                foreach (var r in requests)
                {
                    requestsStr += r + "|";
                }

                Logger.Instance.Log("INFO", string.Format("List of requests: {0}", requestsStr));
            }
            else
            {
                Logger.Instance.Log("INFO", "No requests were intercepted by the proxy");
            }
        }

        protected void ClickElement(IWebDriver driver, TaggingAssertion assertion)
        {
            Logger.Instance.LogAction(
                string.Format("Clicking on element '{0}' (selector '{1}')", assertion.ElementDisplayName, assertion.ElementSelector),
                () =>
                    {
                        if (Azure.Automation.Helpers.TestConfiguration.Instance.DisableElementNavigation)
                        {
                            var jsExecutor = driver as IJavaScriptExecutor;
                            jsExecutor.ExecuteScript(string.Format("$('{0}').on('click', false)", assertion.ElementSelector));
                        }

                        var anchor = driver.FindElement(By.CssSelector(assertion.ElementSelector));
                        BaseTaggingFixture.requestList.Clear();
                        //anchor.Click(true);
                        //Simulate click with mouse
                        var jsExecutorMouseClick = driver as IJavaScriptExecutor;
                        jsExecutorMouseClick.ExecuteScript(string.Format("$('{0}').mousedown().mouseup().click()", assertion.ElementSelector));
                    });
        }

        protected void NavigateToUrl(IWebDriver driver, string fullURLPath)
        {
            Logger.Instance.LogAction(
                string.Format(
                    "Navigate to URL (using proxy @ {0}): {1}",
                    Azure.Automation.Helpers.TestConfiguration.Instance.ProxyAddress,
                    fullURLPath),
                () => driver.Navigate().GoToUrl(fullURLPath));
        }
    }

    public class TaggingAssertions
    {
        private readonly List<TaggingAssertion> assertions = new List<TaggingAssertion>();

        public static TaggingAssertions Create(params TaggingAssertion[] assertions)
        {
            var result = new TaggingAssertions();

            result.Assertions.AddRange(assertions);

            return result;
        }

        public List<TaggingAssertion> Assertions
        {
            get
            {
                return this.assertions;
            }
        }
    }

    public class TaggingAssertion
    {
        public TaggingAssertion()
        {
            this.Result = TaggingAssertionResult.NotExecuted;
        }

        public string ElementDisplayName { get; set; }

        public string ElementSelector { get; set; }

        public string ExpectedTag { get; set; }

        public TaggingAssertionResult Result { get; set; }
    }

    public enum TaggingAssertionResult
    {
        NotExecuted,
        Successful,
        Failed
    }
}
