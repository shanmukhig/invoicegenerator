using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BillGenerator;
using InvoiceGenerator.Entities;
using IronPdf;

namespace InvoiceGenerator
{
  public class InvoiceExtender
  {
    private static readonly string row = @"<div class='row'>
          <div class='col-p-1 text-center'><span class='txt txtg'>{6}</span></div>
          <div class='col-p-3 text-left ww'><span class='txt txtb'>{0}</span></div>
          <div class='col-p-2 text-center'><span class='txt txtg'>{1}</span></div>
          <div class='col-p-2 text-center'><span class='txt txtg'>{2}</span></div>
          <div class='col-p-1 nms'><span class='txt txtg nml'><i class='fa fa-{7}'></i>{3}</span></div>
          <div class='col-p-1 nms'><span class='txt txtg nm nms'>{4}</span></div>
          <div class='col-p-1 nml'><span class='txt txtg nm nml'><i class='fa fa-{7}'></i>{5}</span></div>
        </div>";

    private readonly Customer customer;

    public InvoiceExtender(Invoice invoice, IList<Product> products, Customer customer)
    {
      Invoice = invoice;
      this.customer = customer;

      var sb = new StringBuilder();
      decimal total = 0;
      var i = 1;
      foreach (var bill in invoice.Bills.Where(x => x.InvoiceId == Invoice.Id))
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
          var daysInMonth = DateTime.DaysInMonth(Invoice.StartDate.Year, Invoice.StartDate.Month);

          var pricePerDay = product.Price/daysInMonth;

          price = (decimal) (Invoice.EndDate - Invoice.StartDate).TotalDays*pricePerDay*bill.Quantity;
        }

        sb.Append(string.Format(row, product.ProductName, bill.StartDate.ToString("dd MMM yyyy"),
          bill.EndDate.ToString("dd MMM yyyy"),
          bill.Price.HasValue ? bill.Price.Value.CFormat() : product.Price.CFormat(), bill.Quantity, price.CFormat(),
          i++, customer.Currency));
        total += price;
      }
      summaryOfAccounts = sb.ToString();
      currentCharges = total;
    }

    public string summaryOfAccounts { get; set; }
    public decimal currentCharges { get; set; }

    public string SummaryOfAccounts => summaryOfAccounts;

    public decimal CurrentCharges => currentCharges;

    public Invoice Invoice { get; }

    public decimal ServiceTax => (CurrentCharges + Invoice.Adjustments)*customer.Tax.ServiceTax/100;

    public decimal Swatch => (CurrentCharges + Invoice.Adjustments)*customer.Tax.SwatchBharat/100;

    public decimal KrishiKalyan => (CurrentCharges + Invoice.Adjustments)*customer.Tax.KrishiKalyan/100;

    public decimal Vat => (ServiceTax + CurrentCharges + Invoice.Adjustments)*customer.Tax.Vat/100;

    public decimal TotalCharges => CurrentCharges + ServiceTax + Swatch + Vat;

    public decimal BalanceCarryForward => Invoice.PreviousAmt - Invoice.PreviousPayment + Invoice.Adjustments;

    public decimal TotalDue => BalanceCarryForward + TotalCharges;

    public string InWords => NumberToWords.ToWords((int) Math.Ceiling(TotalDue));

    public string InvoiceNo
    {
      get
      {
        this.Invoice.InvoiceNo = $"{customer.CustomerName.Substring(0, 5)}-{DateTime.UtcNow.ToString("yyyyMMdd")}-{Invoice.Id}";
        return this.Invoice.InvoiceNo;
      }
    }
    
  }

  public class InvoiceProcessor
  {
    //private readonly string templatePath;
    private readonly string logo;
    private readonly string path;
    private readonly PdfPrintOptions pdfPrintOptions;
    private readonly IList<Product> products;
    private readonly string text;

    public InvoiceProcessor(IDataProvider dataProvider, string path)
    {
      this.path = path;
      products = dataProvider.ReadProducts().ToList();
      //this.templatePath = templatePath;
      logo = $@"{path}\templates\logo.png";
      //path = @"D:\AssetManager\BillGenerator\BillProcessor\";

      using (var reader = File.OpenText($@"{path}\templates\template.html"))
      {
        text = reader.ReadToEnd();
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
    }

    public string GeneratePdf(Invoice i, Company company, Customer customer)
    {
      var extender = new InvoiceExtender(i, products, customer);

      pdfPrintOptions.Footer.LeftText =
        $"{company.CompanyName}, {company.Address.Address1}, {company.Address.Address2}, {company.Address.City}.{Environment.NewLine}{company.Address.State}, {company.Address.Zip}, Phone: {company.Address.Phone}. CIN: {company.Tax.Cin}";

      var htmlToPdf = new HtmlToPdf(pdfPrintOptions);

      var resource = htmlToPdf.RenderHtmlAsPdf(GetFormattedHtml(extender, company, customer));

      string directory = $@"{path}\invoices";

      if (!Directory.Exists(directory))
      {
        Directory.CreateDirectory(directory);
      }

      string fileName = $@"{directory}\{extender.InvoiceNo}.pdf";

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

      resource.SaveAs($@"{path}\invoices\{extender.InvoiceNo}.pdf");

      return extender.InvoiceNo;
    }

    private string GetFormattedHtml(InvoiceExtender extender, Company company, Customer customer)
    {
      var i = extender.Invoice;

      return string.Format(text,
        logo, //0
        i.StartDate.DFormat("dd MMM yyyy"), //1
        i.EndDate.DFormat("dd MMM yyyy"), //2
        customer.CustomerName, //3
        customer.Address.Address1, //4
        customer.Address.Address2, //5
        customer.Address.City, //6
        customer.Address.State, //7
        customer.Address.Zip, //8
        customer.Address.Phone, //9
        $"{Math.Round(extender.TotalDue, 0, MidpointRounding.AwayFromZero).CFormat("C0")}.00", //10
        i.DueDate.ToString("dddd, dd MMMM yyyy"), //11
        extender.InvoiceNo, //12
        i.BillDate.DFormat("dd MMM yyyy"), //13
        company.Tax.ServiceTaxNo, //14
        company.Tax.Pan, //15
        company.Tax.Tin, //16
        i.PreviousAmt.CFormat(), //17
        i.PreviousPayment.CFormat(), //18
        i.Adjustments.CFormat(), //19
        extender.CurrentCharges.CFormat(), //20
        extender.BalanceCarryForward.CFormat(), //21
        extender.Invoice.Adjustments.CFormat(), //22
        customer.Tax.ServiceTax.ToString("#00.00"), //23
        extender.ServiceTax.CFormat(), //24
        customer.Tax.SwatchBharat.ToString("#00.00"), //25
        extender.Swatch.CFormat(), //26
        customer.Tax.KrishiKalyan.ToString("#00.00"), //27
        extender.KrishiKalyan.CFormat(), //28
        customer.Tax.Vat.ToString("#00.00"), //29
        extender.Vat.CFormat(), //30
        extender.TotalCharges.CFormat(), //31
        $"{Extentions.GetCurrency(customer.Currency)} {extender.InWords}", //32
        extender.SummaryOfAccounts, //33
        company.CompanyName, //34
        company.Support.Phone, //35
        company.Support.Email, //36,
        customer.Currency, //37,
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
}