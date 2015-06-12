namespace Azure.Automation.WindowsAzurePortal
{
    using System.Linq;
    using System.Collections.Generic;
    using Azure.Automation.Helpers;
    using Azure.Automation.Selenium.Extensions;
    using Microsoft.Selenium.Utilities;
    using OpenQA.Selenium;
    using System.Net;
    using Azure.Automation.Helpers.JsonData;
    using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

    public class PricingDetailsHelper
    {
        public static Dictionary<string, string> GetAllURLsFromPricingDetail()
        {
            string domain = TestConfiguration.Instance.EnvironmentUrl + "/en-us";
            Dictionary<string, string> result = new Dictionary<string, string>();
            JPricingDetails pricingDetails = JsonHelper.ExtractDataFromJson<JPricingDetails>("/api/cache/services/");

            foreach (SServices data in pricingDetails.Data.Services)
            {
                if (data.PricingUrl != null)
                    result.Add(data.DisplayName, domain + data.PricingUrl);
            }
            return result;
        }

        public static Dictionary<string, string> GetAllRelativeURLsFromPricingDetail()
        {
            var result = new Dictionary<string, string>();
            var pricingDetails = JsonHelper.ExtractDataFromJson<JPricingDetails>("/api/cache/services/");

            foreach (SServices data in pricingDetails.Data.Services)
            {
                if (!string.IsNullOrEmpty(data.PricingUrl))
                {
                    result.Add(data.DisplayName, data.PricingUrl);
                }
            }
            return result;
        }
    }
}
