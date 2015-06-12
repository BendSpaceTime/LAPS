using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Automation.Helpers.JsonData
{
    class JBandWidthOffer
    {
        public string Slug { get; set; }
        public Dictionary<string, decimal>[] Prices { get; set; }
    }
}
