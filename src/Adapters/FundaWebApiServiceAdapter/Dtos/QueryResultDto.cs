using System;
using System.Collections.Generic;
using System.Globalization;

using Funda.Core.Entities;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FundaWebApiServiceAdapter.Dtos
{
    internal class QueryResultDto
    {
        public List<ListingDto> Listings { get; set; }

        [JsonProperty("Paging")]
        public PagingDto PagingDto { get; set; }

        [JsonProperty("TotaalAantalObjecten")]
        public int TotaalAantalObjecten { get; set; }

        public static QueryResultDto FromJson(string json)
            => JsonConvert.DeserializeObject<QueryResultDto>(json, Converter.Settings);
    }

    internal class ListingDto
    {
        [JsonProperty("Id")]
        public Guid Id { get; set; }

        [JsonProperty("MakelaarId")]
        public int MakelaarId { get; set; }

        [JsonProperty("MakelaarNaam")]
        public string MakelaarNaam { get; set; }

        [JsonProperty("ProjectNaam")]
        public string ProjectNaam { get; set; }

        [JsonProperty("Woonplaats")]
        public string Woonplaats { get; set; }

        public Listing ToEntity()
            => new Listing(Id, new Makelaar(MakelaarId, MakelaarNaam));
    }

    internal class PagingDto
    {
        [JsonProperty("AantalPaginas")]
        public int AantalPaginas { get; set; }

        [JsonProperty("HuidigePagina")]
        public int HuidigePagina { get; set; }

        [JsonProperty("VolgendeUrl")]
        public string VolgendeUrl { get; set; }

        [JsonProperty("VorigeUrl")]
        public string VorigeUrl { get; set; }
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            },
        };
    }
}