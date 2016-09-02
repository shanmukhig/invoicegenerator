using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace InvoiceGenerator.Entities
{
    public enum BillingFrequency
  {
    Other = 0,
    Monthly = 1,
    Quarterly = 3,
    HalfYearly = 6,
    Yearly = 12
  }

  public class Invoice : BaseEntity
  {
    public string InvoiceNo { get; set; }
    public DateTime DueDate { get; set; }
    public IEnumerable<Bill> Bills { get; set; }
    public string CustomerId { get; set; }
    public string CompanyId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime BillDate { get; set; }
    public decimal PreviousAmt { get; set; } //A
    public decimal PreviousPayment { get; set; } //B
    public decimal Adjustments { get; set; } //C
    public string Currency { get; set; }

    [BsonIgnore]
    public string FileId { get; set; }

    public byte[] PdfStream { get; set; }

    [BsonIgnore]
    public string SummaryOfAccounts { get; set; }

    [BsonIgnore]
    public decimal CurrentCharges { get; set; }

    //public Invoice Invoice { get; }
    [BsonIgnore]
    public string Taxes { get; set; }

    [BsonIgnore]
    public decimal TotalCharges { get; set; }

    [BsonIgnore]
    public decimal BalanceCarryForward { get; set; }

    //private decimal totalDue;

    public decimal TotalDue { get; set; }

    [BsonIgnore]
    public string InWords { get; set; }

    //public void ProcessInvoice(Customer customer, Company company, IList<Product> products)
    //{
    //  if (customer == null || company == null || !products.Any())
    //  {
    //    return;
    //  }

    //  DateTime dt = DateTime.UtcNow;

    //  this.InvoiceNo =
    //    $"{customer.CustomerName.Substring(0, 5)}-{dt.ToString("yyyyMMdd")}-{(dt.Ticks>>5).ToString().Substring(0, 5)}"
    //      .ToUpper();

    //  //this.Currency = customer.Currency;
    //}
  }

  public class Bill
  {
    //public string InvoiceId { get; set; }
    [BsonIgnore]
    public string ProductName { get; set; }
    public string ProductId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Quantity { get; set; }
    public int BillingFrequency { get; set; }
    public decimal? Price { get; set; }
  }
}