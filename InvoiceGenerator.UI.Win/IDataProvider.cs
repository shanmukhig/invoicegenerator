using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Excel;

namespace InvoiceGenerator.UI.Win
{
  public interface IDataProvider
  {
    IEnumerable<Company> ReadCompanies();
    IEnumerable<Customer> ReadCustomers();
    IEnumerable<Product> ReadProducts();
    IEnumerable<Invoice> ReadInvoices();
  }

  public class ExcelDataProvider : IDataProvider
  {
    private readonly string fileName;

    public ExcelDataProvider(string fileName)
    {
      this.fileName = fileName;
    }

    public IEnumerable<Company> ReadCompanies()
    {
      var reader = Read<Company>();
      return from DataRow row in reader
        //let swift = row[12].ToString().StrSplit()
        //let ifsc = row[13].ToString().StrSplit()
        select new Company
        {
          Id = row[0].ToString(),
          CompanyName = row[1].ToString(),
          //Address = new Address
          //{
          //  Address1 = row[2].ToString(),
          //  Address2 = row[3].ToString(),
          //  City = row[4].ToString(),
          //  State = row[5].ToString(),
          //  Zip = row[6].ToString(),
          //  Phone = row[7].ToString(),
          //  Email = row[8].ToString()
          //},
          //Tax = new CompanyTax
          //{
          //  ServiceTaxNo = row[9].ToString(),
          //  Tin = row[10].ToString(),
          //  Pan = row[11].ToString(),
          //  Cin = row[14].ToString()
          //},
          //Cheque = new Bank
          //{
          //  BankName = row[15].ToString()
          //},
          //Ifsc = new Bank
          //{
          //  Beneficiary = ifsc[0],
          //  Branch = ifsc[4],
          //  BankName = ifsc[1],
          //  AccountNo = ifsc[2],
          //  Code = ifsc[3]
          //},
          //Swift = new Bank
          //{
          //  Beneficiary = swift[0],
          //  Branch = swift[4],
          //  BankName = swift[1],
          //  AccountNo = swift[2],
          //  Code = swift[3]
          //},
          //Support = new Support
          //{
          //  Email = row[16].ToString(),
          //  Phone = row[17].ToString()
          //}
        };
    }

    public IEnumerable<Customer> ReadCustomers()
    {
      var reader = Read<Customer>();
      return from DataRow row in reader
        select new Customer
        {
          Id = row[0].ToString(),
          CustomerName = row[1].ToString(),
          ContactName = row[2].ToString(),
          Currency = row[7].ToString(),
          Address = new Address
          {
            Address1 = row[8].ToString(),
            Address2 = row[9].ToString(),
            City = row[10].ToString(),
            State = row[11].ToString(),
            Zip = row[12].ToString(),
            Phone = row[13].ToString(),
            Email = row[14].ToString()
          },
          //Taxes = new Tax[] {},
          //new CustomerTax
          // {
          //     ServiceTax = row[3].ToDecimalOrDefault(),
          //     SwatchBharat = row[4].ToDecimalOrDefault(),
          //     KrishiKalyan = row[5].ToDecimalOrDefault(),
          //     Vat = row[6].ToDecimalOrDefault()
          // }
        };
    }

    public IEnumerable<Product> ReadProducts()
    {
      var reader = Read<Product>();
      return from DataRow row in reader
        select new Product
        {
          Id = row[0].ToString(),
          ProductName = row[1].ToString(),
          Price = row[2].ToDecimalOrDefault()
        };
    }

    public IEnumerable<Invoice> ReadInvoices()
    {
      IList<dynamic> bills = ReadBills().ToList();

      var reader = Read<Invoice>();
      var invoices = from DataRow row in reader
        let id = row[0].ToString()
        //let b = bills.Where(x => x.InvoiceId == id).ToList()
        select new Invoice
        {
          Id = id,
          StartDate = row[1].ToDateTimeOrDefault(),
          EndDate = row[2].ToDateTimeOrDefault(),
          DueDate = row[3].ToDateTimeOrDefault(),
          BillDate = row[4].ToDateTimeOrDefault(),
          PreviousAmt = row[5].ToDecimalOrDefault(),
          PreviousPayment = row[6].ToDecimalOrDefault(),
          Adjustments = row[7].ToDecimalOrDefault(),
          CustomerId = row[8].ToString(),
          CompanyId = row[9].ToString(),
          RefNumber = row[10].ToString(),
          Discount = row[11].ToDecimalOrDefault(),
          DiscountDescription = row[12].ToString(),
          Bills = bills.Where(x => x.InvoiceId == id).Select(x => new Bill
          {
            ProductId = x.ProductId,
            StartDate = x.StartDate,
            EndDate = x.EndDate,
            BillingFrequency = x.BillingFrequency,
            Price = x.Price,
            Quantity = x.Quantity,
            Title = x.Title
          })
        };

      return invoices;
    }

    private IEnumerable<dynamic> ReadBills()
    {
      DataRowCollection reader = Read<Bill>();
      return from DataRow row in reader
        select new
        {
          InvoiceId = row[0].ToString(),
          ProductId = row[1].ToString(),
          StartDate = row[2].ToDateTimeOrDefault(),
          EndDate = row[3].ToDateTimeOrDefault(),
          BillingFrequency = row[4].ToIntOrDefault(),
          Price = row[6].ToDecimalOrDefault(),
          Quantity = row[5].ToIntOrDefault(),
          Title = row[7].ToString()
        };
    }

    private DataRowCollection Read<T>()
    {
      using (var reader = ExcelReaderFactory.CreateOpenXmlReader(File.Open(fileName, FileMode.Open, FileAccess.Read)))
      {
        reader.IsFirstRowAsColumnNames = true;
        var ds = reader.AsDataSet();
        var tableName = typeof(T).Name.ToLower();
        var table = ds.Tables[tableName];
        return table.Rows;
      }
    }
  }
}