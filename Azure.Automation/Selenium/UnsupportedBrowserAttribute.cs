namespace Azure.Automation.Selenium
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class UnsupportedBrowserAttribute : Attribute
    {
        public UnsupportedBrowserAttribute(string browser, string browserVersion = "", string operatingSystem = "", string operatingSystemVersion = "", string device = "", bool isMobile = false)
        {
            this.WebDriverTargetBrowser = WebDriverTargetBrowser.CreateTargetBrowser(browser, browserVersion, operatingSystem, operatingSystemVersion, device, isMobile);
        }

        public WebDriverTargetBrowser WebDriverTargetBrowser { get; private set; }
    }
}
