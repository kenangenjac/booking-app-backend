using System.Text.Json.Serialization;

namespace blandus_backend.Models.Image
{
    public class Image
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Url { get; set; } = string.Empty;

        // props for relationship configuration
        public Guid AccommodationId { get; set; }
        [JsonIgnore]
        public Accommodation.Accommodation? Accommodation { get; set; }
    }
}
