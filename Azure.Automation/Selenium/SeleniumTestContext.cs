namespace Azure.Automation.Selenium
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Xml.Serialization;
    using Azure.Automation.BrowserStack;
    using Azure.Automation.Helpers;
    using Azure.Automation.Helpers.Configuration;
    using Azure.Automation.Selenium.Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Remote;
    using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
    using TestConfiguration = Azure.Automation.Helpers.TestConfiguration;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements", Justification = "Singleton pattern code grouping")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Singleton pattern code grouping")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Singleton pattern code grouping")]
    public class SeleniumTestContext
    {
        private const string LocalSeleniumTestingProfileName = "Local";

        private static readonly DateTime TestInitializationTime = DateTime.Now.ToUniversalTime();
        private static readonly IEnumerable<Func<Exception, bool>> InfrastructureExceptionVerifications = new[] { (Func<Exception, bool>)IsBrowserstackTimeoutException };

        private List<WebDriverTargetBrowser> targetBrowsers = new List<WebDriverTargetBrowser>();
        private List<WebDriverTargetBrowser> unsupportedBrowsers = new List<WebDriverTargetBrowser>();
        private List<string> errors = new List<string>();
        private string testName;
        private string buildName;

        public bool UseProxy { get; set; }

        #region Static access

        private static SeleniumTestContext instance;

        public static SeleniumTestContext Current
        {
            get
            {
                if (instance == null)
                {
                    throw new Exception("Test Context not initialized");
                }

                return instance;
            }
        }

        #endregion

        public SeleniumTestContext(string testName, string buildName, IEnumerable<WebDriverTargetBrowser> unsupportedBrowsers)
        {
            this.testName = testName;
            this.buildName = buildName;
            this.unsupportedBrowsers.AddRange(unsupportedBrowsers);

            // load target browsers from XML
            this.LoadProfilesFromConfiguration();
        }

        public SeleniumTestContext(string testName, string buildName)
            : this(testName, buildName, new List<WebDriverTargetBrowser>())
        {
        }

        public SeleniumTestContext(string testName, string build, WebDriverTargetBrowser targetBrowser)
        {
            this.testName = testName;
            this.buildName = build;

            this.targetBrowsers.Add(targetBrowser);
        }

        #region Infrastructure exception validation methods

        private static bool IsBrowserstackTimeoutException(Exception ex)
        {
            var webEx = ex as WebDriverException;
            return ex != null && ex.InnerException != null && (ex.InnerException.Message == "The request was aborted: The operation has timed out." || ex.Message.StartsWith("The HTTP request to the remote WebDriver server for URL"));
        }

        #endregion

        public void FailOnEnd(string error)
        {
            this.errors.Add(error);
        }

        public void Run(Action<IWebDriver> test)
        {
            // Generate single element combineData collection to run single run with each browser
            this.Run((driver, data) => test(driver), Enumerable.Range(1, 1));
        }

        public void Run<T>(Action<IWebDriver, T> test, IEnumerable<T> combineData)
        {
            var testPlan = new TestPlan<T>(combineData);
            this.Run(testPlan.WithTestIterator(test));
        }

        public void Run<T>(TestPlan<T> testPlan)
        {
            // Failed tests: tests that failed due to unmet test requirements (missing page elements, wrong content, etc.)
            // Inconclusive tests: tests that failed due to underlying mechanism failures (browserstack timeout, selenium browser integration issues, etc.)
            int failedTests = 0, inconclusiveTests = 0;
            var totalTests = this.targetBrowsers.Count;

            Logger.Instance.LogTestStart(totalTests);

            foreach (var targetBrowser in this.targetBrowsers)
            {
                if (!TestConfiguration.Instance.IncludeUnsupportedBrowsers && this.unsupportedBrowsers.Contains(targetBrowser))
                {
                    Logger.Instance.LogUnsupportedBrowser(targetBrowser);
                    continue;
                }

                Logger.Instance.LogBrowserProfile(targetBrowser);

                var uniqueBuildName = string.Format("{0}-{1}", this.buildName, TestInitializationTime.ToString("yyyyMMddHHmm"));
                OpenQA.Selenium.IWebDriver driver = null; 
                instance = this;
                                
                try
                {
                    if (TestConfiguration.Instance.BrowserProfiles == LocalSeleniumTestingProfileName)
                    {
                        ChromeOptions options = new ChromeOptions();
                        //to get rid of Chrome bug in latest version of warning popup banner
                        options.AddArguments("test-type");

                        if (this.UseProxy)
                        {
                            var proxy = new Proxy();
                            proxy.HttpProxy = TestConfiguration.Instance.ProxyAddress;

                            options.Proxy = proxy;
                        }

                        ChromeDriverService CDService = ChromeDriverService.CreateDefaultService();
                        driver = new ChromeDriver(CDService, options);
                    }
                    else
                        driver = (ScreenshotRemoteWebDriver)this.InitializeDriver(this.testName, uniqueBuildName, targetBrowser);
                }
                catch (Exception ex)
                {
                    Logger.Instance.WriteLine("TEST NOT RUN: " + ex.Message);
                    inconclusiveTests++;
                }

                if (driver != null)
                {
                    var stopwatch = new Stopwatch();

                    try
                    {
                        stopwatch.Start();

                        // Always run test maximized
                        driver.Manage().Window.Maximize();
                        testPlan.Run(driver);

                        this.Verify();
                    }
                    catch (TestValidationContextVerificationFailed ex)
                    {
                        foreach (var line in ex.Message.Split('\n'))
                        {
                            Logger.Instance.WriteLine("TEST FAILED: " + line);
                        }

                        failedTests++;
                    }
                    catch (Exception ex)
                    {
                        if (ex is AssertInconclusiveException || this.IsInfrastructureException(ex))
                        {
                            Logger.Instance.WriteLine("TEST INTERRUPTED: " + ex.Message);
                            inconclusiveTests++;
                        }
                        else
                        {
                            Logger.Instance.WriteLine("TEST FAILED: " + ex.Message);
                            failedTests++;
                        }
                    }
                    finally
                    {
                        instance = null;

                        stopwatch.Stop();
                        if (TestConfiguration.Instance.LogTimings)
                        {
                            Logger.Instance.WriteLine("TEST DURATION: {0} s", stopwatch.ElapsedMilliseconds / 1000);
                        }

                        // Screenshot of final state of the page
                        driver.TryTakeScreenshot();
                        driver.Quit();

                        if (TestConfiguration.Instance.BrowserProfiles != LocalSeleniumTestingProfileName)
                        {   
                            // Local tests do not run with browserstack
                            ScreenshotRemoteWebDriver SRdriver = (ScreenshotRemoteWebDriver)driver;
                            var sessionId = (string)SRdriver.Capabilities.GetCapability("webdriver.remote.sessionid");
                            this.LogBrowserStackSessionUrl(sessionId);
                        }
                    }
                }

                Logger.Instance.WriteSeparatorLine();
            }

            Logger.Instance.LogTestEnd();

            if (failedTests > 0)
            {
                Assert.Fail(string.Format("{0} of {1} browser configurations failed.", failedTests, totalTests));
            }
            else if (inconclusiveTests > 0)
            {
                Assert.Inconclusive(string.Format("{0} of {1} browser configurations were not tested.", inconclusiveTests, totalTests));
            }
        }

        public bool IsInfrastructureException(Exception ex)
        {
            return InfrastructureExceptionVerifications.Any(v => v(ex));
        }

        private void Verify()
        {
            if (this.errors.Any())
            {
                throw new TestValidationContextVerificationFailed(this.errors);
            }
        }

        private ScreenshotRemoteWebDriver InitializeDriver(string testName, string build, WebDriverTargetBrowser targetBrowser)
        {
            return ScreenshotRemoteWebDriver.InitializeRemoteDriver(testName, build, targetBrowser, this.UseProxy);
        }

        private void LoadProfilesFromConfiguration()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BrowserProfiles));
            BrowserProfiles allProfilesConfiguration = (BrowserProfiles)serializer.Deserialize(new System.IO.StreamReader("BrowserProfiles.xml"));
            var profilesToRun = TestConfiguration.Instance.BrowserProfiles.Split(',').ToList();
            profilesToRun.ForEach(n => n.Trim());

            var profilesToAdd = allProfilesConfiguration.Profiles.Where(p => profilesToRun.Contains(p.Name));
            this.targetBrowsers.AddRange(profilesToAdd.SelectMany(p => p.Browsers.Select(b => b.ToWebDriverTargetBrowser(p.Name))));
        }

        private void LogBrowserStackSessionUrl(string sessionId)
        {
            Logger.Instance.WriteSeparatorLine();

            try
            {
                var completeBuildName = string.Format("{0}-{1}", this.buildName, TestInitializationTime.ToString("yyyyMMddHHmm"));
                Logger.Instance.WriteLine("BrowserStack Build: " + completeBuildName);

                var service = new AutomateSessionsService();

                if (!string.IsNullOrEmpty(sessionId))
                {
                    var url = service.GetSessionUrl(sessionId);
                    Logger.Instance.WriteLine("BrowserStack session Url: " + url);
                }
                else
                {   // SessionId is not available, log the build Url
                    var build = service.GetBuild(completeBuildName);
                    Logger.Instance.WriteLine("BrowserStack build Url: {0}", build.Url);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteLine("Unable to get BrowserStack session log Url: " + ex.Message);
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Minimal exception class")]
    public class TestValidationContextVerificationFailed : Exception
    {
        public TestValidationContextVerificationFailed(IEnumerable<string> messages)
            : base(string.Join("\n", messages))
        {
            this.Messages = messages;
        }

        public IEnumerable<string> Messages { get; private set; }
    }
}
