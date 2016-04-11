using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var httpHandler = new HttpClientHandler()
            {
                UseProxy = true,
                UseCookies = false,
                Proxy = new WebProxy("127.0.0.1:8080")
            };
            var http = new HttpClient(httpHandler);

            http.DefaultRequestHeaders.Host = "api.baa.bitauto.com";
            
            HttpResponseMessage response = http.PostAsXmlAsync<string>("http://127.0.0.1:80", "").Result;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}