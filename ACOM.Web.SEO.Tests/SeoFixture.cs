namespace ACOM.Web.SEO.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net.Http;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SeoFixture
    {
        [TestMethod]
        [TestCategory("Sitemap")]
        [Description("Gets the EN-US sitemap and checks that all URLs under documentation/videos contain the corresponding video element tags")]
        public void VideoDetailPagesShouldHaveVideoMetadataInSitemap()
        {
            var mainSitemapUrl = string.Format("{0}/en-us/robotsitemap/", ConfigurationManager.AppSettings["EnvironmentUrl"]);

            var client = new HttpClient();
            client.DefaultRequestHeaders.IfModifiedSince = DateTimeOffset.UtcNow;

            XDocument sitemapXml;

            try
            {
                var sitemapContent = client.GetStringAsync(mainSitemapUrl).Result;
                sitemapXml = XDocument.Parse(sitemapContent);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred trying to get the sitemap", ex);
            }

            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XNamespace nsVideo = "http://www.google.com/schemas/sitemap-video/1.1";

            var videoLocElements = sitemapXml.Descendants(ns + "loc")
                    .Where(n => n.Value.Contains("/documentation/videos/"));

            var urlsWithErrors = new List<Tuple<string, List<string>>>();
            foreach (var videoLocElement in videoLocElements)
            {
                var loc = videoLocElement.Value;

                if (this.ShouldSkipVideoUrl(loc))
                {
                    continue;
                }

                var errors = new List<string>();

                var videoElements = videoLocElement.Parent.Descendants(nsVideo + "video").ToList();
                if (!videoElements.Any())
                {
                    errors.Add("No <video> elements were found");
                }
                else
                {
                    var childVideoElements = videoElements.Descendants().ToList();
                    this.CheckAddError(childVideoElements, "thumbnail_loc", errors);
                    this.CheckAddError(childVideoElements, "title", errors);
                    this.CheckAddError(childVideoElements, "description", errors);
                    this.CheckAddError(childVideoElements, "player_loc", errors);
                    this.CheckAddError(childVideoElements, "duration", errors);
                }

                if (errors.Any())
                {
                    urlsWithErrors.Add(new Tuple<string, List<string>>(loc, errors));
                }
            }

            if (urlsWithErrors.Any())
            {
                Assert.Fail(this.GetUrlsWithErrorsMessage(urlsWithErrors));
            }
        }

        private bool ShouldSkipVideoUrl(string loc)
        {
            return loc.EndsWith("/documentation/videos/home/") ||
                   loc.EndsWith("/documentation/videos/index/") ||
                   loc.EndsWith("/documentation/videos/azure-friday/") ||
                   loc.Contains("/documentation/videos/playlists/");
        }

        private string GetUrlsWithErrorsMessage(List<Tuple<string, List<string>>> urlsWithErrors)
        {
            var result = string.Empty;

            foreach (var url in urlsWithErrors)
            {
                result += string.Format("Errors in URL {0}: {1}", url.Item1, string.Join(" - ", url.Item2)) + Environment.NewLine;
            }

            return result;
        }

        private void CheckAddError(List<XElement> videoElements, string elementName, List<string> errors)
        {
            XNamespace nsVideo = "http://www.google.com/schemas/sitemap-video/1.1";

            if (!videoElements.Any(n => n.Name.Equals(nsVideo + elementName)))
            {
                errors.Add(string.Format("Element <{0}> is missing", elementName));
            }
        }
    }
}
