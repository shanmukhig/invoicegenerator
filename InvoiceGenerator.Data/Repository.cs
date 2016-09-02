using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InvoiceGenerator.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace InvoiceGenerator.Data
{
  public class Repository<T> : IRepository<T> where T : BaseEntity
  {
    public Repository(string connectionString, string databaseName)
    {
      string name = typeof(T).Name;
      MongoClient mongoClient = new MongoClient(connectionString);
      IMongoDatabase database = mongoClient.GetDatabase(databaseName);

      Collection = database.GetCollection<T>(name);

      if (Collection != null)
      {
        return;
      }

      database.CreateCollection(name);
      Collection = database.GetCollection<T>(name);

      //Collection.DeleteMany(Builders<T>.Filter.Empty);
    }

    public IMongoCollection<T> Collection { get; }

    public async Task<IEnumerable<T>> GetAll()
    {
      IAsyncCursor<T> document = await Collection.FindAsync(new BsonDocument()).ConfigureAwait(false);
      //Collection.Find(new BsonDocument()).Project<T>(Builders<T>.Projection.Exclude(arg => arg.pdfStream))
      return document.ToEnumerable();
    }

    public async Task<T> GetById(string id)
    {
      IAsyncCursor<T> cursor =
        await Collection.FindAsync(Builders<T>.Filter.Where(arg => arg.Id == id)).ConfigureAwait(false);
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
      DeleteResult result =
        await Collection.DeleteOneAsync(Builders<T>.Filter.Where(arg => arg.Id == id)).ConfigureAwait(false);
      return result.DeletedCount > 0;
    }

    public async Task<string> UploadFile(string fileName, Stream stream)
    {
      IGridFSBucket bucket = new GridFSBucket(Collection.Database);

      ObjectId objectId = await bucket.UploadFromStreamAsync(fileName, stream);
      return objectId.ToString();
    }
  }
}