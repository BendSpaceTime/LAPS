namespace Azure.Automation.Fixtures
{
    using Azure.Automation.Selenium;
    using Azure.Automation.WindowsAzurePortal;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DocumentationPageFixture : BaseFixture
    {
        [TestMethod]
        [TestCategory(Categories.Documentation)]
        [Description("Opens documentation and checks disqus comment board is available")]
        public void OpenDocsPageAndCheckDisqus()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                // STEP 1: Navigate to Doc page with Discus (VM tutorial)
                CommonSeleniumSteps.NavigateToURL(driver, "/en-us/documentation/articles/virtual-machines-windows-tutorial/");

                // STEP 2: Check Disqus comment board is available
                CommonSeleniumSteps.CheckDisqusCommentBoardIsAvailable(driver);
            });
        }

        [TestMethod]
        [TestCategory(Categories.Documentation)]
        [Description("Load the home page, navigates to documentation section via menu")]
        public void NavigationToDocumentationSection()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                // STEP 1: Navigate to homepage
                CommonSeleniumSteps.NavigateToHomepage(driver);

                // STEP 2: Navigate to documentation section
                CommonSeleniumSteps.ClickAndNavigateToSection(driver, "documentation");

                // STEP 3: Navigate to first available documentation index page
                DocumentationPageSeleniumSteps.ClickAndNavigateToFirstAvailableDocumentationByServiceIndexPage(driver);
            });
        }

        [TestMethod]
        [TestCategory(Categories.Documentation)]
        [Description("Checks the TOC links to see if page scrolls")]
        [UnsupportedBrowser("Safari", operatingSystem: "OS X", operatingSystemVersion: "Mavericks")]
        public void CheckDocsPageTOCLinks()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                // STEP 1: Navigate to Doc page with TOC link (VM tutorial)
                CommonSeleniumSteps.NavigateToURL(driver, "/en-us/documentation/articles/virtual-machines-windows-tutorial/");

                // STEP 2: Find last TOC link, click it and ensure title element is displayed in viewport
                DocumentationPageSeleniumSteps.ClickDocumentationLastTocLinkAndCheckTitleIsDisplayed(driver);
            });
        }
    }
}
