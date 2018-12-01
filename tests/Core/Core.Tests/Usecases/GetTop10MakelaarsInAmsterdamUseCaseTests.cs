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

            _fundaApiServiceMock.Setup(m => m.GetAllListings())
                                .ReturnsAsync(listings);

            GetTop10MakelaarsInAmsterdamUseCase sut = CreateSut();

            IEnumerable<Makelaar> result = await sut.Execute();

            result.Should().ContainSingle().Which.Should().Be(makelaarOne);
        }

        private IEnumerable<Listing> CreateListingsForMakelaar(Makelaar makelaar, int count)
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