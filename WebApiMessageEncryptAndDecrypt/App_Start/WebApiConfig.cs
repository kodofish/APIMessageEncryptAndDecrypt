using System.Web.Http;

namespace WebApiMessageEncryptAndDecrypt
{
    /// <summary>
    ///     Web API Config
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        ///     Registers the specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API 設定和服務
            // Enable Web API 屬性路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional});
        }
    }
}