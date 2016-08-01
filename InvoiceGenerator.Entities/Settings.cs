namespace InvoiceGenerator.Entities
{
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