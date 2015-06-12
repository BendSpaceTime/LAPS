namespace Azure.Automation.Fixtures
{
    using Azure.Automation.WindowsAzurePortal;
    using Azure.Automation.Helpers;
    using Azure.Automation.Selenium.Extensions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Selenium.Utilities;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using System.Net;

    [TestClass]
    public class SearchSite : BaseFixture
    {
        [TestMethod]
        [TestCategory(Categories.P0)]
        [Description("Perform a search across site from homepage")]
        public void PerformSiteSearchFromHomepage()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                // STEP 1: Navigate to Homepage
                CommonSeleniumSteps.NavigateToHomepage(driver);

                // STEP 2: Find Search box in header
                IWebElement searchInput = driver.FindElement(By.Id("MainSearchBox"));

                //STEP 3: Type in search term "Documentation" and execute search
                driver.WaitForURLChange(() => searchInput.SendKeys("Documentation" + Keys.Return));

                //STEP 3: Verify page of search results appear
                Assert.IsTrue(driver.Title.Contains("Azure Search"), "Page title is not search: " + driver.Title);

                //STEP 4: Verify search results are relevant
                driver.WaitUntil(() => driver.FindElement(By.XPath("//div[@class='wa-searchResult']/a[text()='azure.microsoft.com/en-us/documentation']")), "No search result for Documentation page");
            });
        }

        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Verify Pagination and Navigation for Search Results")]
        public void SearchResultPagination()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                // STEP 1: Navigate to Search Results for VM
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/searchresults/?query=virtual+machine");

                // STEP 2: Verify page of search results appear
                Assert.IsTrue(driver.Title.Contains("Azure Search"));

                //STEP 3: Find pagination control and navigate to page 2
                IWebElement paginationControl = driver.FindElement(By.ClassName("wa-pagination"));
                driver.WaitForURLChange(() => paginationControl.FindElement(By.LinkText("2")).Click());

                //STEP 4: Verify page for search results appear
                Assert.IsTrue(driver.Title.Contains("Azure Search"));

                //STEP 5: Find pagination control and navigate back to page 1
                paginationControl = driver.FindElement(By.ClassName("wa-pagination"));
                driver.WaitForURLChange(() => paginationControl.FindElement(By.LinkText("1")).Click());

                //STEP 6: Verify page for search results appear
                Assert.IsTrue(driver.Title.Contains("Azure Search"), "Page title is not search: " + driver.Title);
            });
        }
    }
}


