using System;
using System.Net.Http.Headers;

using Funda.Core.Repositories;

using Microsoft.Extensions.DependencyInjection;

namespace FundaWebApiServiceAdapter
{
    public static class WebApiServiceAdapter
    {
        private const string _apiKey = "";
        
        public static IServiceCollection RegisterWebApiServiceAdapter(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpClient<IFundaApiService, FundaWebApiService>(
                client =>
                {
                    client.BaseAddress = new Uri($"http://partnerapi.funda.nl/feeds/Aanbod.svc/{_apiKey}");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                });
            return serviceCollection;
        }
    }
}