using System.Collections.Generic;
using System.Threading.Tasks;

using Funda.Core.Entities;

namespace Funda.Core.Repositories
{
    public interface IFundaApiService
    {
        Task<IEnumerable<Listing>> GetAllListings(string city, bool withTuin);
    }
}