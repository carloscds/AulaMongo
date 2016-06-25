using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //var mongoDB_MongoLab = "mongodb://usuario:senha@ds061158.mongolab.com:61158/teste";
            var mongoDB_Local = "mongodb://localhost";

            var mongoDB = mongoDB_Local;

            var client = new MongoClient(mongoDB);

            var db = client.GetDatabase("teste");

            var clienteCollection = db.GetCollection<Cliente>("cliente");

            var novoCliente = new Cliente() { Nome = "Carlos" };
            clienteCollection.InsertOne(novoCliente);
            var id = novoCliente.Id;

            var filter = Builders<Cliente>.Filter.Eq("Nome", "Carlos");

            var cli1 = clienteCollection.Find<Cliente>(filter).FirstOrDefault();
            if (cli1 != null)
            {
                var update = Builders<Cliente>.Update.Set("Nome", "Carlos dos Santos");
                clienteCollection.UpdateOne(filter, update);
            }

            var dados = clienteCollection.Find<Cliente>(new BsonDocument()).ToListAsync().Result;

            foreach (var c in dados)
            {
                Console.WriteLine("{0} - {1}",c.Id,c.Nome);
            }
        }
    }

    public class Cliente
    {
        public ObjectId Id { get; set; }
        public string Nome { get; set; }
    }

}
