using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace blandus_backend.Models.Service
{
    public class Service
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaxLength(50, ErrorMessage = "You have exceeded the maximum length of this field")]
        public string Name { get; set; } = string.Empty;

        // props for relationship configuration
        public Guid AccommodationId { get; set; }

        [JsonIgnore]
        public Accommodation.Accommodation? Accommodation { get; set; }
    }
}
