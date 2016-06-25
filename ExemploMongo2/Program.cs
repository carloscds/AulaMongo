using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExemploMongo2
{
    class Program
    {
        static void Main(string[] args)
        {
            var mongoDB = "mongodb://localhost"; 
            //"mongodb://teste:teste@localhost/aula";

            var client = new MongoClient(mongoDB);

            var db = client.GetDatabase("univem");

            var colecao = db.GetCollection<BsonDocument>("produto");

            var gridFS = new MongoGridFS(new MongoServer(
                new MongoServerSettings {
                    Server = new MongoServerAddress("localhost") }), 
                    "univem", 
                    new MongoGridFSSettings());

            //BsonDocument novo = new BsonDocument();
            //novo["id"] = 1;
            //novo["nome"] = "produto 1";
            //novo["preco"] = 100;
            //colecao.InsertOne(novo);
            //colecao.InsertOne(new BsonDocument() { {"id",2}, {"nome","produto 2"}, { "preco", "200"}, {"saldo",10}});

            //// Imagem
            //var imagem = @"d:\treinamento\mongo\imagem.jpg";
            //var grid = gridFS.Upload(imagem, Path.GetFileName(imagem));
            //colecao.InsertOne(new BsonDocument() { { "id", 3 },
            //    { "nome", "produto 3" },
            //    { "preco", 300 },
            //    { "saldo", 20 },
            //    { "ImagemID", grid.Id } });

            //// ler imagem
            //var filter = Builders<BsonDocument>.Filter.Eq("id", 3);
            //var pro = colecao.Find<BsonDocument>(filter).FirstOrDefault();

            //var idImagem = new ObjectId(pro["ImagemID"].ToString());
            //var dbImagem = gridFS.Find(Query.EQ("_id", idImagem)).FirstOrDefault();

            //var stream = dbImagem.OpenRead();
            //var buffer = new byte[(int)stream.Length];
            //stream.Read(buffer, 0, buffer.Length);
            //stream.Close();

            //var imagemBanco = @"d:\treinamento\mongo\imagem_banco.jpg";
            //if (File.Exists(imagemBanco))
            //{
            //    File.Delete(imagemBanco);
            //}
            //var sw = new FileStream(imagemBanco, FileMode.Create);
            //sw.Write(buffer, 0, buffer.Length);
            //sw.Flush();
            //sw.Close();

            var colecaoCliente = db.GetCollection<Cliente>("cliente");

            //var novoCli = new Cliente { Nome = "Carlos", Cidade = "Cornelio Procopio", UF = "PR", LimiteCredito = 500 };
            //colecaoCliente.InsertOne(novoCli);
            //var novoCli2 = new Cliente { Nome = "João", Cidade = "Marilia", UF = "SP", LimiteCredito = 500 };
            //colecaoCliente.InsertOne(novoCli2);

            //var mostra = colecaoCliente.AsQueryable();
            //var lista = from c in mostra
            //            where c.UF == "PR"
            //            select c;

            //foreach (var c in lista)
            //{
            //    Console.WriteLine(c.Nome);
            //}
            //return;

            //Console.WriteLine("Aggregate - UF");
            //var match = new BsonDocument { { "UF", "PR" } };
            //var agrega = colecaoCliente.Aggregate().
            //    Match(match);
            //foreach (var c in agrega.ToList<Cliente>())
            //{
            //    Console.WriteLine(c.Nome);
            //}
            //return;

            //var somaConta = new BsonDocument { { "_id", "$UF" },
            //    { "Count", new BsonDocument("$sum", 1) } };
            //var agrega2 = colecaoCliente.Aggregate()
            //    .Group(somaConta)
            //    .ToList();

            //Console.WriteLine("Aggregate - Conta Cliente");
            //foreach (var c in agrega2)
            //{
            //    Console.WriteLine("{0} - {1}", c[0], c[1]);
            //}
            //return;

            //var somaProduto = new BsonDocument {
            //    { "_id", "$nome" },
            //    { "Soma", new BsonDocument("$sum", "$preco") } };
            //var agrega3 = colecao.Aggregate()
            //    .Group(somaProduto)
            //    .ToList();
            //Console.WriteLine("Aggregate - Soma Produto");
            //foreach (var c in agrega3.ToList<BsonDocument>())
            //{
            //    Console.WriteLine("{0} - {1}", c[0], c[1]);
            //}
            //return;


            // MapReduce
            string map = @"
                function() {
                    var cli = this;
                    emit(cli.Nome, { count: 1, 
                    Limite: cli.LimiteCredito });
                }";

            string reduce = @"
                function(key, values) {
                    var result = {count: 0, Limite: 0 };

                    values.forEach(function(value){
                        result.count += value.count;
                        result.Limite += value.Limite;
                    });

                    return result;
                }";
            string finalize = @"
                function(key, value){

                  value.average = value.Limite / value.count;
                  return value;

                }";
            var options = new MapReduceOptions<Cliente, BsonDocument>();
            options.Finalize = finalize;
            options.OutputOptions = MapReduceOutputOptions.Inline;
            var results = colecaoCliente.MapReduce<BsonDocument>(map, 
                reduce, options);

        }
    }

    public class Cliente
    {
        public ObjectId  Id { get; set; }
        public string Nome { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public double LimiteCredito { get; set; }
    }
}
