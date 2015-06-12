namespace Azure.Automation.Fixtures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Azure.Automation.Helpers;
    using Azure.Automation.Selenium;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestValidationAndParameterOutputFixture
    {
        [TestMethod]
        [Description("Outputs the test parameters")]
        public void TestConfigurationInitialization()
        {
            var testConfig = Azure.Automation.Helpers.TestConfiguration.Instance;
            Assert.IsNotNull(testConfig);

            testConfig.LogSettings();
        }

        [TestMethod]
        [Description("Outputs tests marked with the [UnsupportedBrowserAttribute]")]
        public void ShowUnsupportedBrowserMethods()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            Dictionary<MethodInfo, UnsupportedBrowserAttribute[]> methods = assembly.GetTypes().SelectMany(t => t.GetMethods())
                                            .Where(m => m.GetCustomAttributes(typeof(UnsupportedBrowserAttribute)).Count() > 0)
                                            .ToDictionary(m => m, m => m.GetCustomAttributes(typeof(UnsupportedBrowserAttribute)).Select(a => (UnsupportedBrowserAttribute)a).ToArray());

            Logger.Instance.LogUnsupportedBrowsers(methods);
        }

        [TestMethod]
        [Description("Make sure all tests have the [Description] attribute")]
        public void ValidateTestsHaveDescriptionAttribute()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            List<string> methods = assembly.GetTypes().SelectMany(t => t.GetMethods())
                                            .Where(m => m.GetCustomAttributes(typeof(TestMethodAttribute)).Count() > 0 && m.GetCustomAttributes(typeof(DescriptionAttribute)).Count() == 0)
                                            .Select(a => a.Name).ToList();

            if (methods.Count() > 0)
            {
                Logger.Instance.WriteLine("There are test methods without the [Description] attribute. See the list below.");
                methods.ForEach(m => Logger.Instance.WriteLineIndent(string.Format("- {0}", m)));
                Assert.Fail();                
            }

            Logger.Instance.WriteLine("All validations passed.");
        }
    }
}
