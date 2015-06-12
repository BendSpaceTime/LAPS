using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Timers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Microsoft.Selenium.Utilities
{
    /// <summary>
    /// Extensions for web driver.
    /// </summary>
    public static class WebDriverHelpers
    {
        private static TimeSpan defaultTimeout = TimeSpan.FromSeconds(10);
        private static Dictionary<double, long> javaScriptMemoryUsageLogs;
        private static Timer getMemoryUsageTimer;

        /// <summary>
        /// Waits until a condition is met with a default timeout.
        /// </summary>
        /// <param name="webDriver">The web driver instance</param>
        /// <param name="condition">Condition function, returning anything other than null or false passes the condition.</param>
        /// <param name="errorMessage">Error message to display in the case of timeout.</param>
        /// <typeparam name="TResult">The delegate's expected return type.</typeparam>
        public static TResult WaitUntil<TResult>(this IWebDriver webDriver, Func<TResult> condition, string errorMessage)
        {
            return WaitUntil(webDriver, condition, errorMessage, defaultTimeout);
        }

        /// <summary>
        /// Waits until a condition is met with a specified timeout.
        /// </summary>
        /// <param name="webDriver">The web driver instance</param>
        /// <param name="condition">Condition function, returning anything other than null or false passes the condition.</param>
        /// <param name="errorMessage">Error message to display in the case of timeout.</param>
        /// <param name="timeout">The timeout value indicating how long to wait for the condition.</param>     
        /// <typeparam name="TResult">The delegate's expected return type.</typeparam>
        public static TResult WaitUntil<TResult>(this IWebDriver webDriver, Func<TResult> condition, string errorMessage, TimeSpan timeout)
        {
            var wait = new WebDriverWait(webDriver, timeout);

            // Ignore stale element reference exceptions
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

            try
            {
                return wait.Until(driver => condition());
            }
            catch (Exception ex)
            {
                if (string.IsNullOrWhiteSpace(errorMessage))
                {
                    throw;
                }
                else
                {
                    throw new WebDriverException(errorMessage, ex);
                }
            }
        }

        /// <summary>
        /// Takes a screenshot of the current web driver's browser.
        /// </summary>
        /// <param name="webDriver">The web driver instance.</param>
        /// <param name="screenshotDirectory">Directory to use to store screen shots.</param>
        /// <param name="filenamePrefix">Prefix string to add to the screenshot's file name. It is recommended that you use the current Test's name.</param>
        /// <returns>The full path to the file that was saved.</returns>
        public static string TakeScreenshot(this IWebDriver webDriver, string screenshotDirectory = null, string filenamePrefix = "screenshot")
        {
            string fullPath = string.Empty;

            Trace.TraceInformation("Taking screenshot of browser at URI: {0}", webDriver.Url);

            try
            {
                // Create the directory if it doesn't exist and its not null or empty
                if (!string.IsNullOrWhiteSpace(screenshotDirectory) && !System.IO.Directory.Exists(screenshotDirectory))
                {
                    System.IO.Directory.CreateDirectory(screenshotDirectory);
                }

                // Generate a screenshot based on a sortable date time if none is specified
                string fileName = string.Format(CultureInfo.InvariantCulture, "{0}-{1:yyyy-MM-dd_hh-mm-ss-ff-tt}.png", filenamePrefix, DateTime.Now);
                fullPath = System.IO.Path.Combine(screenshotDirectory, fileName);
                ((ITakesScreenshot)webDriver).GetScreenshot().SaveAsFile(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                fullPath = System.IO.Path.GetFullPath(fullPath);
                Trace.TraceInformation("Screenshot saved to: {0}", fullPath);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error taking screenshot: {0}", ex);
            }

            return fullPath;
        }

        /// <summary>
        /// Enables interception of JavaScript Console logs.
        /// </summary>
        /// <param name="webDriver">The web driver instance.</param>
        public static void EnableJavaScriptConsoleInterception(this IWebDriver webDriver)
        {
            if (!IsJavaScriptConsoleInterceptionEnabled(webDriver))
            {
                string script = "var interceptedLogs = new Array();"
                + " function takeOverConsole() {"
                + "     var console = window.console;"
                    + " if (!console) return;"
                    + " function intercept(method) {"
                        + " var original = console[method];"
                        + " console[method] = function () {"
                            + " var argumentsString = '';"
                            + " for (var i = 0; i < arguments.length; i++) {"
                            + "     argumentsString += arguments[i] + ' ';"
                            + " }"

                            + " interceptedLogs.push(method + ': ' + argumentsString);"
                        + " }"
                    + " }"
                    + " var methods = ['log', 'warn', 'error', 'debug', 'Verbose', 'Warning', 'Error'];"
                    + " for (var i = 0; i < methods.length; i++)"
                        + " intercept(methods[i]);"
                + " }";

                script = @"var s = document.createElement('script');
                s.type = 'text/javascript';
                var code = """ + script + @"""; 
                try {
                     s.appendChild(document.createTextNode(code));
                     document.body.appendChild(s);
                } catch (e) {
                    s.text = code;
                    document.body.appendChild(s);
                }";

                (webDriver as IJavaScriptExecutor).ExecuteScript(script);
                (webDriver as IJavaScriptExecutor).ExecuteScript("takeOverConsole()");
                webDriver.WaitUntil(
                    () =>
                    {
                        try
                        {
                            if ((webDriver as IJavaScriptExecutor).ExecuteScript("return MsPortalFx.Base.Diagnostics.Log.enableConsoleOutput") != null)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        catch (System.InvalidOperationException)
                        {
                            return false;
                        }
                    },
                "MsPortalFx.Base.Diagnostics.Log.enableConsoleOutput is not defined.");

                (webDriver as IJavaScriptExecutor).ExecuteScript("MsPortalFx.Base.Diagnostics.Log.enableConsoleOutput()");
            }
        }

        /// <summary>
        /// Gets all the logs sent to the JavaScript console since the last call to EnableJavaScriptConsoleInterception.
        /// </summary>
        /// <param name="webDriver">The web driver instance.</param>
        /// <returns>An object containing the list of console logs.</returns>
        public static IEnumerable<string> GetJavaScriptConsoleLogs(this IWebDriver webDriver)
        {
            if (!IsJavaScriptConsoleInterceptionEnabled(webDriver))
            {
                throw new WebDriverException("JavaScript console interception is not enabled. Please call the EnableJavaScriptConsoleInterception method before calling this method.");
            }

            var logs = (webDriver as IJavaScriptExecutor).ExecuteScript("return interceptedLogs");
            return (logs as System.Collections.ObjectModel.ReadOnlyCollection<object>)
                   .Select(log => (log as string));
        }

        /// <summary>
        /// Waits for an alert for a specified timeout.
        /// </summary>
        /// <param name="webDriver">The web driver instance</param>
        /// <param name="errorMessage">Error message to display in the case of timeout.</param>
        /// <param name="timeout">The timeout value indicating how long to wait for the condition.</param>     
        /// <returns>The alert shown on the browser.</returns>
        public static IAlert WaitForAlert(this IWebDriver webDriver, string errorMessage, TimeSpan timeout)
        {
            var wait = new WebDriverWait(webDriver, timeout);

            return wait.Until<IAlert>(driver =>
            {
                try
                {
                    return driver.SwitchTo().Alert();
                }
                catch (NoAlertPresentException)
                {
                    return null;
                }
            });
        }

        /// <summary>
        /// Waits for an alert.
        /// </summary>
        /// <param name="webDriver">The web driver instance</param>
        /// <param name="errorMessage">Error message to display in the case of timeout.</param>
        /// <returns>The alert shown on the browser.</returns>
        public static IAlert WaitForAlert(this IWebDriver webDriver, string errorMessage)
        {
            return WaitForAlert(webDriver, errorMessage, defaultTimeout);
        }

        /// <summary>
        /// Performs a drag and drop operation from one element to another
        /// </summary>
        /// <param name="webDriver">The web driver instance.</param>
        /// <param name="source">The element on which the drag operation is started.</param>
        /// <param name="target">The element on which the drop is performed.</param>
        /// <remarks>This method does not check that the drag and drop operation succeeded. 
        /// It is up to the caller to verify that the result of drag and drop is the expected one.</remarks>
        public static void DragAndDrop(this IWebDriver webDriver, IWebElement source, IWebElement target)
        {
            Actions actions = new Actions(webDriver);
            actions.ClickAndHold(source);
            actions.DragAndDrop(source, target);
            actions.Perform();
        }

        /// <summary>
        /// Returns a value indicating whether JavaScript Console interception is enabled.
        /// </summary>
        /// <param name="webDriver">The <see cref="WebDriver"/> instance.</param>
        /// <returns>True if JavaScript Console interception is enabled; false, otherwise.</returns>
        public static bool IsJavaScriptConsoleInterceptionEnabled(this IWebDriver webDriver)
        {
            var interceptionDisabled = (webDriver as IJavaScriptExecutor).ExecuteScript("return typeof interceptedLogs === 'undefined'");
            return !(bool)interceptionDisabled;
        }

        #region Performance Gathering

        /// <summary>
        /// Checks if performance gathering is enabled
        /// </summary>
        /// <param name="webDriver">the web driver instance</param>
        /// <returns>true if the TestFramework.Portal.Performance.EnablePerformanceTracing is true (case insensitive), else false</returns>
        public static bool IsPerformanceGatheringEnabled(this IWebDriver webDriver)
        {
            bool enablePerformanceTracing;

            // Check if we should enable performance tracing
            if (!bool.TryParse(ConfigurationManager.AppSettings["TestFramework.Portal.Performance.EnablePerformanceTracing"], out enablePerformanceTracing))
            {
                // Unable to parse the setting, so disable performance tracing
                enablePerformanceTracing = false;
            }

            return enablePerformanceTracing;
        }

        #region Memory
        /// <summary>
        /// Starts the gathering of memory usage information.
        /// </summary>
        /// <param name="webDriver">the web driver instance</param>
        /// <param name="interval">the interval to wait between each check of the memory usage</param>
        /// <remarks>This only works for chrome currently</remarks>
        public static void StartMemoryInfoGathering(this IWebDriver webDriver, TimeSpan interval)
        {
            if (webDriver.IsMemoryInfoSupported())
            {
                javaScriptMemoryUsageLogs = new Dictionary<double, long>();

                // If we get an interval less than 1 millisecond then default to 500
                if (interval.TotalMilliseconds < 1)
                {
                    interval = TimeSpan.FromMilliseconds(500);
                }

                getMemoryUsageTimer = new System.Timers.Timer(interval.TotalMilliseconds);

                getMemoryUsageTimer.Elapsed += (object source, ElapsedEventArgs e) =>
                {
                    if (getMemoryUsageTimer != null && getMemoryUsageTimer.Enabled)
                    {
                        var memInfo = GetMemoryInfoNow(webDriver);
                        javaScriptMemoryUsageLogs.Add(memInfo.Item1, memInfo.Item2);
                    }
                };
                getMemoryUsageTimer.AutoReset = true;
                getMemoryUsageTimer.Start();
            }
        }

        /// <summary>
        /// Gets the memory info at the current instant its requested
        /// </summary>
        /// <param name="webDriver">the web driver instance</param>
        /// <returns>A tuple representing the time the data was requested (based on page navigation timing) and the memory usage in bytes.</returns>
        /// <remarks>Performance gathering must be enabled, else the function will return a tuple of 0,0.  This only works for chrome currently.</remarks>
        public static Tuple<double, long> GetMemoryInfoNow(this IWebDriver webDriver)
        {
            if (webDriver.IsMemoryInfoSupported())
            {
                ReadOnlyCollection<object> timeAndUsedJSHeapSize = ((IJavaScriptExecutor)webDriver).ExecuteScript("return [performance.now(), performance.memory.usedJSHeapSize];") as ReadOnlyCollection<object>;

                double time = (double)timeAndUsedJSHeapSize[0];
                long usedJSHeapSize = (long)timeAndUsedJSHeapSize[1];
                return new Tuple<double, long>(time, usedJSHeapSize);
            }
            else
            {
                throw new NotSupportedException("webDriver.GetMemoryInfoNow() is not supported for the driver " + webDriver.GetType().ToString());
            }
        }

        /// <summary>
        /// Starts the gathering of memory usage information.
        /// </summary>
        /// <param name="webDriver">the web driver instance</param>
        /// <param name="intervalInMilliseconds">the interval to wait between each check of the memory usage in milliseconds</param>
        /// <remarks>This only works for chrome currently</remarks>
        public static void StartMemoryInfoGathering(this IWebDriver webDriver, int intervalInMilliseconds)
        {
            StartMemoryInfoGathering(webDriver, TimeSpan.FromMilliseconds(intervalInMilliseconds));
        }

        /// <summary>
        /// Starts the gathering of memory usage information.
        /// </summary>
        /// <param name="webDriver">the web driver instance</param>
        /// <remarks>This only works for chrome currently</remarks>
        public static void StartMemoryInfoGathering(this IWebDriver webDriver)
        {
            int intervalInMilliseconds;
            if (!int.TryParse(ConfigurationManager.AppSettings["TestFramework.Portal.Performance.GetJSMemoryUsageInterval"], out intervalInMilliseconds))
            {
                intervalInMilliseconds = 500;
            }

            StartMemoryInfoGathering(webDriver, TimeSpan.FromMilliseconds(intervalInMilliseconds));
        }

        /// <summary>
        /// Stops the gathering of memory usage information
        /// </summary>
        /// <param name="webDriver">the web driver instance</param>
        public static void StopMemoryInfoGathering(this IWebDriver webDriver)
        {
            if (webDriver.IsMemoryInfoGatheringEnabled())
            {
                getMemoryUsageTimer.Stop();
            }
        }

        /// <summary>
        /// Retrieves the memory info measurements with the key being time measured as a DOMHighResTimeStamp and the value being memory usage in bytes
        /// </summary>
        /// <returns>the memory info measurements</returns>
        /// <remarks>See <see href="http://www.w3.org/TR/hr-time/#dom-performance-now"/> for details</remarks>
        public static IEnumerable<KeyValuePair<double, long>> GetMemoryInfoMeasurements(this IWebDriver webDriver)
        {
            return javaScriptMemoryUsageLogs;
        }

        /// <summary>
        /// Returns true if memory info gathering is supported
        /// </summary>
        /// <param name="webDriver">the web driver instance</param>
        /// <returns>true if memory info gathering is supported</returns>
        public static bool IsMemoryInfoSupported(this IWebDriver webDriver)
        {
            // Currently only chrome is supported
            return typeof(OpenQA.Selenium.Chrome.ChromeDriver) == webDriver.GetType();
        }

        /// <summary>
        /// Returns true if memory info gathering is enabled and running
        /// </summary>
        /// <param name="webDriver">the web driver instance</param>
        /// <returns>true if memory info gathering is enabled and running</returns>
        public static bool IsMemoryInfoGatheringEnabled(this IWebDriver webDriver)
        {
            return getMemoryUsageTimer != null && getMemoryUsageTimer.Enabled;
        }

        #endregion

        #region Timing

        /// <summary>
        /// Gets the page load timings as measured by the browser
        /// </summary>
        /// <param name="webDriver">the web driver instance</param>
        /// <returns>the page load timings</returns>
        /// <remarks> See <see href="http://www.w3.org/TR/performance-timeline/#sec-performance-timeline" /> for details about browser performance timing</remarks>
        public static string GetPageLoadTimings(this IWebDriver webDriver)
        {
            return ((IJavaScriptExecutor)webDriver).ExecuteScript("return JSON.stringify(performance.timing)") as string;
        }

        /// <summary>
        /// Gets the resource loading timings as measured by the browser
        /// </summary>
        /// <param name="webDriver">the web driver instance</param>
        /// <returns>the resource loading timings</returns>
        /// <remarks>Chrome 30.0.1599.101m only stores the first 150 resource timings, IE11 stores 300+, does not work on FireFox 25.  See <see href="http://www.w3.org/TR/performance-timeline/#sec-performance-timeline" /> for details about browser performance timing</remarks>
        public static string GetResourceTimings(this IWebDriver webDriver)
        {
            if (webDriver.IsGetEntriesSupported())
            {
                return ((IJavaScriptExecutor)webDriver).ExecuteScript("return JSON.stringify(performance.getEntries())") as string;
            }

            return "GetResourceTimings() is not supported for webdriver type: " + webDriver.GetType();
        }

        /// <summary>
        /// Gets the user defined mark timings
        /// </summary>
        /// <param name="webDriver">the web driver instance</param>
        /// <returns>the user defined mark timings</returns>
        /// <remarks>See <see href="http://www.w3.org/TR/user-timing/" /> for user timing details</remarks>
        public static string GetUserDefinedMarkTimings(this IWebDriver webDriver)
        {
            if (webDriver.IsGetEntriesSupported())
            {
                return ((IJavaScriptExecutor)webDriver).ExecuteScript("return JSON.stringify(performance.getEntriesByType('mark'))") as string;
            }

            return "GetUserDefinedMarkTimings() is not supported for webdriver type: " + webDriver.GetType();
        }

        /// <summary>
        /// Gets the user defined measure timings
        /// </summary>
        /// <param name="webDriver">the web driver instance</param>
        /// <returns>the user defined timings</returns>
        /// <remarks>See <see href="http://www.w3.org/TR/user-timing/" /> for user timing details</remarks>
        public static string GetUserDefinedMeasureTimings(this IWebDriver webDriver)
        {
            if (webDriver.IsGetEntriesSupported())
            {
                return ((IJavaScriptExecutor)webDriver).ExecuteScript("return JSON.stringify(performance.getEntriesByType('measure'))") as string;
            }

            return "GetUserDefinedMeasureTimings() is not supported for webdriver type: " + webDriver.GetType();
        }

        /// <summary>
        /// Writes the page load timings to the specified location or temp directory if no path is specified
        /// </summary>
        /// <param name="webDriver">the web driver instance</param>
        /// <param name="path">the path to write the logs to, if not specified then the temp directory</param>
        /// <returns>the path to the log file</returns>
        /// <remarks> See <see href="http://www.w3.org/TR/performance-timeline/#sec-performance-timeline" /> for details about browser performance timing</remarks>
        public static string WritePageLoadTimings(this IWebDriver webDriver, string path = "")
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "pageLoadTimings.txt");
            }

            WriteToFile(path, webDriver.GetPageLoadTimings());

            return path;
        }

        /// <summary>
        /// Writes the resource timings to the specified location or temp directory if no path is specified
        /// </summary>
        /// <param name="webDriver">the web driver instance</param>
        /// <param name="path">the path to write the logs to, if not specified then the temp directory</param>
        /// <returns>the path to the log file</returns>
        /// <remarks>Chrome 30.0.1599.101m only stores the first 150 resource timings, IE11 stores 300+, does not work on FireFox 25.  See <see href="http://www.w3.org/TR/performance-timeline/#sec-performance-timeline" /> for details about browser performance timing</remarks>
        public static string WriteResourceTimings(this IWebDriver webDriver, string path = "")
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "resourceTimings.txt");
            }

            WriteToFile(path, webDriver.GetResourceTimings());

            return path;
        }

        /// <summary>
        /// Writes the user defined mark timings to the specified location or temp directory if no path is specified
        /// </summary>
        /// <param name="webDriver">the web driver instance</param>
        /// <param name="path">the path to write the logs to, if not specified then the temp directory</param>
        /// <returns>the path to the log file</returns>
        /// <remarks>See <see href="http://www.w3.org/TR/user-timing/" /> for user timing details</remarks>
        public static string WriteUserDefinedMarkTimings(this IWebDriver webDriver, string path = "")
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "userDefinedMarkTimings.txt");
            }

            WriteToFile(path, webDriver.GetUserDefinedMarkTimings());

            return path;
        }

        /// <summary>
        /// Writes the user defined measure timings to the specified location or temp directory if no path is specified
        /// </summary>
        /// <param name="webDriver">the web driver instance</param>
        /// <param name="path">the path to write the logs to, if not specified then the temp directory</param>
        /// <returns>the path to the log file</returns>
        /// <remarks>See <see href="http://www.w3.org/TR/user-timing/" /> for user timing details</remarks>
        public static string WriteUserDefinedMeasureTimings(this IWebDriver webDriver, string path = "")
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "userDefinedMeasureTimings.txt");
            }

            WriteToFile(path, webDriver.GetUserDefinedMeasureTimings());

            return path;
        }

        private static void WriteToFile(string path, string text)
        {
            // Create the directory if it does't exist
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            System.IO.File.WriteAllText(path, text);
        }

        private static bool IsGetEntriesSupported(this IWebDriver webDriver)
        {
            // performance.GetEntries() doesn't work in firefox.
            return typeof(OpenQA.Selenium.Chrome.ChromeDriver) == webDriver.GetType() ||
                typeof(OpenQA.Selenium.IE.InternetExplorerDriver) == webDriver.GetType();
        }

        #endregion

        #endregion
    }
}
