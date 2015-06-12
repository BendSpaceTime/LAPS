namespace Azure.Automation.Fixtures
{
    using Azure.Automation.Helpers;
    using Azure.Automation.Helpers.JsonData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class CalculatorHelper
    {
        public static void AddNewVM(IWebDriver driver, string vmName)
        {
            IWebElement addNewVM = driver.FindElement(By.CssSelector("a[ng-click='calculator.addVirtualMachine($event)']"));
            addNewVM.Click();

            List<IWebElement> nameFeilds = driver.FindElements(By.CssSelector("input[ng-model='config.name']")).ToList();
            ReNameVM(nameFeilds[nameFeilds.Count - 1], vmName);
        }

        public static void ReNameVM(IWebElement nameFeild, string vmName)
        {
            nameFeild.Clear();
            nameFeild.SendKeys(vmName);
        }

        public static void ReMoveVM(IWebDriver driver, string vmName)
        {
            List<IWebElement> allVMs = driver.FindElements(By.XPath("//div[contains(@ng-repeat,'virtual-machine')]")).ToList();
            for (int i = 0; i < allVMs.Count; i++)
            {
                IWebElement nameF = allVMs[i].FindElement(By.CssSelector("div input"));
                if (nameF.GetAttribute("value") == vmName)
                {
                    driver.FindElements(By.CssSelector("a[class='close']"))[i].Click();
                    break;
                }
            }
        }

        public static void VerifyPageMatchsJson(IWebDriver driver, string pricingTier, string type, string region)
        {
            VMOffer[] offers = JsonHelper.ExtractDataFromJson<VMOffer[]>("/en-us/pricing/calculator/api/pricing/virtual-machines/offers/");
            driver.FindElement(By.CssSelector("div[ng-model='config.instance'] span[class='arrow']")).Click();

            IList<IWebElement> allVMs = driver.FindElements(By.CssSelector("tbody tr[class='data ng-scope']"));

            foreach (IWebElement element in allVMs)
            {
                IList<IWebElement> allInfo = element.FindElements(By.CssSelector("td"));
                string instanceSize = allInfo[0].FindElement(By.CssSelector("span")).GetAttribute("innerHTML");

                var res =
                    from p in offers
                    where p.Tier.ToLower() == pricingTier.ToLower() && instanceSize.ToLower() == p.InstanceSize && p.Type.ToLower() == type.ToLower()
                    select p;
                List<VMOffer> instances = res.ToList<VMOffer>();

                Assert.IsTrue(instances.Count == 1, "instances.Count == 1");

                Assert.IsTrue(String.Equals(allInfo[1].Text, instances[0].DiskType, StringComparison.OrdinalIgnoreCase), "Disk type does not match");
                Assert.IsTrue(String.Equals(allInfo[2].Text, instances[0].Cores + " cores", StringComparison.OrdinalIgnoreCase), "CPU cores does not match");
                Assert.IsTrue(String.Equals(allInfo[3].Text, instances[0].Ram + " GB RAM", StringComparison.OrdinalIgnoreCase), "RAM does not match");
                Assert.IsTrue(String.Equals(allInfo[4].Text, instances[0].Disk + " GB disk", StringComparison.OrdinalIgnoreCase), "Disk size does not match");
                decimal price = instances[0].Prices[region.Replace(' ', '-').ToLower()];
                decimal jPrice = Decimal.Parse(allInfo[5].Text.Substring(1));

                Assert.AreEqual(price, jPrice, "Price does not match");
            }
        }
    }
}
