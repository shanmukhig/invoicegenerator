using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using InvoiceGenerator.Entities;

namespace InvoiceGenerator.UI.Win
{
  public partial class FrmInvoiceGenerator : Form
  {
    //private readonly INotificationService notificationService;
    private readonly string rootPath;
    //private readonly string subject;
    private string fileName;

    public FrmInvoiceGenerator()
    {
      InitializeComponent();
      //notificationService = new EmailNotificationService(new Settings
      //{
      //  From = ConfigurationManager.AppSettings["From"],
      //  Server = ConfigurationManager.AppSettings["Server"],
      //  Port = ConfigurationManager.AppSettings["Port"],
      //  UseSsl = ConfigurationManager.AppSettings["UseSsl"]
      //});
      //subject = ConfigurationManager.AppSettings["Subject"];
      //rootPath = ConfigurationManager.AppSettings["RootPath"];
      rootPath = Directory.GetCurrentDirectory();
      //lblStatus.Text = rootPath;
      //lblProgress.Text = rootPath;
    }

    private void btnGenerate_Click(object sender, EventArgs e)
    {
      IDataProvider dataProvider = new ExcelDataProvider(fileName);
      var invoices = dataProvider.ReadInvoices();
      IEnumerable<Customer> customers = dataProvider.ReadCustomers().ToList();
      IEnumerable<Company> companies = dataProvider.ReadCompanies().ToList();

      var processor = new InvoiceProcessor(dataProvider, rootPath);

      foreach (var invoice in invoices)
      {
        var customer = customers.Single(x => x.Id == invoice.CustomerId);

        processor.GeneratePdf(invoice, companies.Single(x => x.Id == invoice.CompanyId), customer);

        MessageBox.Show("Successflly completed generating all invoices", "Status", MessageBoxButtons.OK, MessageBoxIcon.Information);

        //bool send = this.notificationService.Send("", string.Format(subject, invoice.BillDate.Month), new string[] {invoiceFile},
        //  new MailAddress(customer.Email, customer.CustomerName));
      }
    }

    private void btnSelectFile_Click(object sender, EventArgs e)
    {
      var fileDialog = new OpenFileDialog
      {
        RestoreDirectory = true,
        Title = "Select Excel file",
        Filter = "Excel Files|*.xlsx;",
        CheckFileExists = true,
        CheckPathExists = true,
        Multiselect = false
      };
      fileDialog.ShowDialog();
      fileName = fileDialog.FileName;
    }
  }
}