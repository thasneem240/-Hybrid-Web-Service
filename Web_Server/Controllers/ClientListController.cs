using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web_Server.Models;

namespace Web_Server.Controllers
{
    [RoutePrefix("api/ClientList")]
    public class ClientListController : ApiController
    {
        private ClientDBEntities1 db = new ClientDBEntities1();


        [Route("GetListOfClients")]
        [HttpGet]
        public IQueryable<Client> GetListOfClients()
        {
            return db.Clients;
        }
    }
}
