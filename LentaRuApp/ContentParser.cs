using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LentaApp
{
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
    }
}
