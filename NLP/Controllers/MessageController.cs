using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NLP.Controllers
{
    public class MessageController : ApiController
    {
        // GET: api/Message
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Message/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Message
        [HttpPost]
        public String Post([FromBody]Object value)
        {
            DocumentClient client;
            using (client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["documentDBUri"]), ConfigurationManager.AppSettings["documentDBKey"]))
            {
                Database database = ReadOrCreateDatabase(client, "slackBigData");
                DocumentCollection collection = ReadOrCreateCollection(client, database.CollectionsLink, "messages");
                //String jsonString = value.ToString();
                client.CreateDocumentAsync(collection.DocumentsLink, new { messageBody = value }).Wait();

            }
            return String.Empty;
        }

        // PUT: api/Message/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Message/5
        public void Delete(int id)
        {
        }

        private static Database ReadOrCreateDatabase(DocumentClient c, String DatabaseId)
        {
            var db =c.CreateDatabaseQuery()
                            .Where(d => d.Id == DatabaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            if (db == null)
            {
                db = c.CreateDatabaseAsync(new Database { Id = DatabaseId }).Result;
            }

            return db;
        }

        private static DocumentCollection ReadOrCreateCollection(DocumentClient Client, String databaseLink, String CollectionName)
        {
            var col = Client.CreateDocumentCollectionQuery(databaseLink)
                              .Where(c => c.Id == CollectionName)
                              .AsEnumerable()
                              .FirstOrDefault();

            if (col == null)
            {
                var collectionSpec = new DocumentCollection { Id = CollectionName };
                var requestOptions = new RequestOptions { OfferType = "S1" };

                col = Client.CreateDocumentCollectionAsync(databaseLink, collectionSpec, requestOptions).Result;
            }

            return col;
        }
    }
}
