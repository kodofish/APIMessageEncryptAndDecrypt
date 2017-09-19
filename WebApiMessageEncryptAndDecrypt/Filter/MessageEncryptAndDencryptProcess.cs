using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ServiceStack;
using WebApiMessageEncryptAndDecrypt.Service;

namespace WebApiMessageEncryptAndDecrypt.Filter
{
    public class MessageEncryptAndDencryptProcess : MessageProcessingHandler
    {
        protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class MessageEncryptAndDencryptProcessUsingDelegatingHandler : DelegatingHandler
    {
        private readonly EncryptionService _encryptService;

        public MessageEncryptAndDencryptProcessUsingDelegatingHandler()
        {
            var securitySetting = new SecuritySettings()
            {
                EncryptionKey = "9mWC]o4cLWgnBT>ZTsG2pJDVqDo{$Zk6Gt6n]zyL",
                IvKey = "$rjVqq3nnAPZrHiBmACy4i2%Uoy$pM+9iRi4LQ[H"
            };
             _encryptService = new EncryptionService(securitySetting);
        }
        /// <summary>
        /// 將 HTTP 要求傳送到內部的處理常式，以傳送到伺服器以非同步作業。
        /// </summary>
        /// <param name="request">要傳送至伺服器的 HTTP 要求訊息。</param>
        /// <param name="cancellationToken">取消語彙基元來取消作業。</param>
        /// <returns>
        /// 傳回 <see cref="T:System.Threading.Tasks.Task`1" />。工作物件，表示非同步作業。
        /// </returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_encryptService == null)
                return base.SendAsync(request, cancellationToken);

            var isUseAes256Encrypt = request.Headers.TryGetValues("X-Encrypt", out var encrypt);

            if (!(isUseAes256Encrypt && encrypt.Contains("AES256"))) return base.SendAsync(request, cancellationToken);

            #region "Decrypt Reqeuest Content"
            //if (request.Content.IsMimeMultipartContent())
            //    return base.SendAsync(request, cancellationToken);
            var hasContent = request.Content != null && !request.Content.ReadAsStringAsync().Result.IsNullOrEmpty();
            if (hasContent)
            {
                var content = request.Content.ReadAsStringAsync().Result;
                var decryptText = _encryptService.DecryptText_Aes(content);

                decryptText = decryptText.UrlDecode();
                request.Content = new StringContent(decryptText);
            }

            #endregion

            #region "Decrypt Request Url"

            var hasQueryString = !request.RequestUri.Query.IsNullOrEmpty();

            if (hasQueryString)
            {
                var decryptQuery = request.RequestUri.Query.Substring(1);
                decryptQuery = decryptQuery.UrlDecode();
                decryptQuery = _encryptService.DecryptText_Aes(decryptQuery);
                request.RequestUri = new Uri($"{request.RequestUri.AbsoluteUri.Split('?')[0]}?{decryptQuery}");
            }

            #endregion

            return base.SendAsync(request, cancellationToken).ContinueWith(
                task =>
                {
                    var response = task.Result;

                    if (_encryptService == null) return response;

                    var isOk = response.StatusCode == HttpStatusCode.OK ||
                               response.StatusCode == HttpStatusCode.Created ||
                               response.StatusCode == HttpStatusCode.Accepted;
                    if (!isOk) return response;

                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    var encryptContent = _encryptService.EncryptText_Aes(responseContent);
                    response.Content = new StringContent(encryptContent);

                    return response;
                }, cancellationToken);
        }
    }
}