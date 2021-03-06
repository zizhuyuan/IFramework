﻿using Newtonsoft.Json.Serialization;
using IFramework.Config;
using IFramework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IFramework.AspNet.MediaTypeFormatters
{
    public class CommandMediaTypeFormatter : JsonMediaTypeFormatter
    {
        static readonly string CommandTypeTemplate = Configuration.GetAppConfig("CommandTypeTemplate");
        bool _useCamelCase;
        public CommandMediaTypeFormatter(bool useCamelCase = true)
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/command"));
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/command+form"));
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/x-www-form-urlencoded"));
            _useCamelCase = useCamelCase;
            if (_useCamelCase)
            {
                this.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }
        }
        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }
        public override Task WriteToStreamAsync(Type type, object value, System.IO.Stream writeStream, HttpContent content, System.Net.TransportContext transportContext)
        {
            return base.WriteToStreamAsync(value.GetType(), value, writeStream, content, transportContext);
        }

        Type GetCommandType(string commandType)
        {
            var type = Type.GetType(commandType);
            if (type == null)
            {
                type = Type.GetType(string.Format(CommandTypeTemplate,
                                                         commandType));
            }
            return type;
        }

        public async override Task<object> ReadFromStreamAsync(Type type, System.IO.Stream readStream, HttpContent content, System.Net.Http.Formatting.IFormatterLogger formatterLogger)
        {
            var commandType = type;
            if (type.IsAbstract || type.IsInterface)
            {
                var commandContentType = content.Headers.ContentType.Parameters.FirstOrDefault(p => p.Name == "command");
                if (commandContentType != null)
                {
                    commandType = GetCommandType(HttpUtility.UrlDecode(commandContentType.Value));
                }
                else
                {
                    commandType = GetCommandType(HttpContext.Current.Request.Url.Segments.Last());
                }
            }
            var part = await content.ReadAsStringAsync();
            var mediaType = content.Headers.ContentType.MediaType;
            object command = null;
            if (mediaType == "application/x-www-form-urlencoded" || mediaType == "application/command+form")
            {
                command = new FormDataCollection(part).ConvertToObject(commandType);
            }
            if (command == null)
            {
                command = part.ToJsonObject(commandType, useCamelCase: _useCamelCase);
            }
            return command;
        }
    }
}
