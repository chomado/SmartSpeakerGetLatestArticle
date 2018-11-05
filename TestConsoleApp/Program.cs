using System;
using TechSummit2018.ServerlessSmartSpeaker.Services;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = new ChomadoBlogService();
            Console.WriteLine(s.GetNewestBlogTitleAsync().Result);
        }
    }
}
