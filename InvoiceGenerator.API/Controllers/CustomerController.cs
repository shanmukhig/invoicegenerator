using System.Threading.Tasks;
using System.Web.Http;
using InvoiceGenerator.Data;
using InvoiceGenerator.Entities;

namespace InvoiceGenerator.API.Controllers
{
  public class CustomerController : BaseApiController<Customer>
  {
    public CustomerController(IRepository<Customer> repository) : base(repository)
    {
    }

    [HttpGet]
    [Route("api/customer")]
    public async Task<IHttpActionResult> GetCustomers()
    {
      var customers = await Repository.GetAll().ConfigureAwait(false);
      return Ok(customers);
    }

    [HttpGet]
    [Route("api/customer/{id}")]
    public async Task<IHttpActionResult> GetCustomer(string id)
    {
      return Ok(await Repository.GetById(id).ConfigureAwait(false));
    }

    [HttpPost]
    [Route("api/customer")]
    public async Task<IHttpActionResult> AddCustomer([FromBody] Customer customer)
    {
      return Ok(await Repository.AddOrUpdate(null, customer).ConfigureAwait(false));
    }

    [HttpPut]
    [Route("api/customer/{id}")]
    public async Task<IHttpActionResult> UpdateCustomer([FromUri] string id, [FromBody] Customer customer)
    {
      return Ok(await Repository.AddOrUpdate(id, customer).ConfigureAwait(false));
    }

    [HttpDelete]
    [Route("api/customer/{id}")]
    public async Task<IHttpActionResult> DeleteCustomer(string id)
    {
      return Ok(await Repository.Delete(id).ConfigureAwait(false));
    }
  }
}