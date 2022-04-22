using Microsoft.AspNetCore.Mvc;

namespace blandus_backend.Models.Accommodation
{
    public class AccommodationFilter
    {
        [FromQuery(Name = "type")]
        public string? Type { get; set; } = string.Empty;

        [FromQuery(Name = "rating")]
        public decimal? Rating { get; set; }

        [FromQuery(Name = "service")]
        public string? Service { get; set; } = string.Empty;
    }
}
