using System.Net.Mail;

namespace InvoiceGenerator
{
  public interface INotificationService
  {
    bool Send(string message, string subject, string[] attachments, params MailAddress[] to);
  }
}