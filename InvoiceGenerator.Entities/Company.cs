namespace InvoiceGenerator.Entities
{
  public class Company : BaseEntity
  {
    public string Currency { get; set; }
    public string CompanyName { get; set; }
    public Address Address { get; set; }
    public CompanyTax Tax { get; set; }
    public Bank Ifsc { get; set; }
    public Bank Swift { get; set; }
    public Bank Cheque { get; set; }
    public string Logo { get; set; }
    public Support Support { get; set; }
    public string Comments { get; set; }
  }

  public class Support
  {
    public string Phone { get; set; }
    public string Email { get; set; }
  }

  public class CompanyTax
  {
    public string ServiceTaxNo { get; set; }
    public string Tin { get; set; }
    public string Pan { get; set; }
    public string Cin { get; set; }
  }
}