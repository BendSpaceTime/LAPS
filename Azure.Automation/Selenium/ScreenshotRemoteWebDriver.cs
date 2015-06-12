namespace Azure.Automation.Selenium
{
    using System;
    using Azure.Automation.Helpers;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Remote;
    
    public class ScreenshotRemoteWebDriver : RemoteWebDriver, ITakesScreenshot
    {
        private static readonly string BrowserStackProject = TestConfiguration.Instance.Project;

        public ScreenshotRemoteWebDriver(Uri uri, DesiredCapabilities dc)
            : base(uri, dc)
        {
        }

        public static ScreenshotRemoteWebDriver InitializeRemoteDriver(string testName, string build, WebDriverTargetBrowser targetBrowser, bool useProxy)
        {
            DesiredCapabilities capabilities = DesiredCapabilities.Chrome();
            capabilities.SetCapability("browserstack.user", TestConfiguration.Instance.BrowserStackUser);
            capabilities.SetCapability("browserstack.key", TestConfiguration.Instance.BrowserStackKey);
            targetBrowser.SetCapabilities(capabilities);

            // Enable visual logs
            capabilities.SetCapability("browserstack.debug", "true");

            capabilities.SetCapability("project", BrowserStackProject);
            capabilities.SetCapability("build", build);

            // Session name
            capabilities.SetCapability("name", testName);
            capabilities.SetCapability("browserTimeout", "120");

            if (useProxy)
            {
                var proxy = new Proxy();
                proxy.HttpProxy = TestConfiguration.Instance.ProxyAddress;
                capabilities.SetCapability(CapabilityType.Proxy, proxy);
            }

            return new ScreenshotRemoteWebDriver(new Uri(TestConfiguration.Instance.SeleniumServerHubUrl), capabilities);
        }

        public Screenshot GetScreenshot()
        {
            Response screenshotResponse = this.Execute(DriverCommand.Screenshot, null);
            string base64 = screenshotResponse.Value.ToString();
            return new Screenshot(base64);
        }
    }
}
