namespace Azure.Automation.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Azure.Automation.Selenium;

    public class Logger
    {
        #region Static access

        private static Logger instance = new Logger();

        public static Logger Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion

        public void LogAction(string description, Action action, bool failOnEnd = false)
        {
            Stopwatch watch = new Stopwatch();

            try
            {
                watch.Start();
                action();

                watch.Stop();
                this.Log("SUCCESS", this.GetActionDescription(description, watch.ElapsedMilliseconds));
            }
            catch (Exception ex)
            {
                watch.Stop();
                this.Log("FAILED", this.GetActionDescription(description, watch.ElapsedMilliseconds));

                if (failOnEnd)
                {
                    SeleniumTestContext.Current.FailOnEnd(string.Format("{0}: {1}", description, ex.Message));
                }
                else
                {
                    throw;
                }
            }
        }

        public void Log(string logType, string message)
        {
            this.WriteLine("[{0}] {1}", logType, message);
        }

        public void LogBrowserProfile(WebDriverTargetBrowser targetBrowser)
        {
            this.WriteLine(string.Format("[BROWSER] {0}", targetBrowser.Description));
            this.WriteLine(string.Format("[PROFILE] {0}", targetBrowser.Profile));
            this.WriteLine("Test Steps:");
        }

        public void LogTestStart(int count)
        {
            this.WriteSeparatorLine();
            this.WriteLine("TEST START");
            this.WriteLine(string.Format("Number of Browser Profiles In Scope: {0}", count));
            this.WriteSeparatorLine();
        }

        public void LogTestEnd()
        {            
            this.WriteLine("TEST END");
            this.WriteSeparatorLine();
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteLineIndent(string message)
        {
            Console.WriteLine(string.Format("     {0}", message));
        }

        public void WriteLine(string message, params object[] formatParams)
        {
            Console.WriteLine(string.Format(message, formatParams));
        }

        public void WriteSeparatorLine()
        {
            this.WriteLine("------------------------------------------------------------------------------------");
        }

        public void LogUnsupportedBrowser(WebDriverTargetBrowser targetBrowser)
        {
            this.WriteLine(string.Format("[BROWSER] {0}", targetBrowser.Description));
            this.WriteLine(string.Format("[PROFILE] {0}", targetBrowser.Profile));
            this.WriteLine("TEST NOT RUN: UNSUPPORTED BROWSER");
            this.WriteSeparatorLine();
        }

        public void LogUnsupportedBrowsers(Dictionary<MethodInfo, UnsupportedBrowserAttribute[]> methods)
        {
            Logger.Instance.WriteSeparatorLine();
            Logger.Instance.WriteLine("UNSUPPORTED TESTS/BROWSERS COMBINATIONS");
            Logger.Instance.WriteSeparatorLine();

            foreach (var method in methods.Keys)
            {
                Logger.Instance.WriteLine(string.Format("[TEST] {0}", method.Name));
                foreach (UnsupportedBrowserAttribute attribute in methods[method])
                {
                    Logger.Instance.WriteLineIndent(string.Format("[BROWSER] {0}", attribute.WebDriverTargetBrowser.Description));
                }
            }

            Logger.Instance.WriteSeparatorLine();
        }

        private string GetActionDescription(string description, long milliseconds)
        {
            return description = TestConfiguration.Instance.LogTimings ? string.Format("({0} s) {1}", milliseconds / 1000, description) : description;
        }
    }
}