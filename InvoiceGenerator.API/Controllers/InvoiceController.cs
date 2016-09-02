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
          TotalDue = i.TotalDue
        });

      //return Ok(map);
    }

    [HttpGet]
    [Route("api/invoice/pdf/{id}")]
    public async Task<HttpResponseMessage> GetPdf(string id)
    {
      Invoice invoice = await Repository.GetById(id).ConfigureAwait(false);

      if (invoice.PdfStream == null)
      {
        return new HttpResponseMessage(HttpStatusCode.NotFound);
      }

      //File.WriteAllBytes($@"C:\temp\{invoice.Id}.pdf", invoice.PdfStream);

      //return Ok(invoice.PdfStream);

      ContentDispositionHeaderValue value = new ContentDispositionHeaderValue("attachment")
      {
        FileName = invoice.InvoiceNo + ".pdf"
      };

      HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = new ByteArrayContent(invoice.PdfStream)
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
      invoice.PdfStream = null;
      return Ok(invoice);
    }

    [HttpPost]
    [Route("api/invoice")]
    public async Task<IHttpActionResult> AddInvoice([FromBody] Invoice invoice)
    {
      await GenerateInvoice(invoice);
      return Ok(await Repository.AddOrUpdate(null, invoice).ConfigureAwait(false));
    }

    [HttpPut]
    [Route("api/invoice/{id}")]
    public async Task<IHttpActionResult> UpdateInvoice([FromUri] string id, [FromBody] Invoice invoice)
    {
      //await GetInvoiceNo(invoice);
      await GenerateInvoice(invoice);
      return Ok(await Repository.AddOrUpdate(id, invoice).ConfigureAwait(false));
    }

    private async Task GenerateInvoice(Invoice invoice)
    {
      Company company = await this.companyRepository.GetById(invoice.CompanyId);
      IEnumerable<Product> products = await this.productRepository.GetAll();
      Customer customer = await this.customeRepository.GetById(invoice.CustomerId);

      MemoryStream stream = invoiceProcessor.GetPdfStream(invoice, company, customer, products);
      stream.Position = 0;
      invoice.PdfStream = new byte[stream.Length];
      await stream.ReadAsync(invoice.PdfStream, 0, (int)stream.Length);

      //string pdfFile = invoiceProcessor.GetPdfFile(invoice, company, customer, products);
    }

    [HttpDelete]
    [Route("api/invoice/{id}")]
    public async Task<IHttpActionResult> DeleteInvoice(string id)
    {
      return Ok(await Repository.Delete(id).ConfigureAwait(false));
    }
  }
}