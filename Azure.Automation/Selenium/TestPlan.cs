namespace Azure.Automation.Selenium
{
    using System;
    using System.Collections.Generic;
    using OpenQA.Selenium;

    public class TestPlan<T>
    {
        private IEnumerable<T> testData;

        private Action<IWebDriver> planInitialization;

        private Action<IWebDriver> planCleanup;

        private Action<IWebDriver, T> testIterator;

        private Func<IWebDriver, T, Exception, bool> testErrorHandler;

        private Action<IWebDriver, T> testCleanup;

        public TestPlan(IEnumerable<T> testData)
        {
            this.testData = testData;
        }

        /// <summary>
        /// Sets an action to be executed when the batch test run begins. This
        /// action will be executed only once before the first Test initialization.
        /// </summary>
        /// <param name="action">Action to execute on Plan initialization</param>
        /// <returns>Current test plan</returns>
        public TestPlan<T> WithPlanInitialization(Action<IWebDriver> action)
        {
            this.planInitialization = action;
            return this;
        }

        /// <summary>
        /// Sets an action to be executed when the a test run ends. This action 
        /// will be executed only once after the last Test cleanup.
        /// </summary>
        /// <param name="action">Action to execute on Plan cleanup</param>
        /// <returns>Current test plan</returns>
        public TestPlan<T> WithPlanCleanup(Action<IWebDriver> action)
        {
            this.planCleanup = action;
            return this;
        }

        /// <summary>
        /// Sets an action to be executed for each test data item.
        /// </summary>
        /// <param name="action">Action to execute on Plan cleanup</param>
        /// <returns>Current test plan</returns>
        public TestPlan<T> WithTestIterator(Action<IWebDriver, T> action)
        {
            this.testIterator = action;
            return this;
        }

        /// <summary>
        /// Sets an action to be excuted after each test run. This action will be 
        /// executed once per test run, after the test iterator or test error handler
        /// action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TestPlan<T> WithTestCleanup(Action<IWebDriver, T> action)
        {
            this.testCleanup = action;
            return this;
        }

        /// <summary>
        /// Sets an action to be extecuted if any exception is thrown by the test iterator.
        /// Returning a value of True will prevent the exception from bubbling up, and allow
        /// executing a test cleanup and proceed to the next test run.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public TestPlan<T> WithTestErrorHandler(Func<IWebDriver, T, Exception, bool> func)
        {
            this.testErrorHandler = func;
            return this;
        }

        public TestPlan<T> WithTestErrorHandler<T1, T2>(Func<IWebDriver, T, Exception, T1, T2, bool> func, T1 param1, T2 param2)
        {
            return this.WithTestErrorHandler((driver, data, ex) => func(driver, data, ex, param1, param2));
        }

        public void Run(IWebDriver driver)
        {
            if (this.testIterator == null)
            {
                throw new InvalidOperationException("Cannot run test plan if test iterator is not set.");
            }

            this.RunIfSet(this.planInitialization, driver);

            foreach (var data in this.testData)
            {
                try
                {
                    this.testIterator(driver, data);
                }
                catch (Exception ex)
                {
                    if (!this.RunIfSet(this.testErrorHandler, driver, data, ex))
                    {
                        throw;
                    }
                }

                this.RunIfSet(this.testCleanup, driver, data);
            }

            this.RunIfSet(this.planCleanup, driver);
        }

        private TOut RunIfSet<T1, T2, T3, TOut>(Func<T1, T2, T3, TOut> func, T1 param1, T2 param2, T3 param3)
        {
            if (func != null)
            {
                return func(param1, param2, param3);
            }
            else
            {
                return default(TOut);
            }
        }

        private void RunIfSet<T>(Action<T> action, T param)
        {
            if (action != null)
            {
                action(param);
            }
        }

        private void RunIfSet<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
        {
            if (action != null)
            {
                action(param1, param2);
            }
        }
    }
}
