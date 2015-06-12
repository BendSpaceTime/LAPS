namespace Azure.Automation.Fixtures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Azure.Automation.Selenium;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class BaseFixture
    {
        private const string BuildName = "Validation Tests";

        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get { return this.testContextInstance; }
            set { this.testContextInstance = value; }
        }

        protected SeleniumTestContext CreateSeleniumTestContext(bool useProxy = false)
        {
            var unsupportedBrowsers = UnsupportedBrowserAttributeHelper.GetUnsupportedBrowsers(
                                                Type.GetType(this.TestContext.FullyQualifiedTestClassName),
                                                this.TestContext.TestName);

            var context = new SeleniumTestContext(this.TestContext.TestName, BuildName, unsupportedBrowsers);
            context.UseProxy = useProxy;

            return context;
        }
    }
}
