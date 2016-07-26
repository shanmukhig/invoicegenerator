namespace InvoiceGenerator.Entities
{
  public class CustomerTax
  {
    public decimal ServiceTax { get; set; }
    public decimal SwatchBharat { get; set; }
    public decimal KrishiKalyan { get; set; }
    public decimal Vat { get; set; }
  }

  public class Settings
  {
    public string UseSsl;
    public string From { get; set; }
    public string Subject { get; set; }
    public string Server { get; set; }
    public string Port { get; set; }
    public string Credentials { get; set; }
  }
}