using FluentValidation.Attributes;

namespace InvoiceGenerator.Entities
{
  [Validator(typeof(ProductValidator))]
  public class Product : BaseEntity
  {
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Comments { get; set; }
    public string Currency { get; set; }
    public string CountryCode { get; set; }
  }
}