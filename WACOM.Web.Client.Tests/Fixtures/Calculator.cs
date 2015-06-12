namespace Azure.Automation.Fixtures
{
    using Azure.Automation.Helpers;
    using Azure.Automation.WindowsAzurePortal;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class Calculator : BaseFixture
    {
        const string Europe_West = "Europe West";
        const string US_North_Central = "US North Central";
        const string Windows = "windows";
        const string Linux = "linux";
        const string PricingTier_Basic = "basic";
        const string PricingTier_Standard = "standard";
        const string VM_NAME_FEILD = "This sentence has 54 characters including white space!";
        string[] VMNAMES = { "VM1", "VM2", "VM3" };

        [TestMethod]
        [TestCategory(Categories.Calculator)]
        [Description("Verify gov regions do not display in “Region” drop down")]
        public void VerifyGovRegionDoNotDisplay()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to VM page.");
                CommonSeleniumSteps.NavigateToURL(driver, "/en-us/pricing/calculator/virtual-machines/");

                Logger.Instance.WriteLine("STEP 2: Verify GOV region did not display");
                IList<string> regionNames = driver.FindElements(By.CssSelector("div[xa-name='region'] option")).ToList().ConvertAll(item => item.Text);
                Assert.IsFalse(regionNames.Contains("US Gov Virginia"));
                Assert.IsFalse(regionNames.Contains("US Gov Iowa"));
            });
        }

        [TestMethod]
        [TestCategory(Categories.Calculator)]
        [Description("Verify “Region” selection enforces valid available instances in the “Instance Size” drop down for Windows VM")]
        public void VerifyBasicWindowsInstanceSize()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to VM page.");
                CommonSeleniumSteps.NavigateToURL(driver, "/en-us/pricing/calculator/virtual-machines/");

                new SelectElement(driver.FindElement(By.CssSelector("div[xa-model='config.region'] select"))).SelectByText(Europe_West);
                new SelectElement(driver.FindElement(By.CssSelector("div[xa-model='config.type'] select"))).SelectByText("Windows");
                new SelectElement(driver.FindElement(By.CssSelector("div[xa-model='config.tier'] select"))).SelectByText("Basic");

                CalculatorHelper.VerifyPageMatchsJson(driver, PricingTier_Basic, Windows, Europe_West);
            });
        }

        [TestMethod]
        [TestCategory(Categories.Calculator)]
        [Description("Verify “Region” selection enforces valid available instances in the “Instance Size” drop down for Windows VM")]
        public void VerifyStandardLinuxInstanceSize()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to VM page.");
                CommonSeleniumSteps.NavigateToURL(driver, "/en-us/pricing/calculator/virtual-machines/");

                new SelectElement(driver.FindElement(By.CssSelector("div[xa-model='config.region'] select"))).SelectByText(US_North_Central);
                new SelectElement(driver.FindElement(By.CssSelector("div[xa-model='config.type'] select"))).SelectByText("Linux");
                new SelectElement(driver.FindElement(By.CssSelector("div[xa-model='config.tier'] select"))).SelectByText("Standard");

                CalculatorHelper.VerifyPageMatchsJson(driver, PricingTier_Standard, Linux, US_North_Central);
            });
        }

        [TestMethod]
        [TestCategory(Categories.Calculator)]
        [Description("Verify VM count entry rules works good by editing on VM Calculator page.")]
        public void VMCountrEntryRulesByEditing()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                CommonSeleniumSteps.NavigateToURL(driver, "/en-us/pricing/calculator/virtual-machines/");

                Logger.Instance.WriteLine("STEP 1: Verify default VM count and valid entries");
                Assert.IsTrue(driver.FindElement(By.CssSelector("div[xa-model='config.count'] input")).GetAttribute("value") == "1");

                Logger.Instance.WriteLine("STEP 2: Valid entries are =>1 and =< 999");
                driver.FindElement(By.CssSelector("div[xa-model='config.count'] input")).SendKeys("0");
                Assert.IsTrue(driver.FindElement(By.CssSelector("div[xa-model='config.count'] input")).GetAttribute("value") == "10");

                driver.FindElement(By.CssSelector("div[xa-model='config.count'] input")).Clear();
                driver.FindElement(By.CssSelector("div[xa-model='config.count'] input")).SendKeys("-1");
                Assert.IsTrue(driver.FindElement(By.CssSelector("div[xa-model='config.count'] input")).GetAttribute("value") == "11");

                driver.FindElement(By.CssSelector("div[xa-model='config.count'] input")).Clear();
                driver.FindElement(By.CssSelector("div[xa-model='config.count'] input")).SendKeys("23");
                Assert.IsTrue(driver.FindElement(By.CssSelector("div[xa-model='config.count'] input")).GetAttribute("value") == "123");

                driver.FindElement(By.CssSelector("div[xa-model='config.count'] input")).SendKeys("10000");
                Assert.IsTrue(driver.FindElement(By.CssSelector("div[xa-model='config.count'] input")).GetAttribute("value") == "999");
            });
        }

        [TestMethod]
        [TestCategory(Categories.Calculator)]
        [Description("Verify number entry controls can be activated using the number entry, up/down arrow and mouse click")]
        public void VMCountrEntryRulesByUpAndDownArrow()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                int count = 0;
                int hour = 0;

                Logger.Instance.WriteLine("STEP 1: Navigate to VM page.");
                CommonSeleniumSteps.NavigateToURL(driver, "/en-us/pricing/calculator/virtual-machines/");
                IWebElement vmCount = driver.FindElement(By.CssSelector("div[xa-model='config.count'] input"));

                Logger.Instance.WriteLine("STEP 2: Verify VM count editbox accepts Arrow key.");
                vmCount.SendKeys(Keys.ArrowDown);
                Assert.IsTrue(vmCount.GetAttribute("value") == "1");

                for (; count < 50; count++)
                    vmCount.SendKeys(Keys.ArrowUp);
                Assert.IsTrue(vmCount.GetAttribute("value") == "51");

                vmCount.SendKeys("99");
                vmCount.SendKeys(Keys.ArrowUp);
                Assert.IsTrue(vmCount.GetAttribute("value") == "999");

                Logger.Instance.WriteLine("STEP 3: Verify VM hour editbox accepts Arrow key.");
                IWebElement vmHours = driver.FindElement(By.CssSelector("div[xa-model='config.hours'] input"));
                vmHours.SendKeys(Keys.ArrowUp);
                Assert.IsTrue(vmHours.GetAttribute("value") == "744");

                for (count = 0; count < 50; count++)
                    vmHours.SendKeys(Keys.ArrowDown);
                Assert.IsTrue(vmHours.GetAttribute("value") == "694");

                vmHours.Clear();
                vmHours.SendKeys(Keys.ArrowDown);
                Assert.IsTrue(vmHours.GetAttribute("value") == "1");

                Logger.Instance.WriteLine("STEP 4: Verify using mouse click to increase/decrease VM count");
                for (count = 0; count < 50; count++)
                    driver.FindElement(By.CssSelector("div[xa-model='config.count'] a[class='arrowDown']")).Click();
                Assert.IsTrue(vmCount.GetAttribute("value") == "949");

                for (; count > 0; count--)
                    driver.FindElement(By.CssSelector("div[xa-model='config.count'] a[class='arrowUp']")).Click();
                Assert.IsTrue(vmCount.GetAttribute("value") == "999");

                vmCount.Clear();
                Assert.IsTrue(vmCount.GetAttribute("value") == "1");

                Logger.Instance.WriteLine("STEP 5: Verify using mouse click to increase/decrease VM hours");

                for (hour = 0; hour < 50; hour++)
                    driver.FindElement(By.CssSelector("div[xa-model='config.hours'] a[class='arrowUp']")).Click();
                Assert.IsTrue(vmHours.GetAttribute("value") == "51");

                for (; hour > 0; hour--)
                    driver.FindElement(By.CssSelector("div[xa-model='config.hours'] a[class='arrowDown']")).Click();
                Assert.IsTrue(vmHours.GetAttribute("value") == "1");

                vmHours.SendKeys("999");
                Assert.IsTrue(vmHours.GetAttribute("value") == "744");
            });
        }

        [TestMethod]
        [TestCategory(Categories.Calculator)]
        [Description("Verify text limits in name field")]
        public void VerifyTextLimitsInNameField()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                CommonSeleniumSteps.NavigateToURL(driver, "/en-us/pricing/calculator/virtual-machines/");
                IWebElement nameFeild = driver.FindElement(By.CssSelector("input[ng-model='config.name']"));
                nameFeild.Clear();
                nameFeild.SendKeys(VM_NAME_FEILD);

                Assert.IsTrue(28 == nameFeild.GetAttribute("value").Length);
                Assert.IsTrue(nameFeild.GetAttribute("value") == driver.FindElement(By.XPath("//li[contains(@ng-repeat,'virtual-machine')]/a")).Text);
            });
        }

        [TestMethod]
        [TestCategory(Categories.Calculator)]
        [Description("Verify name field matches value and order in sidecart")]
        public void VerifyNameFeildMatchsSidecart()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                Logger.Instance.WriteLine("STEP 1: Navigate to VM page.");
                CommonSeleniumSteps.NavigateToURL(driver, @"/en-us/pricing/calculator/virtual-machines/");

                Logger.Instance.WriteLine("STEP 2: Modify first VM name to 'VM1'.");
                IWebElement nameFeild1 = driver.FindElement(By.CssSelector("input[ng-model='config.name']"));
                CalculatorHelper.ReNameVM(nameFeild1, VMNAMES[0]);
                Assert.IsTrue(VMNAMES[0] == driver.FindElement(By.CssSelector("input[ng-model='config.name']")).GetAttribute("value"));

                Logger.Instance.WriteLine("STEP 3: Add a new VM and name with 'VM2'.");
                CalculatorHelper.AddNewVM(driver, VMNAMES[1]);
                Assert.IsTrue(VMNAMES[1] == driver.FindElements(By.CssSelector("input[ng-model='config.name']"))[1].GetAttribute("value"));

                Logger.Instance.WriteLine("STEP 4: Add a new VM and name with 'VM3'.");
                CalculatorHelper.AddNewVM(driver, VMNAMES[2]);
                Assert.IsTrue(VMNAMES[2] == driver.FindElements(By.CssSelector("input[ng-model='config.name']"))[2].GetAttribute("value"));

                Logger.Instance.WriteLine("STEP 5: Verify name field matches value and order in sidecart");
                Assert.IsTrue(VMNAMES[0] == driver.FindElements(By.XPath("//li[contains(@ng-repeat,'virtual-machine')]/a"))[0].Text);
                Assert.IsTrue(VMNAMES[1] == driver.FindElements(By.XPath("//li[contains(@ng-repeat,'virtual-machine')]/a"))[1].Text);
                Assert.IsTrue(VMNAMES[2] == driver.FindElements(By.XPath("//li[contains(@ng-repeat,'virtual-machine')]/a"))[2].Text);
            });
        }

        [TestMethod]
        [TestCategory(Categories.Calculator)]
        [Description("Verify deleting a panel removes it from the sidecart")]
        public void VerifyDeletingPanelFromSidecart()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {

                Logger.Instance.WriteLine("STEP 1: Navigate to VM page.");
                CommonSeleniumSteps.NavigateToURL(driver, "/en-us/pricing/calculator/virtual-machines/");

                Logger.Instance.WriteLine("STEP 2: Add a new VM.");
                CalculatorHelper.ReNameVM(driver.FindElement(By.CssSelector("input[ng-model='config.name']")), VMNAMES[0]);

                CalculatorHelper.AddNewVM(driver, VMNAMES[1]);
                Assert.IsTrue(2 == driver.FindElements(By.XPath("//li[contains(@ng-repeat,'virtual-machine')]")).Count);

                Logger.Instance.WriteLine("STEP 3: Remove a VM.");
                CalculatorHelper.ReMoveVM(driver, VMNAMES[1]);

                Logger.Instance.WriteLine("STEP 4: Verify deleting a panel removes it from the sidecart.");
                List<IWebElement> vmSideCart = driver.FindElements(By.XPath("//li[contains(@ng-repeat,'virtual-machine')]")).ToList();
                Assert.IsTrue(1 == vmSideCart.Count);
                Assert.AreEqual(VMNAMES[0], vmSideCart[0].FindElement(By.CssSelector("a")).Text);
                string vmEstimatedCost = driver.FindElement(By.CssSelector("div[class='total'] span[class='numeric ng-binding']")).Text;
                string vmCostSideCart = vmSideCart[0].FindElement(By.CssSelector("span")).Text;
                Assert.AreEqual(vmEstimatedCost, vmCostSideCart);
            });
        }

        [TestMethod]
        [TestCategory(Categories.Calculator)]
        [Description("Verify sidecart prices match panel prices")]
        public void SidecartPricesMatchPanelPrices()
        {
            this.CreateSeleniumTestContext().Run(driver =>
            {
                CommonSeleniumSteps.NavigateToURL(driver, "/en-us/pricing/calculator/virtual-machines/");

                IWebElement vm1 = driver.FindElement(By.CssSelector("input[ng-model='config.name']"));
                vm1.Clear();
                vm1.SendKeys(VMNAMES[0]);
                driver.FindElement(By.CssSelector("div[xa-model='config.count'] input")).SendKeys("0");

                driver.FindElement(By.CssSelector("a[ng-click='calculator.addVirtualMachine($event)']")).Click();
                IWebElement vm2 = driver.FindElements(By.CssSelector("input[ng-model='config.name']"))[1];
                vm2.Clear();
                vm2.SendKeys(VMNAMES[1]);
                driver.FindElements(By.CssSelector("div[xa-model='config.hours'] input"))[1].Clear();

                string vm1EstimatedCost = driver.FindElements(By.CssSelector("div[class='total'] span[class='numeric ng-binding']"))[0].Text;
                string vm2EstimatedCost = driver.FindElements(By.CssSelector("div[class='total'] span[class='numeric ng-binding']"))[1].Text;

                Assert.AreEqual(vm1EstimatedCost, driver.FindElements(By.XPath("//li[contains(@ng-repeat,'virtual-machine')]"))[0].FindElement(By.CssSelector("span[class='total ng-binding']")).Text);
                Assert.AreEqual(vm2EstimatedCost, driver.FindElements(By.XPath("//li[contains(@ng-repeat,'virtual-machine')]"))[1].FindElement(By.CssSelector("span[class='total ng-binding']")).Text);

                float totalPrices = float.Parse(vm1EstimatedCost.Substring(1)) + float.Parse(vm2EstimatedCost.Substring(1));
                float estimateTotalPrices = float.Parse(driver.FindElement(By.XPath("//div[contains(@class,'cost')]/span[contains(.,'$')]")).Text.Substring(1));
                Assert.AreEqual(totalPrices.ToString("#0.00"),estimateTotalPrices.ToString());
            });
        }
    }
}
