using HtmlAgilityPack;
using LentaRuApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace LentaApp
{
    class Article
    {
        public string Time { get; set; }
        public string Title { get; set; }
        public string Title2 { get; set; }
        public ImageDescription Image { get; set; }
        public FlowDocument Content { get; set; }
    }

    class ImageDescription
    {
        public string Url { get; set; }
        public string Caption { get; set; }
        public string Credits { get; set; }
    }

    class ContentParser
    {
        public async void OpenUrl(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = await request.GetResponseAsync();
            Stream data = response.GetResponseStream();
            HtmlDocument doc = new HtmlDocument();
            doc.Load(data);


        }

        private void LoadContent(HtmlDocument doc)
        {
            HtmlNode content = doc.DocumentNode.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("b-topic__content"));
            HtmlNode header = content.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("b-topic__header"));
            HtmlNode topic = header.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("b-topic__info"));
            Article art = new Article();
            art.Time = HtmlParser.Trim(topic.Descendants("time").First().InnerText);
            art.Title = HtmlParser.Trim(header.Descendants("h1").First(x => x.GetAttributeValue("class", "").Equals("b-topic__title")).InnerText);
            art.Title2 = HtmlParser.Trim(header.Descendants("h2").First(x => x.GetAttributeValue("class", "").Equals("b-topic__rightcol")).InnerText);
            ImageDescription imageDescr = new ImageDescription();
            HtmlNode image = header.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("b-topic__title-image"));
            imageDescr.Url = image.GetAttributeValue("src", "");
            imageDescr.Caption = image.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("b-label__caption")).InnerText;
            imageDescr.Credits = image.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("b-label__credits")).InnerText;
            art.Image = imageDescr;

        }
    }
}
