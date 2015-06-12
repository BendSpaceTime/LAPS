namespace Azure.Automation.Fixtures
{
    using Azure.Automation.Helpers;
    using Azure.Automation.WindowsAzurePortal;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using System.Collections.Generic;
    using System.Net;


    [TestClass]
    public class ErrorChecks : BaseFixture
    {
        public string[] topPages =
        {
            @"/en-us/",
            @"/en-us/pricing/free-trial/",
            @"/en-us/account/",
            @"/en-us/pricing/calculator/",
            @"/en-us/documentation/",
            @"/en-us/downloads/",
            @"/en-us/pricing/details/virtual-machines/",
            @"/en-us/support/service-dashboard/",
            @"/en-us/pricing/calculator/?scenario=virtual-machines",
            @"/en-us/develop/net/?WT.mc_id=DOTNET_SDK_23",
            @"/ja-jp/",
            @"/ru-ru/solutions/dev-test/",
            @"/en-us/pricing/purchase-options/",
            @"/en-us/pricing/details/web-sites/",
            @"/en-us/pricing/calculator/?scenario=full",
            @"/en-us/support/options/",
            @"/en-us/pricing/calculator/?scenario=data-management",
            @"/pt-br/pricing/free-trial/?l=pt-br&WT.mc_id=AzureBg_Brazil_SEM",
            @"/en-us/pricing/details/sql-database/",
            @"/es-es/",
            @"/en-us/documentation/articles/web-sites-dotnet-get-started/",
            @"/en-us/pricing/calculator/?scenario=cloud",
            @"/en-us/overview/what-is-azure/",
            @"/en-us/develop/net/",
            @"/en-us/pricing/overview/",
            @"/fr-fr/",
            @"/ru-ru/solutions/dev-test/",
            @"/en-us/marketplace/virtual-machines",
            @"/en-us/downloads/dotnet-sdk-23/?WT.mc_id=DOTNET_SDK_23"
        };
        public string[] minorPages =
        {
            @"/",
            @"/account/",
            @"/documentation/infographics/",
            @"/documentation/infographics/azure/",
            @"/documentation/scripts/",
            @"/documentation/videos/",
            @"/documentation/videos/azure-friday/",
            @"/documentation/videos/index/",
            @"/downloads/",
            @"/marketplace/",
            @"/marketplace/active-directory/",
            @"/marketplace/application-services/",
            @"/overview/preview-portal/",
            @"/overview/what-is-azure/",
            @"/pricing/calculator/",
            @"/pricing/details/api-management/",
            @"/pricing/details/expressroute/",
            @"/pricing/details/mobile-services/",
            @"/pricing/details/remoteapp/",
            @"/pricing/details/virtual-machines/",
            @"/pricing/free-trial/",
            @"/pricing/overview/",
            @"/regions/",
            @"/searchresults/",
            @"/services/",
            @"/services/api-management/",
            @"/services/expressroute/",
            @"/services/hdinsight/",
            @"/services/mobile-services/",
            @"/services/remoteapp/",
            @"/solutions/infrastructure/",
            @"/solutions/mobile/",
            @"/solutions/web/",
            @"/support/legal/",
            @"/support/legal/preview-supplemental-terms/",
            //@"/support/legal/privacy-statement/",
            //@"/support/legal/services-terms/",
            @"/support/legal/sla/",
            @"/support/legal/subscription-agreement/",
            @"/support/legal/website-terms-of-use/",
            @"/support/trust-center/",
            @"/support/trust-center/compliance/",
            @"/support/trust-center/faq/",
            @"/support/trust-center/privacy/",
            @"/support/trust-center/resources/",
            @"/support/trust-center/security/",
            @"/support/understand-your-bill/",
            @"/updates/"
        };

        public string[] allDeploymentsUrl = 
        {
            "http://acom-prod-uswest-01.azurewebsites.net/",
            "http://acom-prod-useast-01.azurewebsites.net/",
            "http://acom-prod-usnorthcentral-01.azurewebsites.net/",
            "http://acom-prod-europenorth-01.azurewebsites.net/",
            "http://acom-prod-europewest-01.azurewebsites.net/",
            "http://acom-prod-japaneast-01.azurewebsites.net/",
            "http://acom-prod-japanwest-01.azurewebsites.net/",
            "http://acom-prod-asiaeast-01.azurewebsites.net/",
            "http://acom-prod-asiasoutheast-01.azurewebsites.net/",
            "http://acom-prod-brazilsouth-01.azurewebsites.net/",
        };

        public string[] redirectMinorPages = 
        {
            @"/support/legal/privacy-statement/",
            @"/support/legal/services-terms/"
        };

        [TestMethod]
        [TestCategory(Categories.P1)]
        [Description("Load the top 25 pages and verify CSS loaded")]
        public void LoadTop25PagesCheckCSS()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                foreach (string page in topPages)
                {
                    // STEP 1: Navigate to URL
                    CommonSeleniumSteps.NavigateToURL(driver, page);


                    // STEP 2: Check CoreCSS styles
                    CommonSeleniumSteps.CheckCoreCssStylesAreApplied(driver);
                }
            });
        }


        [TestMethod]
        [TestCategory(Categories.P0)]
        [Description("Check the HTTP code for top 25 pages")]
        public void Top25PagesCheckHTTPCode()
        {
            foreach (string page in topPages)
            {
                try
                {
                    string fullURLPath = Azure.Automation.Helpers.TestConfiguration.Instance.EnvironmentUrl + page;

                    // Creates an HttpWebRequest for the specified URL.
                    HttpWebRequest pageRequest = (HttpWebRequest)WebRequest.Create(fullURLPath);

                    // Sends the HttpWebRequest and waits for a response.
                    HttpWebResponse pageResponse = (HttpWebResponse)pageRequest.GetResponse();

                    Assert.AreEqual(pageResponse.StatusCode, HttpStatusCode.OK, "HTTP code not 200.  Actual: " + pageResponse.StatusCode.ToString());


                    // Releases the resources of the response.
                    pageResponse.Close();
                }
                catch (System.Exception e)
                {
                    Assert.Fail("Exception Raised for page: {0}. The following error occured : {1}", page, e.Message);
                }
            }

        }

        [TestMethod]
        [TestCategory(Categories.Prod)]
        [Description("Verify HTTP 404 and Alien Page")]
        public void Verify404PagesShowAlienPage()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                // STEP 1: Get HTTP 404
                string fullURLPath = Azure.Automation.Helpers.TestConfiguration.Instance.EnvironmentUrl + "/blahblah";

                // Creates an HttpWebRequest for the specified URL.
                HttpWebRequest pageRequest = (HttpWebRequest)WebRequest.Create(fullURLPath);
                HttpWebResponse pageResponse = null;
                bool exceptionFired = false;
                try
                {
                    // Sends the HttpWebRequest and waits for a response.
                    pageResponse = (HttpWebResponse)pageRequest.GetResponse();
                    pageResponse.Close();
                }
                catch (WebException ex)
                {
                    exceptionFired = true;
                    Assert.IsTrue(ex.ToString().Contains("404"));
                }
                Assert.IsTrue(exceptionFired, "No 404 exception caught");

                // STEP 2: Navigate to URL
                CommonSeleniumSteps.NavigateToURL(driver, "/blahblah");

                // STEP 3: Check for visual 404 page/alien
                IWebElement errorDiv = driver.FindElement(By.CssSelector("h1[class='wa-headingSuper']"));
                Assert.AreEqual("404 Page not found", errorDiv.Text);

            });
        }

        [TestMethod]
        [TestCategory(Categories.P2)]
        [Description("Load the low pri pages and verify CSS loaded")]
        public void LoadMinorPagesCheckCSS()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                foreach (string page in minorPages)
                {
                    // STEP 1: Navigate to URL
                    string fullUrl = @"/en-us" + page;
                    CommonSeleniumSteps.NavigateToURL(driver, fullUrl);


                    // STEP 2: Check CoreCSS styles
                    CommonSeleniumSteps.CheckCoreCssStylesAreApplied(driver);
                }
            });
        }


        [TestMethod]
        [TestCategory(Categories.P0)]
        [Description("Check the HTTP code for low pri pages in english")]
        public void MinorPagesCheckHTTPCodeUnderEnglish()
        {

            foreach (string page in minorPages)
            {
               string fullURLPath = Azure.Automation.Helpers.TestConfiguration.Instance.EnvironmentUrl + @"/en-us" + page;
               Assert.AreEqual(HttpStatusCode.OK.ToString(), CommonSeleniumSteps.GetHTTPStatusCode(fullURLPath), "HTTP code not 200");
            }

        }


        [TestMethod]
        [TestCategory(Categories.Blog)]
        [Description("Check the blog for each deployment returns a good status code.")]
        public void BlogStatusForAllDeployments()
        {
            foreach (string url in allDeploymentsUrl)
            {
                string fullUrl = url + @"blog";
                Assert.AreEqual(HttpStatusCode.OK.ToString(),CommonSeleniumSteps.GetHTTPStatusCode(fullUrl), "HTTP code not 200");
            }
        }


        [TestMethod]
        [TestCategory(Categories.P2)]
        [Description("Check the HTTP code for low pri pages in all languages")]
        public void MinorPagesCheckHTTPCodeUnderAllLanguages()
        {
            this.CreateSeleniumTestContext().Run(driver =>
                {
                    Logger.Instance.WriteLine("STEP 1: Navigate to Azure home page");
                    CommonSeleniumSteps.NavigateToHomepage(driver);

                    Logger.Instance.WriteLine("STEP 2: Get all languages");
                    List<string> allLanguages = LocCurrencyHelpers.GetAllLangCultures();

                    Logger.Instance.WriteLine("STEP 3: Verify each page under all languages return a good status");
                    foreach (string page in minorPages)
                    {
                        foreach (string lan in allLanguages)
                        {
                            string fullUrl = Azure.Automation.Helpers.TestConfiguration.Instance.EnvironmentUrl + "/" + lan + page;
                            Assert.AreEqual(HttpStatusCode.OK.ToString(), CommonSeleniumSteps.GetHTTPStatusCode(fullUrl), "HTTP code not 200");
                        }
                    }
              });
        }

        [TestMethod]
        [Ignore]
        [TestCategory(Categories.Prod)]
        [Description("Check the HTTP code for all urls in robots.txt file")]
        public void VerifyAllUrlsInRobots()
        {
            List<string> urls = ExtractUrlsFromRobots.getAllURL();
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            string status;
            foreach (string url in urls)
            {
                status = CommonSeleniumSteps.GetHTTPStatusCode(url);
                if (status != HttpStatusCode.OK.ToString())
                {
                    if (result.ContainsKey(status)) result[status].Add(url);
                    else result.Add(status, new List<string>() { url });
                }
            }

            if (result.Count != 0)
            {
                foreach (string key in result.Keys)
                {
                    Logger.Instance.WriteSeparatorLine();
                    Logger.Instance.WriteLine("These urls fails as " + key);
                    foreach (string failUrl in result[key])
                        Logger.Instance.WriteLine(failUrl);
                }

                Assert.Fail("Above Urls did not return an OK status");
            }
        }

        [TestMethod]
        [TestCategory(Categories.Prod)]
        [Description("Check the HTTP code for the sitemap and disallow dir in robots.txt file")]
        public void VerifyHttpStatusForSiteMapAndDisallowDir()
        {
            List<string> siteMap = ExtractUrlsFromRobots.getSiteMap();
            List<string> disAllowDir = ExtractUrlsFromRobots.getDisAllowDir();

            Logger.Instance.WriteLine("Check http code for all the sitemaps");
            foreach (string mapUrl in siteMap)
                Assert.AreEqual(HttpStatusCode.OK.ToString(), CommonSeleniumSteps.GetHTTPStatusCode(mapUrl),"This url is break:"+mapUrl);

            Logger.Instance.WriteLine("Check http code for all the disallow directory");
            foreach (string dir in disAllowDir)
            {
                string fullPath = Azure.Automation.Helpers.TestConfiguration.Instance.EnvironmentUrl + dir;
                Assert.AreEqual(HttpStatusCode.OK.ToString(), CommonSeleniumSteps.GetHTTPStatusCode(fullPath),"This url is break:"+fullPath);
            }

            
        }

        [TestMethod]
        [TestCategory(Categories.P0)]
        [Description("Check the HTTP code for the redirect minorPages")]
        public void CheckHttpStatusForRedirectMinorPages()
        {
            string destinationURL;
            foreach (string url in redirectMinorPages)
            {
                string fullUrl = Azure.Automation.Helpers.TestConfiguration.Instance.EnvironmentUrl + @"/en-us" + url;
                Assert.AreEqual(HttpStatusCode.MovedPermanently.ToString(), CommonSeleniumSteps.CheckURLRedirect(fullUrl, out destinationURL));
                Assert.AreEqual(HttpStatusCode.OK.ToString(), CommonSeleniumSteps.GetHTTPStatusCode(destinationURL));
            }
        }
    }
}
