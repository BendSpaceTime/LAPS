namespace Azure.Automation.Helpers
{
    public static class XPathUtils
    {
        public static string Escape(string value)
        {
            return System.Security.SecurityElement.Escape(value);
        }
    }
}
