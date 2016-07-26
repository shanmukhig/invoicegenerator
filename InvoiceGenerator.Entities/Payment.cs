using System;

namespace InvoiceGenerator.Entities
{
  public class Payment : BaseEntity
  {
    public DateTime PaymentDate { get; set; }
    public string Comments { get; set; }
    public string InvoiceNo { get; set; }
    public string Currency { get; set; }
    public decimal Amount { get; set; }
  }
}