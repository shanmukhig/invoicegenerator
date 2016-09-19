using System;
using FluentValidation.Attributes;

namespace InvoiceGenerator.Entities
{
  [Validator(typeof(PaymentValidator))]
  public class Payment : BaseEntity
  {
    public string InvoiceId { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Comments { get; set; }
    public string InvoiceNo { get; set; }
    public string Currency { get; set; }
    public decimal Amount { get; set; }
  }
}