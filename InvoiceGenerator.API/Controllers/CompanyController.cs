using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Controllers;
using InvoiceGenerator.Data;
using InvoiceGenerator.Entities;
using Newtonsoft.Json;
using StructureMap.Attributes;

namespace InvoiceGenerator.API.Controllers
{
  public class ApiAuthorizeAttribute : AuthorizeAttribute
  {
    [SetterProperty]
    public IHashHelper HashHelper { get; set; }

    //public override bool AllowMultiple => false;

    //public override void OnAuthorization(HttpActionContext actionContext)
    //{
    //  base.OnAuthorization(actionContext);
    //}

    //public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
    //{
    //  return base.OnAuthorizationAsync(actionContext, cancellationToken);
    //}

    protected override bool IsAuthorized(HttpActionContext actionContext)
    {
      AuthenticationHeaderValue value = actionContext.Request.Headers.Authorization;
      if (string.IsNullOrWhiteSpace(value?.Parameter) || string.IsNullOrWhiteSpace(value.Scheme) || !value.Scheme.Equals("Bearer", StringComparison.InvariantCultureIgnoreCase))
      {
        return base.IsAuthorized(actionContext);
      }

      var segments = value.Parameter.Split('.');

      if (segments.Length != 3)
      {
        return base.IsAuthorized(actionContext);
      }

      string signature = HashHelper.Decode(segments, "secret");

      if (!string.Equals(signature, value.Parameter))
      {
        return base.IsAuthorized(actionContext);
      }

      IEnumerable<Claim> claims = new List<Claim>
      {
        new Claim("claim1", "claim1")
      };

      IIdentity identity = new GenericIdentity(string.Empty);
      IPrincipal principal = new GenericPrincipal(identity, new string[] {});
      Thread.CurrentPrincipal = principal;
      if (System.Web.HttpContext.Current != null)
      {
        System.Web.HttpContext.Current.User = principal;
      }

      return true;
    }
  }

  public class CompanyController : BaseApiController<Company>
  {
    public CompanyController(IRepository<Company> repository) : base(repository)
    {
      //hashHelper = new HashHelper();
    }

    [ApiAuthorize]
    [HttpGet]
    [Route("api/company")]
    public async Task<IHttpActionResult> GetCompanies()
    {
      IEnumerable<Company> companies = await Repository.GetAll().ConfigureAwait(false);

      //var result = from c in companies
      //  select new Company
      //  {
      //    Id = c.Id,
      //    Address = c.Address,
      //    Tax = c.Tax,
      //    Currency = c.Currency,
      //    Cheque = c.Cheque,
      //    Comments = c.Comments,
      //    CompanyName = c.CompanyName,
      //    Ifsc = c.Ifsc,
      //    Support = c.Support,
      //    Swift = c.Swift
      //  };

      return Ok(companies);
    }

    [HttpGet]
    [Route("api/company/{id}")]
    public async Task<IHttpActionResult> GetCompany(string id)
    {
      Company company = await Repository.GetById(id).ConfigureAwait(false);
      //company.Logo = null;
      return Ok(company);
    }

    [HttpGet]
    [Route("api/company/logo/{id}")]
    public async Task<HttpResponseMessage> GetLogo(string id)
    {
      byte[] buffer = await Repository.DownloadFileAsync(id).ConfigureAwait(false);

      if (buffer == null || buffer.Length == 0)
      {
        return new HttpResponseMessage(HttpStatusCode.NotFound);
      }

      ContentDispositionHeaderValue value = new ContentDispositionHeaderValue("attachment")
      {
        FileName = $"{id}.img"
      };

      HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = new ByteArrayContent(buffer)
      };

      message.Content.Headers.ContentDisposition = value;
      message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

      return message;
    }

    [ValiateModalStateFilter]
    [HttpPost]
    [Route("api/company")]
    public async Task<IHttpActionResult> AddCompany([FromBody] Company company)
    {

      //string name = company.Logo.Split(';')[0];
      //name = $"{company.CompanyName}.{name.Replace("data:image/", string.Empty)}";
      //company.FileId = await Repository.UploadFileAsync(company.FileId, name, buffer: Encoding.ASCII.GetBytes(company.Logo));
      //company.Logo = null;
      await UploadLogo(company);
      return Ok(await Repository.AddOrUpdate(null, company).ConfigureAwait(false));
    }

    private async Task UploadLogo(Company company)
    {
      string name = company.Logo.Split(';')[0];
      name = $"{company.CompanyName}.{name.Replace("data:image/", string.Empty)}";
      company.FileId = await Repository.UploadFileAsync(company.FileId, name, buffer: Encoding.ASCII.GetBytes(company.Logo));
      company.Logo = null;
    }

    [ValiateModalStateFilter]
    [HttpPut]
    [Route("api/company/{id}")]
    public async Task<IHttpActionResult> UpdateCompany([FromUri] string id, [FromBody] Company company)
    {
      await UploadLogo(company);
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