namespace Azure.Automation.Selenium
{
    using System;
    using System.Collections.Generic;
    using OpenQA.Selenium.Remote;

    public class WebDriverTargetBrowser
    {
        private Dictionary<string, string> settings = new Dictionary<string, string>();

        private WebDriverTargetBrowser()
        {
        }

        public string Description
        {
            get
            {
                if (this.IsMobile)
                {
                    return string.Format("Mobile Browser: {0} - OS: {1} - Device: {2}", this.Browser, this.OperatingSystem, !string.IsNullOrEmpty(this.Device) ? this.Device : "Latest");
                }
                else
                {
                    return string.Format("Desktop Browser: {0} - Version: {1} - OS: {2} - OS Version: {3}", this.Browser, !string.IsNullOrEmpty(this.BrowserVersion) ? this.BrowserVersion : "Latest", this.OperatingSystem, this.OperatingSystemVersion);
                }
            }
        }

        public string Profile { get; private set; }

        public string Browser { get; private set; }

        public string BrowserVersion { get; private set; }

        public string OperatingSystem { get; private set; }

        public string OperatingSystemVersion { get; private set; }

        public string Device { get; private set; }

        public bool IsMobile { get; private set; }

        public static WebDriverTargetBrowser CreateTargetBrowser(string browser, string browserVersion, string operatingSystem, string operatingSystemVersion, string device, bool isMobile)
        {
            if (isMobile && !string.IsNullOrEmpty(operatingSystemVersion))
            {
                throw new ArgumentException("You cannot specify an operating system version on a Mobile Browser");
            }

            if (isMobile && !string.IsNullOrEmpty(browserVersion))
            {
                throw new ArgumentException("You cannot specify a browser version on a Mobile Browser");
            }

            if (!isMobile)
            {
                return WebDriverTargetBrowser.CreateDesktopTargetBrowser(browser, browserVersion, operatingSystem, operatingSystemVersion);
            }
            else
            {
                return WebDriverTargetBrowser.CreateMobileTargetBrowser(browser, operatingSystem, device);
            }
        }

        public static WebDriverTargetBrowser CreateDesktopTargetBrowser(string browser, string browserVersion, string operatingSystem, string operatingSystemVersion, string profile = "")
        {
            var targetBrowser = new WebDriverTargetBrowser();
            targetBrowser.settings.Add("browser", browser);
            targetBrowser.settings.Add("browserName", browser.ToLower());
            targetBrowser.settings.Add("browser_version", browserVersion);
            targetBrowser.settings.Add("os", operatingSystem);
            targetBrowser.settings.Add("os_version", operatingSystemVersion);

            targetBrowser.Profile = profile;
            targetBrowser.Browser = browser;
            targetBrowser.BrowserVersion = browserVersion;
            targetBrowser.OperatingSystem = operatingSystem;
            targetBrowser.OperatingSystemVersion = operatingSystemVersion;
            targetBrowser.Device = string.Empty;
            targetBrowser.IsMobile = false;

            return targetBrowser;
        }

        public static WebDriverTargetBrowser CreateMobileTargetBrowser(string browser, string os, string device, string profile = "")
        {
            var targetBrowser = new WebDriverTargetBrowser();
            targetBrowser.settings.Add("browser", browser);
            targetBrowser.settings.Add("os", os);
            targetBrowser.settings.Add("device", device);

            targetBrowser.Profile = profile;
            targetBrowser.Browser = browser;
            targetBrowser.Device = device;
            targetBrowser.OperatingSystem = os;
            targetBrowser.OperatingSystemVersion = string.Empty;
            targetBrowser.IsMobile = true;

            return targetBrowser;
        }

        public override bool Equals(object obj)
        {
            var otherBrowser = (WebDriverTargetBrowser)obj;

            return this.IsMobile == otherBrowser.IsMobile &&
                this.Browser.Equals(otherBrowser.Browser, StringComparison.InvariantCultureIgnoreCase) &&
                this.BrowserVersion.Equals(otherBrowser.BrowserVersion, StringComparison.InvariantCultureIgnoreCase) &&
                this.OperatingSystem.Equals(otherBrowser.OperatingSystem, StringComparison.InvariantCultureIgnoreCase) &&
                this.OperatingSystemVersion.Equals(otherBrowser.OperatingSystemVersion, StringComparison.InvariantCultureIgnoreCase) &&
                this.Device.Equals(otherBrowser.Device, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void SetCapabilities(DesiredCapabilities capabilities)
        {
            foreach (var keyValue in this.settings)
            {
                capabilities.SetCapability(keyValue.Key, keyValue.Value);
            }
        }
    }
}
