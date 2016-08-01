using System;
using System.Collections.Generic;

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

        public string FileId { get; set; }
    }

    public class Bill
  {
    //public string InvoiceId { get; set; }
    public string ProductId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Quantity { get; set; }
    public int BillingFrequency { get; set; }
    public decimal? Price { get; set; }
  }
}