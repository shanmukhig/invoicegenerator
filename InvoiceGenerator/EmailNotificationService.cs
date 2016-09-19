//using System;
//using System.Linq;
//using System.Net;
//using System.Net.Mail;
//using InvoiceGenerator.Entities;

////using log4net;

//namespace InvoiceGenerator
//{
//  public class EmailNotificationService : INotificationService
//  {
//    //private static readonly ILog Logger = LogManager.GetLogger(typeof(EmailNotificationService));

//    private readonly SmtpClient client;
//    //private readonly Settings settings;
//    //private string Subject => settings.Subject;

//    public EmailNotificationService(Settings settings)
//    {
//      this.settings = settings;
//      client = new SmtpClient
//      {
//        Host = settings.Server,
//        Port = Convert.ToInt32(settings.Port),
//        EnableSsl = Convert.ToBoolean(settings.UseSsl),
//        UseDefaultCredentials = false,
//        Credentials = new NetworkCredential(From[1], From[2])
//      };
//    }

//    private string[] From => settings.From.StrSplit();

//    public bool Send(string message, string subject, string[] attachments, params MailAddress[] to)
//    {
//      var mailMessage = new MailMessage(new MailAddress(From[1], From[0]), to.First())
//      {
//        IsBodyHtml = false,
//        Body = message,
//        Subject = subject,
//        From = new MailAddress(From[1], From[0])
//      };

//      foreach (var address in to.Skip(1))
//      {
//        mailMessage.To.Add(address);
//      }

//      try
//      {
//        client.Send(mailMessage);
//        //Logger.Info("Email sent");
//      }
//      catch (Exception)
//      {
//        //Logger.Error($"error while sending wishes mail {ex.Message} {ex.StackTrace}");
//        return false;
//      }
//      return true;
//    }

//    ~EmailNotificationService()
//    {
//      client?.Dispose();
//    }
//  }
//}