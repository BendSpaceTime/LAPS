namespace Azure.Automation.Selenium.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;

    public static class WebDriverExtensions
    {
        public const string GetElementBySelectorTemplate = "(window.jQuery?jQuery:document.querySelectorAll)(\"{0}\")[{1}]";

        public const string GetElementComputedStyleTemplate = @"
                var cs = {{}}, element = {0}; 
                var style = window.getComputedStyle(element);
                for(var i in style) {{ if (isNaN(i) && i != 'length') {{ cs[i] = style[i]; }} }}
                return cs;
            ";

        public const string IsElementFullyVisibleInViewportTemplate = @"
                var rect = {0}.getBoundingClientRect();

                return rect.top >= 0 && rect.bottom <= (window.innerHeight || document.documentElement.clientHeight)
                    && rect.left >= 0 && rect.right <= (window.innerWidth || document.documentElement.clientWidth);
            ";

        public const string IsElementPartialyVisibleInViewportTemplate = @"
                var rect = {0}.getBoundingClientRect();

                return rect.top <= (window.innerHeight || document.documentElement.clientHeight) && rect.bottom >= 0
                    && rect.right >= 0 && rect.left <= (window.innerWidth || document.documentElement.clientWidth);
            ";

        public const string GetExistingVariableNamesTemplate = @"
                var varsToCheck = {0};
                var existingVars = [];

                for(var i = 0; i < varsToCheck.length; i++) {{
                    window[varsToCheck[i]] && existingVars.push(varsToCheck[i]);
                }}

                return existingVars;
            ";

        public static WebDriverWait Wait(this IWebDriver driver, int timeoutSeconds, bool ignoreSearchExceptions = false, int pollingInterval = 1, string message = null)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            wait.PollingInterval = TimeSpan.FromSeconds(pollingInterval);

            if (!string.IsNullOrEmpty(message))
            {
                wait.Message = message;
            }

            if (ignoreSearchExceptions)
            {
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
            }

            return wait;
        }

        public static IWebElement WaitFindElement(this IWebDriver driver, By locator, int timeoutSeconds, bool expectVisible = false, string errorMessage = null, bool failOnTimeout = true)
        {
            return driver.WaitFindElement(driver, locator, timeoutSeconds, expectVisible, errorMessage, failOnTimeout);
        }

        public static bool TryTakeScreenshot(this IWebDriver driver, out Screenshot screenshot)
        {
            var screenshotDriver = driver as ITakesScreenshot;
            if (screenshotDriver == null)
            {
                screenshot = null;
                return false;
            }
            else
            {
                try
                {
                    screenshot = screenshotDriver.GetScreenshot();
                    return true;
                }
                catch
                {
                    screenshot = null;
                    return false;
                }
            }
        }

        public static bool TryTakeScreenshot(this IWebDriver driver)
        {
            Screenshot screenshot;
            return driver.TryTakeScreenshot(out screenshot);
        }

        public static Screenshot TakeScreenshot(this IWebDriver driver)
        {
            Screenshot screenshot;
            if (!driver.TryTakeScreenshot(out screenshot))
            {
                throw new NotSupportedException("Take Screenshot is not supported by this WebDriver type");
            }

            return screenshot;
        }

        /// <remarks>
        /// Downloaded from Visual C# Kicks - http://www.vcskicks.com/
        /// License - http://www.vcskicks.com/license.php  
        /// </remarks>
        public static string GetText(this IWebDriver driver)
        {
            return driver.FindElement(By.TagName("body")).Text;
        }

        /// <remarks>
        /// Downloaded from Visual C# Kicks - http://www.vcskicks.com/
        /// License - http://www.vcskicks.com/license.php  
        /// </remarks>
        public static void WaitForPageToLoad(this IWebDriver driver, int seconds = 30)
        {
            TimeSpan timeout = TimeSpan.FromSeconds(seconds);
            WebDriverWait wait = new WebDriverWait(driver, timeout);

            IJavaScriptExecutor javascript = driver as IJavaScriptExecutor;
            if (javascript == null)
            {
                throw new ArgumentException("driver", "Driver must support javascript execution");
            }

            wait.Until((d) =>
            {
                try
                {
                    string readyState = javascript.ExecuteScript("if (document.readyState) return document.readyState;").ToString();
                    return readyState.ToLower() == "complete";
                }
                catch (InvalidOperationException e)
                {
                    // Window is no longer available
                    return e.Message.ToLower().Contains("unable to get browser");
                }
                catch (WebDriverException e)
                {
                    // Browser is no longer available
                    return e.Message.ToLower().Contains("unable to connect");
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Gets the computed style object of the selected element
        /// </summary>
        /// <param name="driver">Driver with the remote session to execute the code at</param>
        /// <param name="querySelector">CSS Selector to target the desired element</param>
        /// <param name="selectionResultIndex">Index of the target element in the array of selected elements (First element by default)</param>
        /// <returns></returns>
        public static ComputedStyle GetElementComputedStyle(this IWebDriver driver, string querySelector, int selectionResultIndex = 0)
        {
            var script = string.Format(GetElementComputedStyleTemplate, string.Format(GetElementBySelectorTemplate, querySelector, selectionResultIndex));
            return new ComputedStyle((Dictionary<string, object>)((IJavaScriptExecutor)driver).ExecuteScript(script));
        }

        /// <summary>
        /// Gets if an element is being displayed within the current browser viewport
        /// </summary>
        /// <param name="driver">Driver with the remote session to execute the code at</param>
        /// <param name="querySelector">CSS Selector to target the desired element</param>
        /// <param name="selectionResultIndex">Index of the target element in the array of selected elements (First element by default)</param>
        /// <param name="noCrop">Evaluate if the element is fully visible in the viewport (no cropped content) (False by default)</param>
        /// <returns></returns>
        public static bool IsElementVisibleInViewport(this IWebDriver driver, string querySelector, int selectionResultIndex = 0, bool noCrop = false)
        {
            var template = noCrop ? IsElementFullyVisibleInViewportTemplate : IsElementPartialyVisibleInViewportTemplate;
            var script = string.Format(template, string.Format(GetElementBySelectorTemplate, querySelector, selectionResultIndex));

            return (bool)((IJavaScriptExecutor)driver).ExecuteScript(script);
        }

        /// <summary>
        /// Checks if a list of variables exist in the page Js context
        /// </summary>
        /// <param name="driver">Driver with the remote session to execute the code at</param>
        /// <param name="variableNames">List of variable names to check</param>
        /// <returns>List of matching existing variables</returns>
        public static IEnumerable<string> CheckJsVariablesExist(this IWebDriver driver, params string[] variableNames)
        {
            var script = string.Format(GetExistingVariableNamesTemplate, string.Format("[\"{0}\"]", string.Join("\",\"", variableNames)));
            return ((IReadOnlyCollection<object>)((IJavaScriptExecutor)driver).ExecuteScript(script)).Cast<string>();
        }

        public static void WaitForURLChange(this IWebDriver driver, Action action, int seconds = 30)
        {
            var currentUrl = driver.Url;

            action();

            var wait = driver.Wait(seconds);

            // Ignore stale element reference exceptions
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
            
            wait.Message = "Navigation did not trigger a change in the page Url";
            wait.Until(d => d.Url != currentUrl);

            driver.WaitForPageToLoad(seconds);
        }

        public static void RepeatActionUntilUrlChangesAndWaitForDocumentLoad(this IWebDriver driver, Action action, int seconds = 30)
        {
            var currentUrl = driver.Url;

            var wait = driver.Wait(seconds);
            wait.Message = "Navigation did not trigger a change in the page Url";

            wait.Until(d => 
            {
                action();

                return d.Url != currentUrl;
            });

            driver.WaitForPageToLoad(seconds);
        }
    }
}
