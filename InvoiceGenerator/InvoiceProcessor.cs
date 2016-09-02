using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BillGenerator;
using InvoiceGenerator.Entities;
using IronPdf;

namespace InvoiceGenerator
{
  public class InvoiceExtender
  {
    private static readonly string row =
      @"<div class='row'><div class='col-p-1 text-center'><span class='txt txtg'>{6}</span></div><div class='col-p-3 text-left ww'><span class='txt txtb'>{0}</span></div><div class='col-p-2 text-center'><span class='txt txtg'>{1}</span></div><div class='col-p-2 text-center'><span class='txt txtg'>{2}</span></div><div class='col-p-1 nms'><span class='txt txtg nml'><i class='fa fa-{7}'></i>{3}</span></div><div class='col-p-1 nms'><span class='txt txtg nm nms'>{4}</span></div><div class='col-p-1 nml'><span class='txt txtg nm nml'><i class='fa fa-{7}'></i>{5}</span></div></div>";

    private static readonly string taxes =
      @"<div class='row'><div class='col-p-6 pdl pdtb'><span class='txt txtg'>{0}({1}%)</span></div><div class='col-p-25 pdtb'><span class='txt txtb nm nml'><i class='fa fa-{2}'></i>{3}</span></div><div class='col-p-25'><span></span></div></div>";

    private readonly Customer customer;

    private readonly Invoice invoice;

    private readonly string currency;

    public InvoiceExtender(Invoice invoice, IEnumerable<Product> products, Customer customer)
    {
      this.invoice = invoice;
      this.customer = customer;
      this.currency = customer.Currency.Replace("fa-", string.Empty);

      CalculateTotals(products);
      CalculateTaxes();
      this.invoice.BalanceCarryForward = this.invoice.PreviousAmt - this.invoice.PreviousPayment + this.invoice.Adjustments;
      this.invoice.TotalDue = (int) Math.Ceiling(this.invoice.TotalCharges + this.invoice.BalanceCarryForward);
      this.invoice.InWords = NumberToWords.ToWords((int) this.invoice.TotalDue);
      DateTime dt = DateTime.UtcNow;
      this.invoice.InvoiceNo = $"{customer.CustomerName.Substring(0, 5)}-{dt.ToString("yyyyMMdd-HHmmss")}".ToUpper();
      this.invoice.Currency = this.customer.Currency;
    }

    private void CalculateTaxes()
    {
      decimal value = this.invoice.CurrentCharges + this.invoice.Adjustments;
      decimal final = 0;

      StringBuilder sb = new StringBuilder();

      foreach (Tax tax in customer.Taxes)
      {
        decimal v = value*(tax.Percent/100);
        sb.Append(string.Format(taxes, tax.Name, tax.Percent, this.currency, v.ToString("N")));
        final += v;
      }
      this.invoice.Taxes = sb.ToString();
      this.invoice.TotalCharges = this.invoice.CurrentCharges + final;
    }

    private void CalculateTotals(IEnumerable<Product> products)
    {
      products = products?.ToList() ?? new List<Product>();

      StringBuilder sb = new StringBuilder();
      decimal total = 0;
      var i = 1;
      foreach (var bill in invoice.Bills)
      {
        var product = products.SingleOrDefault(x => x.Id == bill.ProductId);

        if (product == null)
        {
          continue;
        }

        decimal price;
        if (bill.Price.HasValue)
        {
          price = bill.Price.Value*bill.Quantity;
        }
        else if (bill.BillingFrequency != (int) BillingFrequency.Other)
        {
          price = product.Price*bill.Quantity*bill.BillingFrequency;
        }
        else
        {
          int daysInMonth = DateTime.DaysInMonth(this.invoice.StartDate.Year, this.invoice.StartDate.Month);

          decimal pricePerDay = product.Price/daysInMonth;

          price = (decimal) (this.invoice.EndDate - this.invoice.StartDate).TotalDays*pricePerDay*bill.Quantity;
        }

        sb.Append(string.Format(row, product.ProductName, bill.StartDate.ToString("dd MMM yyyy"),
          bill.EndDate.ToString("dd MMM yyyy"),
          bill.Price.HasValue ? bill.Price.Value.CFormat() : product.Price.CFormat(), bill.Quantity, price.CFormat(),
          i++, this.currency));
        total += price;
      }
      this.invoice.SummaryOfAccounts = sb.ToString();
      this.invoice.CurrentCharges = total;
    }

    //public string SummaryOfAccounts { get; private set; }

    //public decimal CurrentCharges { get; private set; }

    //public Invoice Invoice { get; }

    //public string Taxes { get; set; }

    //public decimal TotalCharges { get; private set; }

    //public decimal BalanceCarryForward => Invoice.PreviousAmt - Invoice.PreviousPayment + Invoice.Adjustments;

    //public decimal TotalDue => BalanceCarryForward + TotalCharges;

    //public string InWords => NumberToWords.ToWords((int) Math.Ceiling(TotalDue));

    //public string InvoiceNo
    //{
    //  get
    //  {
    //    this.Invoice.InvoiceNo = $"{customer.CustomerName.Substring(0, 5)}-{DateTime.UtcNow.ToString("yyyyMMdd")}-{Invoice.Id.Substring(0, 5)}".ToUpper();
    //    return this.Invoice.InvoiceNo;
    //  }
    //}

  }

  //public class PdfSharpInvoiceProcessor : IInvoiceProcessor
  //{
  //  public string GetPdfFile(Invoice i, Company company, Customer customer, IEnumerable<Product> products)
  //  {
  //    throw new NotImplementedException();
  //  }

  //  public MemoryStream GetPdfStream(Invoice i, Company company, Customer customer, IEnumerable<Product> products)
  //  {
  //    throw new NotImplementedException();
  //  }
  //}

  public class InvoiceProcessor : IInvoiceProcessor
  {
    private string logo;
    private readonly string path;
    private PdfPrintOptions pdfPrintOptions;
    private string text;

    public InvoiceProcessor(string path)
    {
      this.path = path;
    }

    public MemoryStream GetPdfStream(Invoice i, Company company, Customer customer, IEnumerable<Product> products)
    {
      InvoiceExtender extender = new InvoiceExtender(i, products, customer);
      PdfResource resource = GeneratePdfResource(i, company, customer);
      return resource.Stream;
    }

    public string GetPdfFile(Invoice i, Company company, Customer customer, IEnumerable<Product> products)
    {
      InvoiceExtender extender = new InvoiceExtender(i, products, customer);

      string directory = $@"{path}\invoices";

      if (!Directory.Exists(directory))
      {
        Directory.CreateDirectory(directory);
      }

      string fileName = $@"{directory}\{i.InvoiceNo}.pdf";

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

      PdfResource resource = GeneratePdfResource(i, company, customer);

      resource.SaveAs(fileName);

      return i.InvoiceNo;
    }

    private PdfResource GeneratePdfResource(Invoice i, Company company, Customer customer)
    {
      logo = Path.Combine(path, @"templates\logo.png");

      using (var reader = File.OpenText(Path.Combine(path, @"templates\template.html")))
      {
        text = reader.ReadToEnd();//.Replace("##path##", path);
      }

      pdfPrintOptions = new PdfPrintOptions
      {
        CreatePdfFormsFromHtml = true,
        CssMediaType = PdfPrintOptions.PdfCssMediaType.Print,
        FirstPageNumber = 0,
        PaperSize = PdfPrintOptions.PdfPaperSize.A4,
        CustomCssUrl = new Uri($@"{path}\templates\format.css"),
        PrintHtmlBackgrounds = true,
        Footer = new PdfPrintOptions.PdfHeaderFooter
        {
          RightText = "Page {page} of {total-pages}",
          LeftText = "",
          DrawDividerLine = true,
          FontFamily = "Eras Medium ITC",
          FontSize = 8
        },
        LicenseKey = "IRONPDF-AB27C4-178893-AB27C4-54D1AF1CB5-E0D33E16-22DCCAD251-189927",
        MarginRight = 15,
        MarginTop = 15,
        MarginLeft = 15
      };
      
      pdfPrintOptions.Footer.LeftText =
        $"{company.CompanyName}, {company.Address.Address1}, {company.Address.Address2}, {company.Address.City}.{Environment.NewLine}{company.Address.State}, {company.Address.Zip}, Phone: {company.Address.Phone}. CIN: {company.Tax.Cin}";

      HtmlToPdf htmlToPdf = new HtmlToPdf(pdfPrintOptions);

      //string formattedHtml = GetFormattedHtml(i, company, customer);

      PdfResource resource = htmlToPdf.RenderHtmlAsPdf(GetFormattedHtml(i, company, customer));

      return resource;
    }

    private string GetFormattedHtml(Invoice i, Company company, Customer customer)
    {
      string currency = customer.Currency.Replace("fa-", string.Empty);

      return string.Format(text,
        company.Logo ?? this.logo, //0
        i.StartDate.ToString("dd MMM yyyy"), //1
        i.EndDate.ToString("dd MMM yyyy"), //2
        customer.CustomerName, //3
        customer.Address.Address1, //4
        customer.Address.Address2, //5
        customer.Address.City, //6
        customer.Address.State, //7
        customer.Address.Zip, //8
        customer.Address.Phone, //9
        $"{Math.Round(i.TotalDue, 0, MidpointRounding.AwayFromZero).CFormat("C0")}.00", //10
        i.DueDate.ToString("dddd, dd MMMM yyyy"), //11
        i.InvoiceNo, //12
        i.BillDate.ToString("dd MMM yyyy"), //13
        company.Tax.ServiceTaxNo, //14
        company.Tax.Pan, //15
        company.Tax.Tin, //16
        i.PreviousAmt.CFormat(), //17
        i.PreviousPayment.CFormat(), //18
        i.Adjustments.CFormat(), //19
        i.CurrentCharges.CFormat(), //20
        i.BalanceCarryForward.CFormat(), //21
        i.Adjustments.CFormat(), //22
        i.Taxes, //23
        string.Empty, //24
        string.Empty, //25
        string.Empty, //26
        string.Empty, //27
        string.Empty, //28
        string.Empty, //29
        string.Empty, //30
        i.TotalCharges.CFormat(), //31
        $"{Extentions.GetCurrency(currency)} {i.InWords}", //32
        i.SummaryOfAccounts, //33
        company.CompanyName, //34
        company.Support.Phone, //35
        company.Support.Email, //36,
        currency, //37,
        company.Ifsc.Beneficiary, //38
        company.Ifsc.BankName, //39
        company.Ifsc.AccountNo, //40
        company.Ifsc.Code, //41
        company.Ifsc.Branch, //42
        company.Swift.Beneficiary, //43
        company.Swift.BankName, //44
        company.Swift.AccountNo, //45
        company.Swift.Code, //46
        company.Swift.Branch //47
        );
    }
  }

  public interface IInvoiceProcessor
  {
    string GetPdfFile(Invoice i, Company company, Customer customer, IEnumerable<Product> products);
    MemoryStream GetPdfStream(Invoice i, Company company, Customer customer, IEnumerable<Product> products);
  }
}