using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Web_Server.Models;

namespace Web_Server.Controllers
{
    public class ClientsController : ApiController
    {
        private ClientDBEntities1 db = new ClientDBEntities1();

        // GET: api/Clients
        public IQueryable<Client> GetClients()
        {
            return db.Clients;
        }

        // GET: api/Clients/5
        [ResponseType(typeof(Client))]
        public IHttpActionResult GetClient(string id)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(id);
            string ID= System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

            Client client = db.Clients.Find(ID);
            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        // PUT: api/Clients/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutClient(string id, Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != client.IP_Address)
            {
                return BadRequest();
            }

            db.Entry(client).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Clients
        [ResponseType(typeof(Client))]
        public IHttpActionResult PostClient(Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Clients.Add(client);

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

            return CreatedAtRoute("DefaultApi", new { id = client.IP_Address }, client);
        }

        // DELETE: api/Clients/5
        [ResponseType(typeof(Client))]
        public IHttpActionResult DeleteClient(string id)
        {
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            db.Clients.Remove(client);
            db.SaveChanges();

            return Ok(client);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClientExists(string id)
        {
            return db.Clients.Count(e => e.IP_Address == id) > 0;
        }
    }
}