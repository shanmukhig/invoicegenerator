using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using InvoiceGenerator.Data;
using InvoiceGenerator.Entities;

namespace InvoiceGenerator.API.Controllers
{
  public class InvoiceController : BaseApiController<Invoice>
  {
    private readonly IRepository<Customer> customeRepository;
    private readonly IRepository<Company> companyRepository;
    private readonly IRepository<Product> productRepository;
    private readonly IInvoiceProcessor invoiceProcessor;

    public InvoiceController(IRepository<Invoice> repository, 
      IRepository<Customer> customeRepository,
      IRepository<Company> companyRepository, 
      IRepository<Product> productRepository, 
      IInvoiceProcessor invoiceProcessor) : base(repository)
    {
      this.customeRepository = customeRepository;
      this.companyRepository = companyRepository;
      this.productRepository = productRepository;
      this.invoiceProcessor = invoiceProcessor;
    }

    [HttpGet]
    [Route("api/invoice")]
    public async Task<IHttpActionResult> GetInvoices()
    {
      IEnumerable<Product> products = (await productRepository.GetAll().ConfigureAwait(false)).ToList();

      IEnumerable<Invoice> invoices = await Repository.GetAll().ConfigureAwait(false);

      return Ok(from i in invoices
        select new Invoice
        {
          Id = i.Id,
          InvoiceNo = i.InvoiceNo,
          DueDate = i.DueDate,
          Bills = (from b in i.Bills
            select new Bill
              {
                EndDate = b.EndDate,
                Price = b.Price,
                StartDate = b.StartDate,
                ProductId = b.ProductId,
                Quantity = b.Quantity,
                ProductName = products.SingleOrDefault(x => x.Id == b.ProductId)?.ProductName,
                BillingFrequency = b.BillingFrequency
              }).ToList(),
          CustomerId = i.CustomerId,
          CompanyId = i.CompanyId,
          StartDate = i.StartDate,
          EndDate = i.EndDate,
          BillDate = i.BillDate,
          PreviousAmt = i.PreviousAmt,
          PreviousPayment = i.PreviousPayment,
          Adjustments = i.Adjustments,
          Currency = i.Currency,
          TotalDue = i.TotalDue,
          FileId = i.FileId
        });
    }

    [HttpGet]
    [Route("api/invoice/pdf/{id}")]
    public async Task<HttpResponseMessage> GetPdf(string id)
    {
      byte[] buffer = await Repository.DownloadFileAsync(id);

      if (buffer == null || buffer.Length == 0)
      {
        return new HttpResponseMessage(HttpStatusCode.NotFound);
      }

      ContentDispositionHeaderValue value = new ContentDispositionHeaderValue("attachment")
      {
        FileName = $"{id}.pdf"
      };

      HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = new ByteArrayContent(buffer)
      };

      message.Content.Headers.ContentDisposition = value;
      message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

      return message;
    }

    [HttpGet]
    [Route("api/invoice/{id}")]
    public async Task<IHttpActionResult> GetInvoice(string id)
    {
      Invoice invoice = await Repository.GetById(id).ConfigureAwait(false);
      return Ok(invoice);
    }

    [ValiateModalStateFilter]
    [HttpPost]
    [Route("api/invoice")]
    public async Task<IHttpActionResult> AddInvoice([FromBody] Invoice invoice)
    {
      await GenerateInvoice(invoice);
      return Ok(await Repository.AddOrUpdate(null, invoice).ConfigureAwait(false));
    }

    [ValiateModalStateFilter]
    [HttpPut]
    [Route("api/invoice/{id}")]
    public async Task<IHttpActionResult> UpdateInvoice([FromUri] string id, [FromBody] Invoice invoice)
    {
      await GenerateInvoice(invoice);
      return Ok(await Repository.AddOrUpdate(id, invoice).ConfigureAwait(false));
    }

    private async Task GenerateInvoice(Invoice invoice)
    {
      Company company = await this.companyRepository.GetById(invoice.CompanyId);
      IEnumerable<Product> products = await this.productRepository.GetAll();
      Customer customer = await this.customeRepository.GetById(invoice.CustomerId);

      MemoryStream stream = invoiceProcessor.GetPdfStream(invoice, company, customer, products);
      invoice.FileId = await Repository.UploadFileAsync(invoice.FileId, $"{invoice.InvoiceNo}.pdf", stream);
    }

    [HttpDelete]
    [Route("api/invoice/{id}")]
    public async Task<IHttpActionResult> DeleteInvoice(string id)
    {
      return Ok(await Repository.Delete(id).ConfigureAwait(false));
    }
  }
}