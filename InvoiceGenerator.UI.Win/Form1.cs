using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace InvoiceGenerator.UI.Win
{
  public enum BillingFrequency
  {
    Other = 0,
    Monthly = 1,
    Quarterly = 3,
    HalfYearly = 6,
    Yearly = 12
  }

  public class BaseEntity
  {
    public string Id { get; set; }
  }

  public class Company : BaseEntity
  {
    //public string Currency { get; set; }
    public string CompanyName { get; set; }
  }

  public class Customer : BaseEntity
  {
    public string CustomerName { get; set; }
    public Address Address { get; set; }
    public string Currency { get; set; }
    public string ContactName { get; set; }
  }

  public class Address
  {
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string CountryCode { get; set; }
  }

  public class Product : BaseEntity
  {
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Comments { get; set; }
    public string Currency { get; set; }
    public string CountryCode { get; set; }
  }

  public class Payment : BaseEntity
  {
    public string InvoiceId { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Comments { get; set; }
    public string InvoiceNo { get; set; }
    public string Currency { get; set; }
    public decimal Amount { get; set; }
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
    public decimal Adjustments { get; set; }
    public string RefNumber { get; set; }
    public string DiscountDescription { get; set; }
    public decimal Discount { get; set; }

//C
    //public string Currency { get; set; }

    //public string FileId { get; set; }

    //public string SummaryOfAccounts { get; set; }

    //public decimal CurrentCharges { get; set; }
    //public string Taxes { get; set; }
    //public decimal TotalCharges { get; set; }
    //public decimal BalanceCarryForward { get; set; }
    //public decimal TotalDue { get; set; }
    //public string InWords { get; set; }
  }

  public class Bill
  {
    public string ProductName { get; set; }
    public string ProductId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Quantity { get; set; }
    public int BillingFrequency { get; set; }
    public decimal? Price { get; set; }
    public string Title { get; set; }
  }

  public class InvoiceExcelProcessor
  {
    private readonly string path;

    public InvoiceExcelProcessor(string path)
    {
      this.path = path;
    }

    public void CreateExcelAndSaveAsPdf(Invoice invoice, Company company, Customer customer, IEnumerable<Product> products)
    {

      if (!invoice.Bills.Any())
      {
        return;
      }

      invoice.InvoiceNo = $"{customer.CustomerName.Substring(0, 5)}-{DateTime.UtcNow:yyyyMMdd}-{invoice.Id}".ToUpper();

      string directory = $@"{path}\invoices";

      if (!Directory.Exists(directory))
      {
        Directory.CreateDirectory(directory);
      }

      string fileName = $@"{directory}\{invoice.InvoiceNo}.pdf";

      if (File.Exists(fileName))
      {
        try
        {
          File.Delete(fileName);
        }
        catch
        {
        }
      }

      Workbook workbook = null;
      Application application = null;

      try
      {
        application = new Application();
        string template;
        int diff = 0;
        int words = 0;
        if (customer.Currency.Equals("usd", StringComparison.InvariantCultureIgnoreCase))
        {
          //open and process usa excel file
          template = "usa";
          words = 4;
        }
        else if (customer.Currency.Equals("inr", StringComparison.InvariantCultureIgnoreCase))
        {
          //open and process india exce file
          template = "in";
          diff = 2;
          words = 8;
        }
        else
        {
          MessageBox.Show("No excel template defined for customer country", "Template not found", MessageBoxButtons.OK,
            MessageBoxIcon.Error);
          return;
        }

        string excelFileName = $@"{directory}\{invoice.InvoiceNo}.xlsx";

        File.Copy($@"{path}\templates\invoice-{template}.xlsx", excelFileName, true);

        workbook = application.Workbooks.Open(excelFileName);

        //dynamic activeSheet = workbook.ActiveSheet;
        _Worksheet activeSheet = (_Worksheet)workbook.Worksheets.Item[1];

        activeSheet.Range["C9"].Value2 = customer.CustomerName;
        activeSheet.Range["C10"].Value2 = customer.Address.Address1;
        activeSheet.Range["C11"].Value2 = customer.Address.Address2;
        activeSheet.Range["C12"].Value2 = $"{customer.Address.City}-{customer.Address.Zip}";
        activeSheet.Range["C13"].Value2 = $"{customer.Address.State}, {customer.Address.CountryCode}";
        activeSheet.Range["C14"].Value2 = customer.Address.Phone;

        activeSheet.Range["L9"].Value2 = invoice.InvoiceNo;
        activeSheet.Range["L10"].Value2 = $"{invoice.StartDate:MM-dd-yyyy} TO {invoice.EndDate:MM-dd-yyyy}";
        activeSheet.Range["L11"].Value2 = invoice.RefNumber;
        activeSheet.Range["L12"].Value2 = $"{invoice.BillDate:dd-MMM-yyyy}";

        //usa - C14, in - C16 //attn.

        activeSheet.Range[$"C{14 + diff}"].Value2 = $"Kind Attention: {customer.ContactName}";

        //usa - C18, E18, G18, I18, K18, L18, M18 in - C20, E20, G20, I20, K20, L20, M20  //payments

        activeSheet.Range[$"C{18 + diff}"].Value2 = invoice.PreviousAmt;
        activeSheet.Range[$"E{18 + diff}"].Value2 = invoice.PreviousPayment;
        activeSheet.Range[$"G{18 + diff}"].Value2 = invoice.Adjustments;
        activeSheet.Range[$"L{18 + diff}"].Value2 = invoice.DueDate;

        //products
        int index = 24;
        int counter = 0;
        foreach (Bill bill in invoice.Bills)
        {
          Product product = products.SingleOrDefault(x => x.Id == bill.ProductId);
          if (product == null)
          {
            continue;
          }

          if (!string.IsNullOrWhiteSpace(bill.Title))
          {
            index = 26 + diff + counter;
            activeSheet.Range[$"D{25 + diff + counter}"].Value2 = bill.Title;
          }
          else
          {
            index = 24 + diff + counter;
          }

          Range range = activeSheet.Range[$"A{index + 1}"].EntireRow;
          range.Insert(XlInsertShiftDirection.xlShiftDown);

          //if (counter == 0)
          //{
          //  range = activeSheet.Range[$"A{index++}"].EntireRow;
          //  range.Insert(XlInsertShiftDirection.xlShiftDown);
          //}

          activeSheet.Range[$"C{index}"].Value2 = ++counter;
          activeSheet.Range[$"D{index}"].Value2 = product.ProductName;
          activeSheet.Range[$"K{index}"].Value2 = bill.Quantity;
          activeSheet.Range[$"L{index}"].Value2 = bill.Price ?? product.Price;
          activeSheet.Range[$"L{index}"].HorizontalAlignment = XlHAlign.xlHAlignRight;

          decimal subTotal;
          if (bill.Price.HasValue)
          {
            subTotal = bill.Price.Value * bill.Quantity;
          }
          else if (bill.BillingFrequency != (int)BillingFrequency.Other)
          {
            subTotal = product.Price * bill.Quantity * bill.BillingFrequency;
          }
          else
          {
            int daysInMonth = DateTime.DaysInMonth(bill.StartDate.Year, bill.StartDate.Month);

            decimal pricePerDay = product.Price / daysInMonth;

            subTotal = (decimal)(bill.EndDate - bill.StartDate).TotalDays * pricePerDay * bill.Quantity;
          }
          activeSheet.Range[$"M{index}"].Value2 = subTotal;
        }

        activeSheet.Range[$"M{index + 2}"].Formula = $"=SUM(M{index - --counter}:M{index})";

        workbook.Save();

        string value = activeSheet.Range[$"M{index + words}"].Value2.ToString();
        decimal parsed;
        if (decimal.TryParse(value, out parsed))
        {
          string numberToWords = NumberToWords.ToWords((int)parsed);
          activeSheet.Range[$"C{index + words + 2}"].Value2 = $"In words: {numberToWords} only";
        }

        workbook.Save();
        workbook.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, fileName);

        //if (File.Exists(excelFileName))
        //{
        //  File.Delete(excelFileName);
        //}
      }
      catch (Exception exception)
      {
        //throw;
      }
      finally
      {
        if (workbook != null)
        {
          workbook.Close();
          Marshal.ReleaseComObject(workbook);
        }
        application?.Quit();
      }
    }
  }

  public partial class FrmInvoiceGenerator : Form
  {
    private readonly string rootPath;
    private string fileName;

    public FrmInvoiceGenerator()
    {
      InitializeComponent();
      rootPath = Directory.GetCurrentDirectory();
    }

    private void btnGenerate_Click(object sender, EventArgs e)
    {
      IDataProvider dataProvider = new ExcelDataProvider(fileName);
      IEnumerable<Invoice> invoices = dataProvider.ReadInvoices();
      IEnumerable<Customer> customers = dataProvider.ReadCustomers().ToList();
      IEnumerable<Company> companies = dataProvider.ReadCompanies().ToList();
      IEnumerable<Product> products = dataProvider.ReadProducts().ToList();

      InvoiceExcelProcessor processor = new InvoiceExcelProcessor(rootPath);

      foreach (var invoice in invoices)
      {
        var customer = customers.Single(x => x.Id == invoice.CustomerId);

        processor.CreateExcelAndSaveAsPdf(invoice, companies.Single(x => x.Id == invoice.CompanyId), customer, products);

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