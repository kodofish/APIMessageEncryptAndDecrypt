using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace WebApiMessageEncryptAndDecrypt
{
    public class Global : HttpApplication
    {
        private readonly HttpConfiguration _httpConfig;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Global" /> class.
        /// </summary>
        public Global()
        {
            _httpConfig = GlobalConfiguration.Configuration;
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //Setting API Response format use JSON format

            var jsonFormatter = _httpConfig.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}