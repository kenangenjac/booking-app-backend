namespace blandus_backend.Models.Accommodation
{
    public class OutAccommodation
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string RoomType { get; set; } = string.Empty;

        public string Contact { get; set; } = string.Empty;

        public double Price { get; set; }

        public int StarRating { get; set; }

        public decimal Rating { get; set; }

        public List<Review.Review>? Reviews { get; set; } = new();

        public List<Service.Service>? Services { get; set; } = new();

        public List<Image.Image>? Images { get; set; } = new();

        public List<Reservation.Reservation>? Reservations { get; set; } = new();

        public List<DatesOccupied.DatesOccupied>? DatesOccupied { get; set; } = new();

        public string Owner { get; set; } = string.Empty;
    }
}
