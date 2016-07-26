namespace InvoiceGenerator.Entities
{
  public class Customer : BaseEntity
  {
    public string CustomerName { get; set; }
    public string ContactName { get; set; }
    public Address Address { get; set; }
    public CustomerTax Tax { get; set; }
    public string Comments { get; set; }
    public string Currency { get; set; }
    public string Email { get; set; }
  }

  public class Bank
  {
    public string Beneficiary { get; set; }
    public string Branch { get; set; }
    public string BankName { get; set; }
    public string AccountNo { get; set; }
    public string Code { get; set; }
    //}
    //  return $"Beneficiary: {Beneficiary}, Branch: {Branch}, Bank Name: {BankName}, Account No.: {AccountNo}, Code: {Code}";
    //{

    //public override string ToString()
  }
}