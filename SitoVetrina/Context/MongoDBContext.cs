using Microsoft.Data.SqlClient;
using System.Data;
using MongoDB.Driver;
using SitoVetrina.Models;
using MongoDB.Bson;

namespace SitoVetrina.Context
{
    public class MongoDBContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly IMongoDatabase _database;
        public MongoDBContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SitoVetrinaContextConnectionMongo");
            MongoClient mongoClient= new MongoClient(_connectionString);
            _database = mongoClient.GetDatabase("SitoVetrina");
        }

        public IMongoDatabase TakeDatabase() => _database;
    }
}
