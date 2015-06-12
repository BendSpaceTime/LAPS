using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Automation.Helpers.JsonData
{
    public class JCultures
    {
        public string Name { get; set; }
        public JCache Cache { get; set; }
        public JData Data { get; set; }
    }

    public class JCache
    {
        public string LastRefresh { get; set; }
    }

    public class JData
    {
        public List<LData> Cultures { get; set; }
    }

    public class LData
    {
        public string Slug { get; set; }
        public string Lockey { get; set; }
        public string DefaultCurrency { get; set; }
        public string[] FallbackCultureSlugs { get; set; }
        public string IsSupported { get; set; }
        public string IsDisplayed { get; set; }
        public string Channel9Language { get; set; }
    }
}
