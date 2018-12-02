using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Funda.Core.Entities;
using Funda.Core.Repositories;
using Funda.Core.Usecases;

using Moq;

using Xunit;

namespace Core.Tests.Usecases
{
    public class GetTop10MakelaarsInAmsterdamUseCaseTests
    {
        private readonly Mock<IFundaApiService> _fundaApiServiceMock;

        public GetTop10MakelaarsInAmsterdamUseCaseTests()
        {
            _fundaApiServiceMock = new Mock<IFundaApiService>(MockBehavior.Strict);
        }

        [Fact]
        public async Task Execute_WhenServiceCallSucceeded_ReturnsSortedResults()
        {
            var makelaarOne = new Makelaar(1, "makelaarOne");
            IEnumerable<Listing> listings = CreateListingsForMakelaar(makelaarOne, 40);

            _fundaApiServiceMock.Setup(m => m.GetAllListings("Amsterdam", false))
                                .ReturnsAsync(listings);

            GetTop10MakelaarsInAmsterdamUseCase sut = CreateSut();

            IEnumerable<Tuple<Makelaar, int>> result = await sut.Execute();

            result.Should().ContainSingle().Which.Should().BeEquivalentTo(new Tuple<Makelaar, int>(makelaarOne, 40));
        }

        [Theory]
        [InlineData(new[] {10, 80, 200, 500, 54, 1000, 23, 57, 1231, 345321, 1, 25, 786})]
        [InlineData(new[] {30, 45, 23, 12})]
        [InlineData(new[] {46, 7, 12, 100, 3, 55, 8})]
        public async Task Execute_WhenServiceCallSucceeded_WithMoreThan10Makelaars_ReturnsOnlyTop10Sorted(int[] listingsPerMakelaar)
        {
            var listings = new List<Listing>();
            var makelaars = new Dictionary<int, Makelaar>();

            foreach (var numberOfListings in listingsPerMakelaar)
            {
                var buildResult = Build(numberOfListings, numberOfListings);
                listings.AddRange(buildResult.Item2);
                makelaars[numberOfListings] = buildResult.Item1;
            }

            IEnumerable<Tuple<Makelaar, int>> sortedMakelaars = makelaars.OrderByDescending(m => m.Key)
                                                                         .Take(10)
                                                                         .Select(m => new Tuple<Makelaar, int>(m.Value, m.Key));

            _fundaApiServiceMock.Setup(m => m.GetAllListings("Amsterdam", false))
                                .ReturnsAsync(listings);

            GetTop10MakelaarsInAmsterdamUseCase sut = CreateSut();

            var result = await sut.Execute();

            result.Should().HaveCountLessOrEqualTo(10);
            result.Should().ContainInOrder(sortedMakelaars);
        }

        private Tuple<Makelaar, IEnumerable<Listing>> Build(int makelaarId, int numberOfListings)
        {
            var makelaar = new Makelaar(makelaarId, makelaarId.ToString());
            IEnumerable<Listing> listings = CreateListingsForMakelaar(makelaar, numberOfListings);
            return new Tuple<Makelaar, IEnumerable<Listing>>(makelaar, listings);
        }

        private static IEnumerable<Listing> CreateListingsForMakelaar(Makelaar makelaar, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new Listing(Guid.NewGuid(), makelaar);
            }
        }

        private GetTop10MakelaarsInAmsterdamUseCase CreateSut()
            => new GetTop10MakelaarsInAmsterdamUseCase(_fundaApiServiceMock.Object);
    }
}