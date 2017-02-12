using System;
using System.Configuration;
using System.Web.Http;
using System.Xml;
using Newtonsoft.Json.Serialization;
using Microsoft.Practices.ServiceLocation;

namespace Amod.University.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            bool isDebugLogEnabled;
            
            #region Serialization Config

            config.Formatters.XmlFormatter.UseXmlSerializer = true;
            config.Formatters.XmlFormatter.WriterSettings.OmitXmlDeclaration = true;
            config.Formatters.XmlFormatter.WriterSettings.Indent = true;
            config.Formatters.XmlFormatter.WriterSettings.NamespaceHandling = NamespaceHandling.Default;

            config.Formatters.JsonFormatter.SerializerSettings.Formatting =
                Newtonsoft.Json.Formatting.Indented;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add
                (new Newtonsoft.Json.Converters.StringEnumConverter());
          
            #endregion

            #region Exceptions Config
            config.Filters.Add(new ApiExceptionAttribute());
            #endregion


            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                 defaults: new { id = RouteParameter.Optional });

        }
    }
}
