using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using FluentValidation;
using FluentValidation.WebApi;
using InvoiceGenerator.API.Controllers;
using InvoiceGenerator.API.DependencyResolution;
using InvoiceGenerator.Data;
using InvoiceGenerator.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StructureMap;
using StructureMap.Web;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;
//using IFilterProvider = System.Web.Http.Filters.IFilterProvider;

namespace InvoiceGenerator.API
{
  //public class ResponseWrappingHandler : DelegatingHandler
  //{
  //  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  //  {
  //    HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

  //    return await BuildResponse(request, response);
  //  }

  //  private async Task<HttpResponseMessage> BuildResponse(HttpRequestMessage request, HttpResponseMessage response)
  //  {
  //    object content;
  //    List<string> modelStateErrors = new List<string>();

  //    if (response.TryGetContentValue(out content) && !response.IsSuccessStatusCode)
  //    {
  //      HttpError error = content as HttpError;
  //      if (error != null)
  //      {
  //        content = null;
  //        if (error.ModelState != null)
  //        {
            
  //        }
  //      }
  //    }
  //    return await Task.FromResult(response);
  //  }
  //}

  public class ValiateModalStateFilterAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(HttpActionContext filterContext)
    {
      if (!filterContext.ModelState.IsValid)
      {
        filterContext.Response = filterContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
          filterContext.ModelState);
      }
    }
  }

  public class StructureMapFilterProvider : FilterAttributeFilterProvider
  {
    private readonly IContainer container;

    public StructureMapFilterProvider(IContainer container)
    {
      this.container = container;
    }

    public override IEnumerable<Filter> GetFilters(ControllerContext controllerContext,
      ActionDescriptor actionDescriptor)
    {
      var filters = base.GetFilters(controllerContext, actionDescriptor).ToList();
      filters.ForEach(filter => container.BuildUp(filter));
      return filters;
    }
  }

  public class WebApiApplication : HttpApplication
  {
    protected void Application_Start()
    {
      var container = GetContainer();

      GlobalConfiguration.Configure(WebApiConfig.Register);
      var formatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
      formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
      formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
      formatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
      GlobalConfiguration.Configuration.Filters.Add(new ValiateModalStateFilterAttribute());
      GlobalConfiguration.Configuration.Filters.Add(container.GetInstance<ApiAuthorizeAttribute>());
      //GlobalConfiguration.Configuration.MessageHandlers.Add(new ResponseWrappingHandler());

      GlobalConfiguration.Configuration.DependencyResolver = new StructureMapWebApiDependencyResolver(container);
      FilterProviders.Providers.Add(container.GetInstance<IFilterProvider>());
      FluentValidationModelValidatorProvider.Configure(GlobalConfiguration.Configuration);
    }

    //private void CreateMapper()
    //{
    //  Mapper.Initialize(expression => expression.CreateMap<Invoice, Invoice>()
    //    .ForMember(invoice => invoice.PdfStream, opt => opt.Ignore())
    //    .ForAllMembers(cfg => cfg.UseDestinationValue())
    //    );

    //  //MapperConfiguration configuration =
    //  //  new MapperConfiguration(expression =>
    //  //    expression.AddProfile(new MappingProfile()));
    //  //return configuration.CreateMapper();
    //}

    private IContainer GetContainer()
    {
      //CreateMapper();

      var connectionString = ConfigurationManager.AppSettings["connectionString"];
      IContainer container = new Container(expression =>
      {
        expression.For<IFilterProvider>().Use<StructureMapFilterProvider>();
        expression.For(typeof(IRepository<>))
          .HybridHttpOrThreadLocalScoped()
          .Use(typeof(Repository<>))
          .Ctor<string>("connectionString")
          .Is(connectionString)
          .Ctor<string>("databaseName")
          .Is(ConfigurationManager.AppSettings["databaseName"]);
        expression.For<IUserRepository>()
          .HybridHttpOrThreadLocalScoped()
          .Use<UserRepository>()
          .Ctor<string>("connectionString")
          .Is(connectionString)
          .Ctor<string>("databaseName")
          .Is(ConfigurationManager.AppSettings["databaseName"]);
        expression.For<IInvoiceProcessor>()
          .HybridHttpOrThreadLocalScoped()
          .Use<InvoiceProcessor>()
          .Ctor<string>("path")
          .Is(Path.GetDirectoryName(Uri.UnescapeDataString((new UriBuilder(Assembly.GetExecutingAssembly().CodeBase)).Path)));
        expression.For<IHashHelper>().HybridHttpOrThreadLocalScoped().Use<HashHelper>();
        expression.For<ApiAuthorizeAttribute>().HybridHttpOrThreadLocalScoped().Use<ApiAuthorizeAttribute>();
        //expression.For<IValidatorFactory>().HybridHttpOrThreadLocalScoped().Use<ContainerValidatorFactory>();
        expression.ForConcreteType<ApiAuthorizeAttribute>().Configure.Setter<IHashHelper>(attribute => attribute.HashHelper).IsTheDefault();
        expression.For(typeof(IValidator<>)).HttpContextScoped().Use(typeof(StopOnFirstFailureValidator<>));

        //expression.For<IMapper>().HybridHttpOrThreadLocalScoped().Use(Mapper.Instance);
      });
      return container;
    }
  }
}