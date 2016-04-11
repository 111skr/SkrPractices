using Shouldly;
using System.Net;
using System.Net.Http;
using Xunit;

namespace WebApp.Tests
{
    public class LoadHttpTest
    {
        [Fact]
        public void Should_Get_Response_Code_200()
        {
            var httpHandler = new HttpClientHandler()
            {                
                UseProxy = true,
                UseCookies = false,
                Proxy = new WebProxy("127.0.0.1:8080") { UseDefaultCredentials=false }
            };
            var http = new HttpClient(httpHandler);

            http.DefaultRequestHeaders.Host = "skr.com";

            HttpResponseMessage response = http.PostAsXmlAsync<string>("http://127.0.0.1:80", "").Result;

            response.Content.ReadAsAsync<string>().Result.Length.ShouldBeGreaterThan<int>(0);
        }
    }
}
