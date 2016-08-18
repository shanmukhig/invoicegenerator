using System.Collections.Generic;

namespace InvoiceGenerator.Entities
{
  public class Customer : BaseEntity
  {
    public string CustomerName { get; set; }
    public string ContactName { get; set; }
    public Address Address { get; set; }
    public IEnumerable<Tax> Taxes { get; set; }
    public string Comments { get; set; }
    public string Currency { get; set; }
  }

  public class Tax
  {
    public string Name { get; set; }
    public decimal Percent { get; set; }
  }
}