namespace WACOM.Web.Client.Tests.Fixtures
{
    using Azure.Automation;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TaggingFixture : BaseTaggingFixture
    {
        [TestMethod]
        [TestCategory(Categories.Tagging)]
        [Description("Tagging test for /en-us/campaigns/azure-cloud-that-can-sql-server/")]
        public void Azure_Sql_EnUs_TaggingTest()
        {
            var assertions = TaggingAssertions.Create(
                new TaggingAssertion
                {
                    ElementDisplayName = "Link - Service - SharePoint on IaaS",
                    ElementSelector = "#content > section.wa-section.wa-section-light > div.wa-content.wa-content-3up > div:nth-child(1) > ul > li > a",
                    ExpectedTag = "mrtocm_FY141COMMITIAZSQLInfraCON_1"
                },
                new TaggingAssertion
                {
                    ElementDisplayName = "Button - Try for free",
                    ElementSelector = "#content > section.wa-section.wa-sectionGetStarted > div > div:nth-child(2) > a",
                    ExpectedTag = "mrtocm_FY141COMMITIAZSQLTrialCON_1"
                });

            this.RunTaggingTests(@"/en-us/campaigns/azure-cloud-that-can-sql-server/", assertions);
        }

        [TestMethod]
        [Ignore]
        [TestCategory(Categories.Tagging)]
        [Description("Tagging test for /ru-ru/solutions/dev-test/")]
        public void Solutions_DevTest_RuRu_TaggingTest()
        {
            var assertions = TaggingAssertions.Create(
                new TaggingAssertion
                        {
                            ElementDisplayName = "Button - Try it now",
                            ElementSelector = "#RunwayMasterForm > div > div.hero > div > p:nth-child(4) > a",
                            ExpectedTag = "mrrruc_FY14STBCampaignjanjun14poprobuitecon_1"
                        },
                new TaggingAssertion
                        {
                            ElementDisplayName = "Link - Learn More",
                            ElementSelector = "#RunwayMasterForm > div > div.benefits.section.active > div.s2 > p > a",
                            ExpectedTag = "mrrruc_FY14STBCampaignjanjun14podrobneecon_1"
                        },
                new TaggingAssertion
                        {
                            ElementDisplayName = "Link - MSDN subscription",
                            ElementSelector = "#RunwayMasterForm > div > div.benefits.section.active > div.s3 > p > a",
                            ExpectedTag = "mrrruc_FY14STBCampaignjanjun14podpiskacon_1"
                        },
                new TaggingAssertion
                        {
                            ElementDisplayName = "Link - Service - Virtual Machines",
                            ElementSelector = "#RunwayMasterForm > div > div.services > ul > li:nth-child(1) > a",
                            ExpectedTag = "mrrruc_FY14STBCampaignjanjun14mashinycon_1"
                        },
                new TaggingAssertion
                        {
                            ElementDisplayName = "Link - Service - Virtual Network",
                            ElementSelector = "#RunwayMasterForm > div > div.services > ul > li:nth-child(4) > a",
                            ExpectedTag = "mrrruc_FY14STBCampaignjanjun14setcon_1"
                        },
                new TaggingAssertion
                        {
                            ElementDisplayName = "Link - Service - Storage",
                            ElementSelector = "#RunwayMasterForm > div > div.services > ul > li:nth-child(2) > a",
                            ExpectedTag = "mrrruc_FY14STBCampaignjanjun14harnilishecon_1"
                        },
                new TaggingAssertion
                        {
                            ElementDisplayName = "Link - Service - Visual Studio Online",
                            ElementSelector = "#RunwayMasterForm > div > div.services > ul > li:nth-child(3) > a",
                            ExpectedTag = "mrrruc_FY14STBCampaignjanjun14studiocon_1"
                        },
                new TaggingAssertion
                        {
                            ElementDisplayName = "Link - Light Blue Band - Start Today",
                            ElementSelector = "div.wa-content-freeTrialCta div.wa-spacer:nth-child(1) a.wa-arrowLinkLarge.wa-arrowLinkLarge-light",
                            ExpectedTag = "mrrruc_FY14STBCampaignjanjun14Nachnitecon_1"
                        },
                new TaggingAssertion
                        {
                            ElementDisplayName = "Link - Light Blue Band - Activate Now",
                            ElementSelector = "div.wa-content-freeTrialCta div.wa-spacer:nth-child(2) a.wa-arrowLinkLarge.wa-arrowLinkLarge-light",
                            ExpectedTag = "mrrruc_FY14STBCampaignjanjun14aktivirovatcon_1"
                        },
                new TaggingAssertion
                        {
                            ElementDisplayName = "Link - Light Blue Band - Learn More",
                            ElementSelector = "div.wa-content-freeTrialCta div.wa-spacer:nth-child(3) a.wa-arrowLinkLarge.wa-arrowLinkLarge-light",
                            ExpectedTag = "mrrruc_FY14STBCampaignjanjun14planycon_1"
                        });

            this.RunTaggingTests(@"/ru-ru/solutions/dev-test/", assertions);
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            BaseTaggingFixture.BaseClassInitialize();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            BaseTaggingFixture.BaseClassCleanup();
        }
    }
}
