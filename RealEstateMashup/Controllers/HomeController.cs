using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using RealEstateMashup.Models;

namespace RealEstateMashup.Controllers
{
    public class HomeController : Controller
    {
        public string HudUrl = "http://www.hudhomestore.com/Listing/PropertySearchResult.aspx?pageId=1&zipCode=&city=&county={0}&sState=SC&fromPrice=0&toPrice=0&fCaseNumber=&bed=0&bath=0&street=&buyerType=0&specialProgram=&Status=0&sPageSize=10&OrderbyName=DLISTPRICE&OrderbyValue=ASC&sLanguage=ENGLISH";
        public string HomePathUrl = "http://www.homepath.com/listing/search?loc={0}%20County,%20SC&pg=1&o=p&ob=asc&ps=25";

        public ActionResult Index()
        {
            var model = new ForeclosuresModel();

            // Florence
            var florenceHud = GetWebResponse(string.Format(HudUrl, "Florence"));
            ProcessHudResponse(florenceHud, model.FlorenceProperties);

            var florenceHomePath = GetWebResponse(string.Format(HomePathUrl, "Florence"));
            ProcessHomePathResponse(florenceHomePath, model.FlorenceProperties);

            // Darlington
            var darlingtonHud = GetWebResponse(string.Format(HudUrl, "Darlington"));
            ProcessHudResponse(darlingtonHud, model.DarlingtonProperties);

            var darlingtonHomePath = GetWebResponse(string.Format(HomePathUrl, "Darlington"));
            ProcessHomePathResponse(darlingtonHomePath, model.DarlingtonProperties);

            // Horry
            var horryHud = GetWebResponse(string.Format(HudUrl, "Horry"));
            ProcessHudResponse(horryHud, model.HorryProperties);

            var horryHomePath = GetWebResponse(string.Format(HomePathUrl, "Horry"));
            ProcessHomePathResponse(horryHomePath, model.HorryProperties);

            // Charleston
            var charlestonHud = GetWebResponse(string.Format(HudUrl, "Charleston"));
            ProcessHudResponse(charlestonHud, model.CharlestonProperties);

            var charlestonHomePath = GetWebResponse(string.Format(HomePathUrl, "Charleston"));
            ProcessHomePathResponse(charlestonHomePath, model.CharlestonProperties);

            return View(model);
        }

        private static string GetWebResponse(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Real Estate Mashup";

                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream == null)
                            return null;

                        using (var reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd().Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }

        private static void ProcessHudResponse(string response, IList<ForeclosureModel> foreclosures)
        {
            if (response == null) return;

            var doc = new HtmlDocument();
            doc.LoadHtml(response);

            foreach (var row in doc.DocumentNode.SelectNodes("//table[@id='dgPropertyList']/tr[@class='FormTablerow']"))
            {
                var onclickAttribute = row.ChildNodes[1].SelectSingleNode("a").GetAttributeValue("onclick", string.Empty);
                foreclosures.Add(new ForeclosureModel
                    {
                        Photo = "http://www.hudhomestore.com" + row.ChildNodes[1].SelectSingleNode("a/img").GetAttributeValue("src", string.Empty).Substring(2),
                        DetailPage = onclickAttribute.Substring(onclickAttribute.IndexOf("top.location.href") + 21, onclickAttribute.Length - 23),
                        Address = row.ChildNodes[3].SelectSingleNode("span").InnerHtml,
                        Price = row.ChildNodes[4].SelectSingleNode("label/span").InnerHtml,
                        Beds = row.ChildNodes[6].SelectSingleNode("label").InnerHtml,
                        Baths = row.ChildNodes[7].SelectSingleNode("label").InnerHtml,
                        BiddingRestricted = row.ChildNodes[8].SelectSingleNode("label/span/b/span").InnerHtml.ToLower().Contains("exclusive")
                    });
            }
        }

        private static void ProcessHomePathResponse(string response, IList<ForeclosureModel> foreclosures)
        {
            if (response == null) return;

            var doc = new HtmlDocument();
            doc.LoadHtml(response);

            foreach (var row in doc.DocumentNode.SelectNodes("//tbody[@id='listingRowsHtml']/tr"))
            {
                if (row.ChildNodes.Count < 16 || row.ChildNodes[11].SelectNodes("p/span[@class='activelisting']") == null) continue;

                foreclosures.Add(new ForeclosureModel
                {
                    Photo = row.ChildNodes[1].SelectSingleNode("a/img").GetAttributeValue("src", string.Empty),
                    DetailPage = "http://www.homepath.com" + row.ChildNodes[1].SelectSingleNode("a").GetAttributeValue("href", string.Empty),
                    Address = row.ChildNodes[3].SelectSingleNode("p/a").InnerHtml,
                    Price = "<b>" + row.ChildNodes[5].SelectSingleNode("p").InnerHtml + "</b>",
                    Beds = row.ChildNodes[7].SelectSingleNode("p").InnerHtml.Replace(" br", string.Empty),
                    Baths = row.ChildNodes[9].SelectSingleNode("p").InnerHtml.Replace(" ba", string.Empty),
                    BiddingRestricted = row.ChildNodes[15].SelectSingleNode("p/a/img[@class='firstLookSearch']") != null
                });
            }
        }
    }
}
