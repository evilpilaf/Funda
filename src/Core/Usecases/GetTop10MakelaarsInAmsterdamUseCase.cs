using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Funda.Core.Entities;
using Funda.Core.Repositories;

namespace Funda.Core.Usecases
{
    public sealed class GetTop10MakelaarsInAmsterdamUseCase
    {
        private readonly IFundaApiService _fundaApiService;

        public GetTop10MakelaarsInAmsterdamUseCase(IFundaApiService fundaApiService)
        {
            _fundaApiService = fundaApiService;
        }

        public async Task<IEnumerable<Tuple<Makelaar, int>>> Execute()
        {
            IEnumerable<Listing> listings = await _fundaApiService.GetAllListings("Amsterdam", false);

            return listings.GroupBy(l => l.Makelaar)
                           .Select(group => new
                           {
                               Makelaar = group.Key,
                               TotalListings = group.Count()
                           })
                           .OrderByDescending(r => r.TotalListings)
                           .Take(10)
                           .Select(g => new Tuple<Makelaar, int>(g.Makelaar, g.TotalListings));
        }
    }
}