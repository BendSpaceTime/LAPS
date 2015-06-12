namespace Azure.Automation.Selenium.Extensions
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;

    public static class SelectElementExtensions
    {
        public static bool SelectByText(this SelectElement selectElement, string text, bool throwIfNotFound)
        {
            try
            {
                selectElement.SelectByText(text);
                return true;
            }
            catch (NoSuchElementException)
            {
                if (!throwIfNotFound)
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
