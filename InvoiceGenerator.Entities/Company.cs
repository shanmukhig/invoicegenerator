using FluentValidation.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace InvoiceGenerator.Entities
{
  [Validator(typeof(CompanyValidator))]
  public class Company : BaseEntity
  {
    public string Currency { get; set; }
    public string CompanyName { get; set; }
    public Address Address { get; set; }
    public CompanyTax Tax { get; set; }
    public Bank Ifsc { get; set; }
    public Bank Swift { get; set; }
    public Bank Cheque { get; set; }
    public Support Support { get; set; }
    public string Comments { get; set; }
    public string FileId { get; set; }
    //[BsonIgnore]
    public string Logo { get; set; }
  }
}