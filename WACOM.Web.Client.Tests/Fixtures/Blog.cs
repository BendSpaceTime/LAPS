namespace Azure.Automation.Fixtures
{
    using Azure.Automation.Helpers;
    using Azure.Automation.Selenium.Extensions;
    using Azure.Automation.WindowsAzurePortal;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using System;
    using System.Collections.ObjectModel;
    using System.Net;

    [TestClass]
    public class Blog : BaseFixture
    {
        [TestMethod]
        [TestCategory(Categories.Blog)]
        [Description("Verify Azure Blog loads")]
        public void LoadAzureBlog()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                //Step 1: Load Blog page    
                CommonSeleniumSteps.NavigateToURL(driver, "/blog");

                //Step 2:  Verify page loads
                Assert.AreEqual("Blog", driver.FindElement(By.ClassName("blog")).FindElement(By.CssSelector("a[class='trigger current']")).Text, "Page Header does not say Blog");
            });
        }

        [TestMethod]
        [TestCategory(Categories.Blog)]
        [Description("Verify Azure Blog RSS Feed works")]
        public void VerifyAzureBlogRSSFeed()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                //Step 1: Load Blog page    
                CommonSeleniumSteps.NavigateToURL(driver, "/blog");

                //Step 2: Check for Subscribe Button
                IWebElement subscribeButton = driver.FindElement(By.LinkText("Subscribe"));

                string expectedRSSUrl = Azure.Automation.Helpers.TestConfiguration.Instance.EnvironmentUrl + "/blog/feed/";
                Assert.AreEqual(expectedRSSUrl, subscribeButton.GetAttribute("href"));
                Assert.AreEqual("btn-subscribe button small", subscribeButton.GetAttribute("class"));

                //Step 3: Verify URL is available
                Assert.AreEqual(HttpStatusCode.OK.ToString(), CommonSeleniumSteps.GetHTTPStatusCode(expectedRSSUrl));
            });
        }

        [TestMethod]
        [TestCategory(Categories.Blog)]
        [Description("Verify user can search for term “azure” in blog and results are returned")]
        public void CanSearchInBlog()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                ReadOnlyCollection<IWebElement> searchResultCollection;
                string keyWords = "azure";
                int selectedItemIndex;
                string articleId;

                Logger.Instance.WriteLine("Navigate to the blog page");
                CommonSeleniumSteps.NavigateToURL(driver, "/blog/");
                IWebElement searchBox = driver.FindElement(By.CssSelector("input[id='s']"));
                IWebElement searchButton = driver.FindElement(By.CssSelector("input[id='searchsubmit']"));

                Logger.Instance.WriteLine("Input keyword and search");
                searchBox.SendKeys(keyWords);
                driver.WaitForURLChange(() => { searchButton.Click(); });
                Assert.IsTrue(driver.Url.Contains("/blog/?s="+keyWords),"Result page's Url doesn't contain '/blog/?s='");

                Logger.Instance.WriteLine("Verify result page did list some post items");
                searchResultCollection = driver.FindElements(By.CssSelector("div article[role='article']"));
                Assert.IsTrue(searchResultCollection.Count != 0, "Did not return any post items on result page with the given keyword");

                Logger.Instance.WriteLine("Verify Blog item points to a blog article");
                selectedItemIndex = new Random().Next(0, searchResultCollection.Count);
                articleId = searchResultCollection[selectedItemIndex].GetAttribute("id");
                driver.WaitForURLChange(() => { searchResultCollection[selectedItemIndex].FindElement(By.CssSelector("div div header h2 a")).Click(); });
                Assert.AreEqual(articleId, driver.FindElement(By.CssSelector("article")).GetAttribute("id"),"Article id is not same in result page and item detail page");
            });
        }

        [TestMethod]
        [TestCategory(Categories.Blog)]
        [Description("Verified there’s a featured blog post and there’s an article/link there…section isn’t empty")]
        public void FeaturedBlogExists()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("Navigate to blog page");
                CommonSeleniumSteps.NavigateToURL(driver, "/blog/");

                Logger.Instance.WriteLine("Verify Featured blog exists in blog page");
                ReadOnlyCollection<IWebElement> featuredPost = driver.FindElements(By.ClassName("feature-article"));
                Assert.AreEqual(featuredPost.Count, 1, "No featured post was found");
                Assert.AreEqual("Featured", featuredPost[0].FindElement(By.CssSelector("article header h3")).Text, "The queried post was not featured blog");
                string postId = featuredPost[0].FindElement(By.CssSelector("article")).GetAttribute("id");

                Logger.Instance.WriteLine("Verify Featured blog do point to an article");
                driver.WaitForURLChange(() => { featuredPost[0].FindElement(By.CssSelector("article header h1 a")).Click(); });
                Assert.AreEqual(postId, driver.FindElement(By.CssSelector("div article")).GetAttribute("id"), "Article id is not same in blog and blog detail page");
            });
        }

        [TestMethod]
        [TestCategory(Categories.Blog)]
        [Description("Verify that all blog images on the blog home page can be loaded successfully")]
        public void NoMissingBlogImages()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                ReadOnlyCollection<IWebElement> postItemWithImg = null;

                Logger.Instance.WriteLine("STEP 1: Navigate to blog page");
                CommonSeleniumSteps.NavigateToURL(driver, "/blog/");
                
                Logger.Instance.WriteLine("STEP 2: Find all img urls");
                postItemWithImg = driver.FindElements(By.CssSelector("img[class='attachment-thumbnail wp-post-image']"));
                Assert.AreNotEqual(0, postItemWithImg.Count, "Couldn't find any blog image on the blog home page");
                
                Logger.Instance.WriteLine("STEP 3: Verify each image can be loaded successfully");
                foreach (IWebElement img in postItemWithImg)
                    Assert.AreEqual(HttpStatusCode.OK.ToString(), CommonSeleniumSteps.GetHTTPStatusCode(img.GetAttribute("src")), "This img can not be loaded:" + img.GetAttribute("src"));
            });
        }
    }
}
