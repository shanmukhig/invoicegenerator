using System.Web.Http;
using InvoiceGenerator.Data;
using InvoiceGenerator.Entities;

namespace InvoiceGenerator.API.Controllers
{
  public abstract class BaseApiController<T> : ApiController where T : BaseEntity
  {
    protected readonly IRepository<T> Repository;

    protected BaseApiController(IRepository<T> repository)
    {
      Repository = repository;
    }
  }
}