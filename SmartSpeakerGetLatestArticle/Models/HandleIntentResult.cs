using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSpeakerGetLatestArticle.Models
{
    public class HandleIntentResult
    {
        public string ResponseMessage { get; set; }
        public Blog Blog { get; set; }
    }

    public class Blog
    {
        public string Url { get; set; }
        public string Title { get; set; }
    }
}
