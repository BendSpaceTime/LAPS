////====================================================
////| Downloaded From                                  |
////| Visual C# Kicks - http://www.vcskicks.com/       |
////| License - http://www.vcskicks.com/license.php    |
////====================================================
namespace Azure.Automation.Selenium.Extensions
{
    using System;
    using System.ComponentModel;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Internal;

    public static class WebElementExtensions
    {
        public static void SendKeys(this IWebElement element, string value, bool clearFirst)
        {
            if (clearFirst)
            {
                element.Clear();
            }

            element.SendKeys(value);
        }

        public static void SetAttribute(this IWebElement element, string attributeName, string value)
        {
            IWrapsDriver wrappedElement = element as IWrapsDriver;
            if (wrappedElement == null)
            {
                throw new ArgumentException("element", "Element must wrap a web driver");
            }

            IWebDriver driver = wrappedElement.WrappedDriver;
            IJavaScriptExecutor javascript = driver as IJavaScriptExecutor;
            if (javascript == null)
            {
                throw new ArgumentException("element", "Element must wrap a web driver that supports javascript execution");
            }

            javascript.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2])", element, attributeName, value);
        }

        public static T GetAttributeAsType<T>(this IWebElement element, string attributeName)
        {
            string value = element.GetAttribute(attributeName) ?? string.Empty;
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
        }

        public static T TextAsType<T>(this IWebElement element)
        {
            string value = element.Text;
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
        }

        public static IWebElement FindParentElement(this IWebElement element)
        {
            return element.FindElement(By.XPath(".."));
        }

        public static bool IsVisibleInViewport(this IWebElement element, bool noCrop = false)
        {
            var driver = ((IWrapsDriver)element).WrappedDriver;

            var template = noCrop ? WebDriverExtensions.IsElementFullyVisibleInViewportTemplate : WebDriverExtensions.IsElementPartialyVisibleInViewportTemplate;
            var script = string.Format(template, "arguments[0]");

            return (bool)((IJavaScriptExecutor)driver).ExecuteScript(script, element);
        }

        /// <summary>
        /// Allows using native events or JS simulated click events. JS simulated clicks 
        /// allows the browser to react to clicks to elements that are not properly displayed 
        /// in the page (hidden, covered by other element on top)
        /// </summary>
        /// <param name="element">Element to click</param>
        /// <param name="useJsClick">True if click should be executed via script</param>
        public static void Click(this IWebElement element, bool useJsClick)
        {
            if (useJsClick)
            {
                var driver = (IJavaScriptExecutor)((IWrapsDriver)element).WrappedDriver;
                driver.ExecuteScript("arguments[0].click()", element);
            }
            else
            {
                element.Click();
            }
        }
    }
}
