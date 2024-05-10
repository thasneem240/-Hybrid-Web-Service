using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web_Server.Models;

namespace Web_Server.Controllers
{

    [RoutePrefix("api/UpdateCurrentlyDoing")]
    public class UpdateCurrentlyDoingController : ApiController
    {
        private ClientDBEntities1 db = new ClientDBEntities1();
        [Route("{ip}/{busy}")]
        public IHttpActionResult PutConnected(string ip, string busy)
        {

            var base64EncodedBytes = System.Convert.FromBase64String(ip);
            string ID = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            Client client = db.Clients.Find(ID);
            db.Clients.Attach(client);
            client.Currently_Doing_Any_Job = busy;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ClientExists(client.IP_Address))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return Ok("Successfully updated");

        }

        private bool ClientExists(string id)
        {
            return db.Clients.Count(e => e.IP_Address == id) > 0;
        }
    }
}
