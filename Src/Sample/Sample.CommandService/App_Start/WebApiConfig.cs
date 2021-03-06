﻿
using Newtonsoft.Json.Serialization;
using IFramework.AspNet.MediaTypeFormatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Sample.CommandService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //传输以JSON格式
            //config.Formatters.Clear();
            //config.Formatters.Add(new JsonFormatter());
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            //config.Formatters.Add(new JsonFormatter());
            //config.Formatters.Add(new StackTextJsonMediaTypeFormatter());
            //config.Formatters.Add(config.Formatters.JsonFormatter);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";

            config.Formatters.Insert(0, new MultipartMediaTypeFormatter());
            config.Formatters.Insert(0, new CommandMediaTypeFormatter());
            config.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{command}",
               defaults: new { command = RouteParameter.Optional }
            );
            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();
        }
    }
}
