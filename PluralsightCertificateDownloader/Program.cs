using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace PluralsightCertificateDownloader
{
    class Program {
        static void Main(string[] args)
        {
            var filename = args[0]; // HTML Filename
            var targetFolder = args[1]; // Target Folder with Trailing Slash
            var jwtToken = args[2]; // JWT Access Token

            // DownloadFile(jwtToken, "https://app.pluralsight.com/library/history", targetFolder + "history.html");

            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(filename);
            var links = htmlDoc.DocumentNode
                .Descendants("a")
                .Where(x => x.Attributes["class"] != null &&
                    x.Attributes["class"].Value == "certificateIcon---H3J5h");

            foreach(var link in links) 
            {
                var dateFinished = new DateTime();

                var divs = link.ParentNode.ParentNode.Descendants("div");
                foreach (var div in divs)
                {
                    if (div.Descendants("time").Any())
                    {
                        Debug.WriteLine(div.InnerHtml);

                        foreach (var attr in div.ChildNodes.First().Attributes.Where(a => a.Name == "datetime"))
                        {
                            if (DateTime.TryParseExact(
                                attr.Value, 
                                "MM-dd-yyyy", 
                                CultureInfo.InvariantCulture, 
                                DateTimeStyles.None, 
                                out dateFinished))
                            {
                                break;
                            }
                        }
                    }
                }

                var linkToCertificate = link.Attributes["href"].Value;
                Debug.WriteLine(linkToCertificate);

                var fileName = linkToCertificate
                    .Replace("https://app.pluralsight.com/learner/user/courses/", "")
                    .Replace("/certificate", ".pdf");
                var fullFileName = targetFolder + dateFinished.ToString("yyyy-MM-dd") + "-" + fileName;

                if (!File.Exists(fullFileName)) 
                {
                    DownloadFile(jwtToken, linkToCertificate, fullFileName);

                    Thread.Sleep(3000);
                }

                Console.WriteLine(fullFileName + " is done.");
            }

            Console.WriteLine("############## Done !");
        }

        public static void DownloadFile(string jwtToken, string linkToCertificate, string fullFileName)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(linkToCertificate);
            request.Headers.Add("pragma", "no-cache");
            request.Headers.Add("cache-control", "no-cache");
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(
                new Uri("https://app.pluralsight.com"),
                new Cookie("PsJwt-production", jwtToken)
            );
            using (var respond = request.GetResponse())
            {
                using (var stream = File.Create(fullFileName))
                {
                    respond.GetResponseStream().CopyTo(stream);
                }
                respond.Close();
            }
        }
    }
}