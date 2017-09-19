using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Should;
using WebApiMessageEncryptAndDecrypt.Filter;

namespace WebApiMessageEncryptAndDecrypt.Tests
{
    [TestFixture]
    public class MessageEncryptAndDencryptProcessTests
    {
        private MessageEncryptAndDencryptProcessUsingDelegatingHandler _target;
        private HttpMessageInvoker _client;
        private HttpRequestMessage _request;

        [SetUp]
        public void Setup()
        {
            _target = new MessageEncryptAndDencryptProcessUsingDelegatingHandler
            {
                InnerHandler = new FakeInnerHandler()
                {
                    Message = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("fake response.") }
                }
            };

            _client = new HttpMessageInvoker(_target);

            _request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:2188/api/Value/");
        }

        [Test]
        public void Request沒有帶加密Header時_應直接跳過不進行解密處理()
        {
            var actual = _client.SendAsync(_request, new CancellationToken()).Result;

            //Assert
            actual.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void Request有帶加密Header時_應進行解密處理()
        {
            _request.Headers.Add("X-Encrypt", "AES256");

            var actual = _client.SendAsync(_request, new CancellationToken()).Result;

            //Assert
            actual.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }
    }

    /// <summary>
    /// 模擬 假的 DelegatingHandler, 且無條件回傳 HttpStatus.OK 的
    /// </summary>
    /// <seealso cref="System.Net.Http.DelegatingHandler" />
    internal class FakeInnerHandler : DelegatingHandler
    {
        internal HttpResponseMessage Message { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Message == null)
            {
                return base.SendAsync(request, cancellationToken);
            }
            return Task.Factory.StartNew(() => Message, cancellationToken);
        }
    }
}
