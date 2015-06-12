using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace Azure.Automation.Helpers
{
    public class JsonHelper
    {
        public static T ExtractDataFromJson<T>(string url)
        {
            //extract json data to object
            string jsonURI = TestConfiguration.Instance.EnvironmentUrl + url;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(jsonURI);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sreader = new StreamReader(response.GetResponseStream());
            string jsonstr = sreader.ReadToEnd();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(jsonstr);
        }
    }
}
