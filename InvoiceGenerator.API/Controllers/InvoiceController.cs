using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using InvoiceGenerator.Data;
using InvoiceGenerator.Entities;

namespace InvoiceGenerator.API.Controllers
{
  public class InvoiceController : BaseApiController<Invoice>
  {
    private readonly IRepository<Customer> customeRepository;

    public InvoiceController(IRepository<Invoice> repository, IRepository<Customer> customeRepository) : base(repository)
    {
      this.customeRepository = customeRepository;
    }

    [HttpGet]
    [Route("api/invoice")]
    public async Task<IHttpActionResult> GetInvoices()
    {
      //IEnumerable<Invoice> invoices = new List<Invoice>
      //{
      //  new Invoice
      //  {
      //    InvoiceNo = $"abcd-{DateTime.UtcNow.ToString("yyyyMMdd")}-02",
      //    CompanyId = "1",
      //    CustomerId = "1",
      //    BillDate = DateTime.UtcNow.AddDays(-1),
      //    DueDate = DateTime.UtcNow.AddDays(10),
      //    StartDate = DateTime.UtcNow.AddMonths(-1),
      //    EndDate = DateTime.UtcNow.AddMonths(+1)
      //  },
      //  new Invoice
      //  {
      //    InvoiceNo = $"pqrs-{DateTime.UtcNow.ToString("yyyyMMdd")}-03",
      //    CompanyId = "1",
      //    CustomerId = "1",
      //    BillDate = DateTime.UtcNow.AddDays(-1),
      //    DueDate = DateTime.UtcNow.AddDays(10),
      //    StartDate = DateTime.UtcNow.AddMonths(-1),
      //    EndDate = DateTime.UtcNow.AddMonths(+1)
      //  }
      //};

      //await Task.FromResult(0);

      IEnumerable<Invoice> invoices = await Repository.GetAll().ConfigureAwait(false);
      return Ok(invoices);
    }

    [HttpGet]
    [Route("api/invoice/{id}")]
    public async Task<IHttpActionResult> GetInvoice(string id)
    {
      return Ok(await Repository.GetById(id).ConfigureAwait(false));
    }

    [HttpPost]
    [Route("api/invoice")]
    public async Task<IHttpActionResult> AddInvoice([FromBody] Invoice invoice)
    {
      await GetInvoiceNo(invoice);
      return Ok(await Repository.AddOrUpdate(null, invoice).ConfigureAwait(false));
    }

    [HttpPut]
    [Route("api/invoice/{id}")]
    public async Task<IHttpActionResult> UpdateInvoice([FromUri] string id, [FromBody] Invoice invoice)
    {
      await GetInvoiceNo(invoice);
      return Ok(await Repository.AddOrUpdate(id, invoice).ConfigureAwait(false));
    }

    private async Task GetInvoiceNo(Invoice invoice)
    {
      Customer customer = await customeRepository.GetById(invoice.CustomerId);
      invoice.GenerateInvoiceNo(invoice, customer);
    }

    [HttpDelete]
    [Route("api/invoice/{id}")]
    public async Task<IHttpActionResult> DeleteInvoice(string id)
    {
      return Ok(await Repository.Delete(id).ConfigureAwait(false));
    }
  }
}