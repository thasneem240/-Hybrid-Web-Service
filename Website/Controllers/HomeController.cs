using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using Website.Models;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Home";
            string URL = "http://localhost:11662/";
            RestClient restClient = new RestClient(URL);
            RestRequest restRequest = new RestRequest("api/Clients", Method.Get);
            RestResponse restResponse = restClient.Execute(restRequest);

            List<Clients> clientList = JsonConvert.DeserializeObject<List<Clients>>(restResponse.Content);

            return View(clientList);
       
        }
    }
}
