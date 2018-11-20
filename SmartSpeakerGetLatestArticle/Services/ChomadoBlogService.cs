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
        public async Task<string> GetLatestBlogTitleAsync()
        {
            var response = await HttpClient.GetAsync(ChomadoBlogRssUrl);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var xml = XDocument.Parse(await response.Content.ReadAsStringAsync());
            return xml.Descendants("item").FirstOrDefault()?.Element("title")?.Value;
        }
    }
}
