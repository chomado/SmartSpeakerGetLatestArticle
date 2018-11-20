using SmartSpeakerGetLatestArticle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TechSummit2018.ServerlessSmartSpeaker.Services
{
    public class ChomadoBlogService
    {
        private static string ChomadoBlogRssUrl { get; } = "https://chomado.com/author/chomado/feed/";
        private static HttpClient HttpClient { get; } = new HttpClient();
        public async Task<Blog> GetLatestBlogAsync()
        {
            var response = await HttpClient.GetAsync(ChomadoBlogRssUrl);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var xml = XDocument.Parse(await response.Content.ReadAsStringAsync());
            // var title = xml.Descendants("item").FirstOrDefault()?.Element("title")?.Value;
            var item = xml.Descendants("item").FirstOrDefault();
            if (item == null)
            {
                return null;
            }

            return new Blog
            {
                Title = item.Element("title").Value,
                Url = item.Element("link").Value,
            };
        }
    }
}
