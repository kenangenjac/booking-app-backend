using System.ComponentModel.DataAnnotations.Schema;

namespace blandus_backend.Models.Accommodation
{
    [NotMapped]
    public class PaginationAccommodationResponse
    {
        public PaginationAccommodationResponse(List<OutAccommodation> lOA, int pages, int currentPage)
        {
            ResponseAccommodations = lOA;
            Pages = pages;
            CurrentPage = currentPage;
        }
        public List<OutAccommodation> ResponseAccommodations { get; set; }

        public int Pages { get; set; }

        public int CurrentPage { get; set; }
    }
}
