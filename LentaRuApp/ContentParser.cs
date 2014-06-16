using HtmlAgilityPack;
using LentaRuApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;

namespace LentaApp
{
    class Article
    {
        public string Time { get; set; }
        public string Title { get; set; }
        public string Title2 { get; set; }
        public ImageDescription Image { get; set; }
        public List<Block> Blocks { get; set; }
    }

    class ImageDescription
    {
        public string Url { get; set; }
        public string Caption { get; set; }
        public string Credits { get; set; }
    }

    class ContentParser
    {
        public async Task<Article> OpenUrl(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = await request.GetResponseAsync();
            Stream data = response.GetResponseStream();
            HtmlDocument doc = new HtmlDocument();
            doc.Load(data, Encoding.UTF8);
            return LoadContent(doc);
        }

        private Article LoadContent(HtmlDocument doc)
        {
            HtmlNode content = doc.DocumentNode.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("b-topic__content"));
            HtmlNode header = content.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("b-topic__header"));
            HtmlNode topic = header.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("b-topic__info"));
            Article art = new Article();
            art.Time = HtmlParser.Trim(topic.Descendants("time").First().InnerText);
            art.Title = HtmlParser.Trim(header.Descendants("h1").First(x => x.GetAttributeValue("class", "").Equals("b-topic__title")).InnerText);
            if(header.Descendants("h2").Count() > 0)
                art.Title2 = HtmlParser.Trim(header.Descendants("h2").First(x => x.GetAttributeValue("class", "").Equals("b-topic__rightcol")).InnerText);
            ImageDescription imageDescr = new ImageDescription();
            HtmlNode image = header.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("b-topic__title-image"));
            imageDescr.Url = image.Descendants("img").First().GetAttributeValue("src", "");
            if (image.Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("b-label__caption")).Count() == 1)
                imageDescr.Caption = image.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("b-label__caption")).InnerText;
            if (image.Descendants("div").Where(x => x.GetAttributeValue("class", "").Equals("b-label__credits")).Count() == 1)
            imageDescr.Credits = image.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("b-label__credits")).Descendants("div").First().InnerText;
            art.Image = imageDescr;

            HtmlNode text = content.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("b-text clearfix"));
            List<Block> blocks = new List<Block>();

            foreach (HtmlNode node in text.ChildNodes)
            {
                if (node.Name.Equals("p"))
                {
                    Paragraph par = new Paragraph();
                    Run run = new Run();
                    run.Text = node.InnerText;
                    par.Inlines.Add(run);
                    blocks.Add(par);
                }
                if (node.Name.Equals("aside"))
                {
                    string url = node.Descendants("img").First().GetAttributeValue("src", "");
                    Image img = new Image();
                    BitmapImage btmpImg = new BitmapImage(new Uri(url));
                    img.Source = btmpImg;
                    Run caption = new Run();
                    caption.Text = node.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("b-label__caption")).InnerText;
                    //Run credits = new Run();
                    //credits.Text = node.Descendants("div").First(x => x.GetAttributeValue("class", "").Equals("b-label__credits")).InnerText;
                    InlineUIContainer container = new InlineUIContainer();
                    container.Child = img;
                    Paragraph par = new Paragraph();
                    par.Inlines.Add(container);
                    par.Inlines.Add(caption);
                    //par.Inlines.Add(credits);

                    blocks.Add(par);
                }
                if(node.Name.Equals("h1") || node.Name.Equals("h2"))
                {
                    Paragraph par = new Paragraph();
                    Run run = new Run();
                    run.Text = node.InnerText;
                    par.Inlines.Add(run);
                    blocks.Add(par);
                }
            }

            art.Blocks = blocks;

            return art;
        }
    }
}
