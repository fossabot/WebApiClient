﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using WebApiClient.Interfaces;
using Xunit;

namespace WebApiClientTest.Attributes.HttpActionAttributes
{
    public class XmlContentAttributeTest
    {
        public interface IMyApi : IDisposable
        {
            ITask<HttpResponseMessage> PostAsync(object content);
        }

        public class Model
        {
            public string name { get; set; }

            public DateTime birthDay { get; set; }
        }

        [Fact]
        public async Task BeforeRequestAsyncTest()
        {
            var context = new ApiActionContext
            {
                HttpApiConfig = new HttpApiConfig(),
                RequestMessage = new HttpApiRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("http://www.webapi.com/")
                },
                ApiActionDescriptor = ApiDescriptorCache.GetApiActionDescriptor(typeof(IMyApi).GetMethod("PostAsync"))
            };

            var parameter = context.ApiActionDescriptor.Parameters[0];
            parameter.Value = new Model
            {
                name = "laojiu",
                birthDay = DateTime.Parse("2010-10-10")
            };

            var attr = new XmlContentAttribute();
            await ((IApiParameterAttribute)attr).BeforeRequestAsync(context, parameter);

            var body = await context.RequestMessage.Content.ReadAsStringAsync();
            var target = context.HttpApiConfig.XmlFormatter.Serialize(parameter.Value, Encoding.UTF8);
            Assert.True(body == target);
        }
    }
}

