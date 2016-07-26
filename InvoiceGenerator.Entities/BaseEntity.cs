using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace InvoiceGenerator.Entities
{
  public class BaseEntity
  {
    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    public string Id { get; set; }
  }
}