using System.Threading.Tasks;
using System.Web.Http;
using InvoiceGenerator.Data;
using InvoiceGenerator.Entities;

namespace InvoiceGenerator.API.Controllers
{
  public class ProductController : BaseApiController<Product>
  {
    public ProductController(IRepository<Product> repository) : base(repository)
    {
    }

    [HttpGet]
    [Route("api/product")]
    public async Task<IHttpActionResult> GetCompanies()
    {
      return Ok(await Repository.GetAll().ConfigureAwait(false));
    }

    [HttpGet]
    [Route("api/product/{id}")]
    public async Task<IHttpActionResult> GetCompany(string id)
    {
      return Ok(await Repository.GetById(id).ConfigureAwait(false));
    }

    [HttpPost]
    [Route("api/product")]
    public async Task<IHttpActionResult> AddCompany([FromBody] Product product)
    {
      return Ok(await Repository.AddOrUpdate(null, product).ConfigureAwait(false));
    }

    [HttpPut]
    [Route("api/product/{id}")]
    public async Task<IHttpActionResult> UpdateCompany([FromUri] string id, [FromBody] Product product)
    {
      return Ok(await Repository.AddOrUpdate(id, product).ConfigureAwait(false));
    }

    [HttpDelete]
    [Route("api/product/{id}")]
    public async Task<IHttpActionResult> DeleteCompany(string id)
    {
      return Ok(await Repository.Delete(id).ConfigureAwait(false));
    }
  }
}