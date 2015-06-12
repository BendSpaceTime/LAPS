namespace Azure.Automation.Selenium
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UnsupportedBrowserAttributeHelper
    {
        public static IEnumerable<WebDriverTargetBrowser> GetUnsupportedBrowsers(Type testClassType, string testName)
        {
            var attributes = (UnsupportedBrowserAttribute[])Attribute.GetCustomAttributes(testClassType.GetMethod(testName), typeof(UnsupportedBrowserAttribute));

            return attributes.Select(attribute => attribute.WebDriverTargetBrowser);
        }
    }
}
