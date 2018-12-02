using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Funda.Core.Entities;
using Funda.Core.Repositories;

using FundaWebApiServiceAdapter.Dtos;

using Microsoft.Extensions.Logging;

namespace FundaWebApiServiceAdapter
{
    internal sealed class FundaWebApiService : IFundaApiService
    {
        private readonly ILogger<FundaWebApiService> _logger;
        private readonly HttpClient _client;

        public FundaWebApiService(ILogger<FundaWebApiService> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<IEnumerable<Listing>> GetAllListings(string city, bool withTuin)
        {
            var pageNumber = 0;
            var listings = new List<Listing>();
            QueryResultDto content;
            
            const string baseUri = "type=#TYPE#&zo=/#CITY#/#WITHTUIN#/";

            var uri = baseUri.Replace("#TYPE#", "koop")
                             .Replace("#CITY#", city)
                             .Replace("#WITHTUIN#", withTuin ? "tuin" : string.Empty);

            do
            {
                string url = $"?{uri}&page={++pageNumber}&pagesize=25";

                content = await ExecuteQuery(url);

                listings.AddRange(content.Listings.Select(l => l.ToEntity()));
            } while (pageNumber < content.PagingDto.AantalPaginas);

            return listings;
        }

        private async Task<QueryResultDto> ExecuteQuery(string url, int retry = 0)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (retry >= 4)
            {
                _logger.LogError("Reached maximum number of retries when executing request {url}", url);
                throw new Exception("Max retries reached.");
            }

            try
            {
                HttpResponseMessage response = await _client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string stringContent = await response.Content.ReadAsStringAsync();

                    return QueryResultDto.FromJson(stringContent);
                }
                else if (retry < 4 && response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Status code: {StatusCode} on attempt {attempt} for url {url}",
                                       response.StatusCode, retry, url);
                    await Task.Delay(TimeSpan.FromMinutes(1));
                    return await ExecuteQuery(url, ++retry);
                }

                _logger.LogError("Failed calling the endpoint {url}, response has status code {StatusCode}", url,
                                 response.StatusCode);
                throw new Exception(
                    $"Failed calling the endpoint {url}, response has status code {response.StatusCode}");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Exception thrown when retrieving the results for the query.");
                throw;
            }
        }
    }
}