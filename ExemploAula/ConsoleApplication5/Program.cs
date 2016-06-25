using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5
{
    class Program
    {
        static void Main(string[] args)
        {
            var hostMongo = "mongodb://usuario:senha@ds023654.mlab.com:23654/exercicio2016";
            var client = new MongoClient(hostMongo);

            var db = client.GetDatabase("exercicio2016");

            var colecao = db.GetCollection<Produto>("CarlosCDS");

            var lp = new List<Produto>();
            lp.Add(new Produto { Nome = "Laranja", NomeGrupo = "Frutas", Saldo = 100, Preco = 5.99 });
            lp.Add(new Produto { Nome = "Banana", NomeGrupo = "Frutas", Saldo = 200, Preco = 3.20 });
            lp.Add(new Produto { Nome = "Maca", NomeGrupo = "Frutas", Saldo = 300, Preco = 7.98 });
            lp.Add(new Produto { Nome = "Alface", NomeGrupo = "Hortalica", Saldo = 70, Preco = 2.65 });
            lp.Add(new Produto { Nome = "Salsinha", NomeGrupo = "Hortalica", Saldo = 20, Preco = 0.75 });
            colecao.InsertMany(lp);

            var filtro = Builders<Produto>.Filter.Empty;
            var totalProdutos = colecao.Count(filtro);
            Console.WriteLine("Total de Produtos: {0}",totalProdutos);

            var filtroSaldo = Builders<Produto>.Filter.Gt("Saldo", 5);
            var listaSaldo = colecao.Find<Produto>(filtroSaldo).ToList();
            Console.WriteLine("\nProdutos com Saldo Maior que 5");
            foreach(var p in listaSaldo)
            {
                Console.WriteLine("{0} - {1}",p.Nome,p.Saldo);
            }

            var filtroPreco = Builders<Produto>.Filter.Gt("Preco", 5.00M);
            var listaPreco = colecao.Find<Produto>(filtroPreco).ToList();
            Console.WriteLine("\nProdutos com Preco maior que 2");
            foreach (var p in listaPreco)
            {
                Console.WriteLine("{0} - {1}", p.Nome, p.Preco);
            }

            var filtroUpdate = Builders<Produto>.Filter.Empty;
            var update = Builders<Produto>.Update.Set("Saldo",100);
            colecao.UpdateMany(filtroUpdate, update);
            Console.WriteLine("\nProdutos atualizados com Saldo = 100");

            var filtroDelete = Builders<Produto>.Filter.Eq("Nome","Laranja");
            colecao.DeleteOne(filtroDelete);
            Console.WriteLine("\nProduto 'Laranja' deletado!");

            var somaSaldo = colecao.AsQueryable();
            var soma = somaSaldo.Sum(p => p.Saldo);
            Console.WriteLine("\nSoma dos Saldos: {0}",soma);

            var somaProduto = new BsonDocument {
                { "_id", "$NomeGrupo" },
                { "Soma", new BsonDocument("$sum", "$Saldo") } };
            var agregaSaldo = colecao.Aggregate()
                .Group(somaProduto)
                .ToList();
            Console.WriteLine("\nSoma dos Saldos dos Grupos de Produtos");
            foreach (var c in agregaSaldo.ToList<BsonDocument>())
            {
                Console.WriteLine("{0} - {1}", c[0], c[1]);
            }
        }
    }

    public class Produto
    {
        public ObjectId Id { get; set; }
        public string Nome { get; set; }
        public string NomeGrupo { get; set; }
        public int Saldo { get; set; }
        public double Preco { get; set; }
    }
}
