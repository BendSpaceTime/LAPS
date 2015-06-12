using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Azure.Automation.Helpers
{
    public class ExtractUrlsFromRobots
    {
        private static List<string> allUrls = new List<string>();
        private static List<string> siteMap = new List<string>();
        private static List<string> DisAllowDir = new List<string>();
        private static void initialize()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(TestConfiguration.Instance.EnvironmentUrl+"/robots.txt");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            string data = sr.ReadLine();
            while (data != null)
            {
                if (data.Contains("Sitemap"))
                {
                    int pos = data.IndexOf("http://");
                    string url = data.Substring(pos, data.Length - pos);
                    siteMap.Add(url);
                }

                if (data.Contains("Disallow"))
                {
                    //return the first index of the given character matched in the string
                    int pos = data.IndexOf('/');
                    string url = data.Substring(pos, data.Length - pos);
                    DisAllowDir.Add(url);
                }
                data = sr.ReadLine();
            }

        }

        public static List<string> getAllURL(string tagName = "loc")
        {
            if (allUrls.Count == 0)
            {
                if (siteMap.Count == 0)
                {
                    initialize();
                }

                XmlDocument doc = new XmlDocument();
                foreach (string url in siteMap)
                {
                    doc.Load(url);
                    XmlNodeList nodeList = doc.GetElementsByTagName(tagName);
                    foreach (XmlNode node in nodeList)
                        allUrls.Add(node.InnerText);
                }
            }
            return allUrls;
        }

        public static List<string> getDisAllowDir()
        {
            if (DisAllowDir.Count == 0)
                initialize();
            return DisAllowDir;
        }

        public static List<string> getSiteMap()
        {
            if (siteMap.Count == 0)
                initialize();
            return siteMap;
        }
    }
}
