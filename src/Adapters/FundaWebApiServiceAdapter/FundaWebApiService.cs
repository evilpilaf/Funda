using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Funda.Core.Entities;
using Funda.Core.Repositories;

using FundaWebApiServiceAdapter.Dtos;

namespace FundaWebApiServiceAdapter
{
    internal sealed class FundaWebApiService : IFundaApiService
    {
        private readonly HttpClient _client;
        private const string apiKey = " ";

        public FundaWebApiService(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<Listing>> GetAllListings()
        {
            string uri = $"/feeds/Aanbod.svc/{apiKey}/?type=koop&zo=/amsterdam/tuin/&page=1&pagesize=25";

            QueryResultDto content = await ExecuteQuery(uri);

            var listings = new List<Listing>(content.TotaalAantalObjecten);

            listings.AddRange(content.Listings.Select(l => l.ToEntity()));

            while (!String.IsNullOrWhiteSpace(content.PagingDto.VolgendeUrl))
            {
                string requestUrl = content.PagingDto.VorigeUrl;
                content = await ExecuteQuery(requestUrl);
            }

            return listings;
        }

        private async Task<QueryResultDto> ExecuteQuery(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            HttpResponseMessage response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            string stringContent = await response.Content.ReadAsStringAsync();

            return QueryResultDto.FromJson(stringContent);   
        }
    }
}