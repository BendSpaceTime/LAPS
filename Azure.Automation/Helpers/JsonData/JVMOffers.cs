using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Automation.Helpers.JsonData
{
    public class VMOffer
    {
        public string Slug { get; set; }
        public string Tier { get; set; }
        public string Type { get; set; }
        public string Series { get; set; }
        public string Cores { get; set; }
        public string Disk { get; set; }
        public string Ram { get; set; }
        public string InstanceSize { get; set; }
        public string DiskType { get; set; }
        public string Sort { get; set; }

        public Dictionary<string, decimal> Prices { get; set; }
    }
}
