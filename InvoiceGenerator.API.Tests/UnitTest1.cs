using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using InvoiceGenerator.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvoiceGenerator.API.Tests
{
  [TestClass]
  public class UnitTest1
  {
    private string NewGuid => Guid.NewGuid().ToString("N").ToUpper().Substring(0, 10);
    private DateTime NewDate => DateTime.UtcNow;

    [TestMethod]
    public void TestMethod1()
    {
      IInvoiceProcessor processor = new InvoiceProcessor(Path.GetDirectoryName(Uri.UnescapeDataString((new UriBuilder(Assembly.GetExecutingAssembly().CodeBase)).Path)));

      Address address = new Address
      {
        Phone = NewGuid,
        Email = NewGuid,
        State = NewGuid,
        Zip = NewGuid,
        Address2 = NewGuid,
        City = NewGuid,
        Address1 = NewGuid,
        CountryCode = "India"
      };

      IEnumerable<Product> products = new List<Product>
      {
        new Product
        {
          Id = NewGuid,
          Price = 123,
          Currency = "fa-inr",
          ProductName = "Skype for business",
          Quantity = 1000,
          CountryCode = "India"
        }
      };

      Company company = new Company
      {
        Id = NewGuid,
        Tax = new CompanyTax
        {
          Tin = NewGuid,
          ServiceTaxNo = NewGuid,
          Cin = NewGuid,
          Pan = NewGuid
        },
        Ifsc =
          new Bank
          {
            BankName = NewGuid,
            AccountNo = NewGuid,
            Code = NewGuid,
            Branch = NewGuid,
            Beneficiary = NewGuid
          },
        Swift =
          new Bank
          {
            BankName = NewGuid,
            AccountNo = NewGuid,
            Code = NewGuid,
            Branch = NewGuid,
            Beneficiary = NewGuid
          },
        Address = address,
        Currency = "fa-inr",
        Support = new Support {Phone = NewGuid, Email = NewGuid},
        CompanyName = NewGuid,
        Cheque = new Bank {BankName = NewGuid, AccountNo = NewGuid}
      };

      Customer customer = new Customer
      {
        Id = NewGuid,
        Address = address,
        CustomerName = NewGuid,
        Currency = "fa-inr",
        Taxes = new List<Tax> { new Tax { Name = NewGuid, Percent = (decimal)12.50}, new Tax {Name = NewGuid, Percent = (decimal)4} },
        ContactName = NewGuid
      };

      Invoice invoice = new Invoice
      {
        Adjustments = 1,
        BillDate = NewDate,
        CompanyId = company.Id,
        Currency = "fa-inr",
        CustomerId = customer.Id,
        DueDate = NewDate,
        Id = NewGuid,
        Bills =
          new List<Bill>
          {
            new Bill
            {
              StartDate = NewDate,
              Price = (decimal) 123,
              EndDate = NewDate,
              Quantity = 1,
              ProductId = products.First().Id,
              BillingFrequency = 1
            }
          },
        EndDate = NewDate,
        StartDate = NewDate,
        PreviousAmt = 125,
        PreviousPayment = 123
      };

      string pdfFile = processor.GetPdfFile(invoice, company, customer, products);
    }
  }
}
