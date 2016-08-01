using System.Collections.Generic;

namespace InvoiceGenerator.Entities
{
  public class Customer : BaseEntity
  {
    public string CustomerName { get; set; }
    public string ContactName { get; set; }
    public Address Address { get; set; }
    public IDictionary<string, decimal> Taxes { get; set; }
    public string Comments { get; set; }
    public string Currency { get; set; }
    public string Email { get; set; }
  }
}