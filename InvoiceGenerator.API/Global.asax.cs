using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using InvoiceGenerator.API.DependencyResolution;
using InvoiceGenerator.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StructureMap;
using StructureMap.Web;

namespace InvoiceGenerator.API
{
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
      GlobalConfiguration.Configure(WebApiConfig.Register);
      var formatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
      formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
      formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
      formatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
      //GlobalConfiguration.Configuration.Formatters.Add(new MultiPartMediaTypeFormatter());

      var container = GetContainer();
      GlobalConfiguration.Configuration.DependencyResolver = new StructureMapWebApiDependencyResolver(container);

      FilterProviders.Providers.Add(container.GetInstance<IFilterProvider>());
    }

    private IContainer GetContainer()
    {
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
      });
      return container;
    }
  }
}