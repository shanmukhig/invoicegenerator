using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceGenerator.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InvoiceGenerator.Data
{
  public interface IRepository<T> where T : BaseEntity
  {
    Task<IEnumerable<T>> GetAll();
    Task<T> GetById(string id);
    Task<T> AddOrUpdate(string id, T customer);
    Task<bool> Delete(string id);
  }

  public class Repository<T> : IRepository<T> where T : BaseEntity
  {
    public Repository(string connectionString, string databaseName)
    {
      var name = typeof(T).Name;
      var mongoClient = new MongoClient(connectionString);
      var database = mongoClient.GetDatabase(databaseName);

      Collection = database.GetCollection<T>(name);

      if (Collection == null)
      {
        database.CreateCollection(name);
        Collection = database.GetCollection<T>(name);
      }

      //Collection.DeleteMany(Builders<T>.Filter.Empty);
    }

    public IMongoCollection<T> Collection { get; }

    public async Task<IEnumerable<T>> GetAll()
    {
      //await this.Collection.Find(Builders<T>.Filter.Empty).ToListAsync();

      var document = await Collection.FindAsync(new BsonDocument()).ConfigureAwait(false);
      return document.ToEnumerable();
      ;
    }

    public async Task<T> GetById(string id)
    {
      var cursor = await Collection.FindAsync(Builders<T>.Filter.Where(arg => arg.Id == id)).ConfigureAwait(false);
      return await cursor.SingleOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<T> AddOrUpdate(string id, T customer)
    {
      if (!string.IsNullOrWhiteSpace(id))
      {
        await Collection.ReplaceOneAsync(arg => arg.Id == id, customer).ConfigureAwait(false);
      }
      else
      {
        await Collection.InsertOneAsync(customer);
      }
      return customer;
    }

    public async Task<bool> Delete(string id)
    {
      var result = await Collection.DeleteOneAsync(Builders<T>.Filter.Where(arg => arg.Id == id)).ConfigureAwait(false);
      return result.DeletedCount > 0;
    }
  }
}