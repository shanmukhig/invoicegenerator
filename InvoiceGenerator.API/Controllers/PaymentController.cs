using System.Threading.Tasks;
using System.Web.Http;
using InvoiceGenerator.Data;
using InvoiceGenerator.Entities;

namespace InvoiceGenerator.API.Controllers
{
  public class PaymentController : BaseApiController<Payment>
  {
    public PaymentController(IRepository<Payment> repository) : base(repository)
    {
    }

    [HttpGet]
    [Route("api/payment")]
    public async Task<IHttpActionResult> GetCustomers()
    {
      var customers = await Repository.GetAll().ConfigureAwait(false);
      return Ok(customers);
    }

    [HttpGet]
    [Route("api/payment/{id}")]
    public async Task<IHttpActionResult> GetCustomer(string id)
    {
      return Ok(await Repository.GetById(id).ConfigureAwait(false));
    }

    [HttpPost]
    [Route("api/payment")]
    public async Task<IHttpActionResult> AddCustomer([FromBody] Payment payment)
    {
      return Ok(await Repository.AddOrUpdate(null, payment).ConfigureAwait(false));
    }

    [HttpPut]
    [Route("api/payment/{id}")]
    public async Task<IHttpActionResult> UpdateCustomer([FromUri] string id, [FromBody] Payment payment)
    {
      return Ok(await Repository.AddOrUpdate(id, payment).ConfigureAwait(false));
    }

    [HttpDelete]
    [Route("api/payment/{id}")]
    public async Task<IHttpActionResult> DeleteCustomer(string id)
    {
      return Ok(await Repository.Delete(id).ConfigureAwait(false));
    }
  }
}