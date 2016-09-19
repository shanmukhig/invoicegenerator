using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InvoiceGenerator.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace InvoiceGenerator.Data
{
  public interface IUserRepository : IRepository<User>
  {
    Task<User> GetByName(string name);
  }

  public class UserRepository : Repository<User>, IUserRepository
  {
    public UserRepository(string connectionString, string databaseName) : base(connectionString, databaseName)
    {
    }

    public async Task<User> GetByName(string name)
    {
      IAsyncCursor<User> cursor =
        await Collection.FindAsync(Builders<User>.Filter.Where(arg => arg.Username == name)).ConfigureAwait(false);
      return await cursor.SingleOrDefaultAsync().ConfigureAwait(false);
    }
  }

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
    }

    public IMongoCollection<T> Collection { get; }

    public async Task<IEnumerable<T>> GetAll()
    {
      IAsyncCursor<T> document = await Collection.FindAsync(new BsonDocument()).ConfigureAwait(false);
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

    public async Task<string> UploadFileAsync(string id, string fileName, Stream stream = null, byte[] buffer = null)
    {
      GridFSUploadOptions options = new GridFSUploadOptions
      {
        ChunkSizeBytes = 1024,
        Metadata = new BsonDocument("type", "stream")
      };

      GridFSBucket gridFsBucket = new GridFSBucket(this.Collection.Database);

      ObjectId objectId = ObjectId.Empty;

      try
      {
        await gridFsBucket.DeleteAsync(ObjectId.Parse(id));
      }
      catch
      {
        //TODO:may be record not found.
      }

      if (buffer != null)
      {
        objectId = await gridFsBucket.UploadFromBytesAsync(fileName, buffer, options);
      }
      else if (stream != null)
      {
          stream.Position = 0;
          objectId = await gridFsBucket.UploadFromStreamAsync(fileName, stream, options);
      }
      
      return objectId.ToString();
    }

    public async Task<byte[]> DownloadFileAsync(string id)
    {
      GridFSBucket gridFsBucket = new GridFSBucket(this.Collection.Database);
      return await gridFsBucket.DownloadAsBytesAsync(ObjectId.Parse(id));
    }
  }
}