using System;
using System.Linq;
using HtmlAgilityPack;

namespace PluralsightCertificateDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = args.First();

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(filename);
            var links = htmlDoc.DocumentNode
                  .Descendants("a")
                  .Where(x => x.Attributes["class"] != null 
                           && x.Attributes["class"].Value == "certificateIcon---H3J5h");

            foreach (var link in links)
            {
                Console.WriteLine(link.Attributes["href"].Value);
                var divs = link.ParentNode.ParentNode.Descendants("div");
                foreach (var div in divs)
                {
                    if (div.Descendants("time").Any()) {
                        Console.WriteLine(div.InnerHtml);
                    }
                }
                Console.WriteLine("##############");
            }

            // string hrefValue = links.First().Attributes["href"].Value;
            // long playerId = Convert.ToInt64(hrefValue.Split('=')[1]);
        }
    }
}
