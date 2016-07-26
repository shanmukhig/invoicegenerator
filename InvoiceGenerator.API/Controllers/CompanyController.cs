using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using InvoiceGenerator.Data;
using InvoiceGenerator.Entities;

namespace InvoiceGenerator.API.Controllers
{
  //public class MultiPartMediaTypeFormatter : MediaTypeFormatter
  //{
  //  public MultiPartMediaTypeFormatter()
  //  {
  //    SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
  //  }
  //  public override bool CanReadType(Type type)
  //  {
  //    return true;
  //  }

  //  public override bool CanWriteType(Type type)
  //  {
  //    return true;
  //  }

  //  public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
  //  {
  //    if (type == (Type)null)
  //      throw new ArgumentNullException(nameof(type));
  //    if (readStream == null)
  //      throw new ArgumentNullException(nameof(readStream));

  //    return Task.FromResult<object>(new Company());

  //    //if (effectiveEncoding == null)
  //    //  throw Error.ArgumentNull("effectiveEncoding");
  //    //if (!this.UseDataContractJsonSerializer)
  //    //  return base.ReadFromStream(type, readStream, effectiveEncoding, formatterLogger);
  //    //using (XmlReader reader = (XmlReader)JsonReaderWriterFactory.CreateJsonReader((Stream)new NonClosingDelegatingStream(readStream), effectiveEncoding, this._readerQuotas, (OnXmlDictionaryReaderClose)null))
  //    //  return this.GetDataContractSerializer(type).ReadObject(reader);
  //  }

  //  public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger,
  //    CancellationToken cancellationToken)
  //  {
  //    if (type == (Type)null)
  //      throw new ArgumentNullException(nameof(type));
  //    if (readStream == null)
  //      throw new ArgumentNullException(nameof(readStream));

  //    return Task.FromResult<object>(new Company());
  //    //return base.ReadFromStreamAsync(type, readStream, content, formatterLogger, cancellationToken);
  //  }

  //  public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
  //  {
  //    return base.WriteToStreamAsync(type, value, writeStream, content, transportContext);
  //  }

  //  public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext, CancellationToken cancellationToken)
  //  {
  //    return base.WriteToStreamAsync(type, value, writeStream, content, transportContext, cancellationToken);
  //  }
  //}

  //public class PhotoMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
  //{

  //  public PhotoMultipartFormDataStreamProvider(string path) : base(path)
  //  {
  //  }

  //  public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
  //  {
  //    //Make the file name URL safe and then use it & is the only disallowed url character allowed in a windows filename 
  //    var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName)
  //      ? headers.ContentDisposition.FileName
  //      : "NoName";
  //    return name.Trim(new char[] {'"'})
  //      .Replace("&", "and");
  //  }
  //}

  public class CompanyApiController : BaseApiController<Company>
  {
    public CompanyApiController(IRepository<Company> repository) : base(repository)
    {
    }

    [HttpGet]
    [Route("api/company")]
    public async Task<IHttpActionResult> GetCompanies()
    {
      return Ok(await Repository.GetAll().ConfigureAwait(false));
    }

    [HttpGet]
    [Route("api/company/{id}")]
    public async Task<IHttpActionResult> GetCompany(string id)
    {
      return Ok(await Repository.GetById(id).ConfigureAwait(false));
    }

    [HttpPost]
    [Route("api/company")]
    public async Task<IHttpActionResult> AddCompany([FromBody] Company company)
    {
      return Ok(await Repository.AddOrUpdate(null, company).ConfigureAwait(false));
    }

    [HttpPut]
    [Route("api/company/{id}")]
    public async Task<IHttpActionResult> UpdateCompany([FromUri] string id, [FromBody] Company company)
    {
      return Ok(await Repository.AddOrUpdate(id, company).ConfigureAwait(false));
    }

    [HttpDelete]
    [Route("api/company/{id}")]
    public async Task<IHttpActionResult> DeleteCompany(string id)
    {
      return Ok(await Repository.Delete(id).ConfigureAwait(false));
    }
  }
}