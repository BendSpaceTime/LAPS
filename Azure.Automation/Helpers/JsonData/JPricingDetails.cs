using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Automation.Helpers.JsonData
{
    public class JPricingDetails
    {
        public string Name { get; set; }
        public SCache Cache { get; set; }
        public SData Data { get; set; }
    }

    public class SCache 
    {
        public string LastRefresh { get; set; }
    }

    public class SData
    {
        public List<SServices> Services { get; set; }
    }

    public class SServices
    {
        public object Id { get; set; }

        public object Slug { get; set; }

        public string DisplayName { get; set; }

        public object LocKey { get; set; }

        public object Url { get; set; }

        public object DocumentationUrl { get; set; }

        public string PricingUrl { get; set; }

        public object FeedbackForumId { get; set; }

        public object CategorySlugs { get; set; }

        public object Categories { get; set; }

        public object Solutions { get; set; }

        public object CapabilitySlugs { get; set; }

        public object Capabilities { get; set; }

        public object IsActive { get; set; }

        public object IsGlobal { get; set; }

        public object StatusLocKey { get; set; }

        public object MSDNForumSlug { get; set; }

        public object StackOverflowTag { get; set; }

        public object RelatedServiceSlugs { get; set; }

        public object Compliances { get; set; }
    }
}
