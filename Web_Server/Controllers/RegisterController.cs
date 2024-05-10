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
    [RoutePrefix("api/Register")]
    public class RegisterController : ApiController
    {
        private ClientDBEntities1 db1 = new ClientDBEntities1();



        [Route("RegisterClient/{ipAddress}/{portNo}/{name}")]
        [Route("RegisterClient")]
        [HttpGet]
        public IHttpActionResult RegisterClient(String ipAddress, int portNo, String name) 
        {
            string success = "0";


            /* Create a Client Object */
            Client client = new Client();

            client.IP_Address = ipAddress;
            client.Port_NO = portNo;
            client.Name = name;

            /* Default Values */
            client.Connected = "NO";
            client.Currently_Doing_Any_Job = "NO";
    
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db1.Clients.Add(client);

            try
            {
                db1.SaveChanges();
                success = "1";   // if success  = 1 then client successfully registered
            }
            catch (DbUpdateException)
            {
                if (ClientExists(client.IP_Address))
                {
                    return Ok(success);
                }
                else
                {
                    throw;
                }
            }


            return Ok(success);
        }



        /* Check whether same ip is exist or not */
        private bool ClientExists(string id)
        {
            return db1.Clients.Count(e => e.IP_Address == id) > 0;
        }

    }
}
