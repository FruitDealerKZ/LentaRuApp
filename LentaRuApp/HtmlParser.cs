using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LentaRuApp
{
    class ArticleHeader
    {
        public string Time { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string Title2 { get; set; }
    }

    class HtmlParser
    {
        public async Task<List<ArticleHeader>> LoadContent()
        {
            WebRequest request = WebRequest.Create(new Uri("http://lenta.ru"));
            WebResponse response = await request.GetResponseAsync();
            Stream data = response.GetResponseStream();
            HtmlDocument doc = new HtmlDocument();
            doc.Load(data, Encoding.UTF8);
            return LoadMainNews(doc);
        }
        
        private List<ArticleHeader> LoadMainNews(HtmlDocument doc)
        {
            IEnumerable<HtmlNode> items = doc.DocumentNode.Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("item"));
            IEnumerable<HtmlNode> articles = doc.DocumentNode.Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("article item"));

            List<ArticleHeader> itemsList = new List<ArticleHeader>();
            List<ArticleHeader> articlesList = new List<ArticleHeader>();
            foreach(HtmlNode node in items)
            {
                itemsList.Add(ParseItem(node));
            }

            foreach(HtmlNode node in articles)
            {
                articlesList.Add(ParseArticle(node));
            }

            return articlesList;
        }

        private ArticleHeader ParseItem(HtmlNode item)
        {
            ArticleHeader art = new ArticleHeader();

            if(item.ChildNodes.LongCount(x => x.Name == "time") > 0)
            {
                art.Time = item.Descendants("time").First().InnerText;
                art.Title = Trim(item.Descendants("a").First().InnerText);
                art.Url = "http://lenta.ru" + item.Descendants("a").First().GetAttributeValue("href", "");
            }
            else
            {
                art.Time = item.Descendants("span").First(x => x.GetAttributeValue("class", "").Equals("time")).InnerText;
                art.Title = Trim(item.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("titles")).InnerText);
                art.Url = "http://lenta.ru" + item.Descendants("a").First().GetAttributeValue("href", "");
            }

            return art;
        }

        private ArticleHeader ParseArticle(HtmlNode node)
        {
            ArticleHeader art = new ArticleHeader();

            art.Time = Trim(node.Descendants("span").First(x => x.GetAttributeValue("class", "").Equals("time")).InnerText);
            if(node.Descendants("img").Count() > 0)
                art.Image = node.Descendants("img").First().GetAttributeValue("src", "");
            art.Title = Trim(node.Descendants("h3").First().InnerText);
            art.Title2 = Trim(node.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("rightcol")).InnerText);
            art.Url = "http://lenta.ru" + node.Descendants("h3").First().Descendants("a").First().GetAttributeValue("href", "");

            return art;
        }

        public static string Trim(string input)
        {
            return input.Trim(new char[] {'\n', ' '});
        }
    }
}
