namespace blandus_backend.Models.User
{
    public class PaginationUserResponse
    {
        public PaginationUserResponse(List<UserOutModel> users, int pages, int currentPage)
        {
            Users = users;
            Pages = pages;
            CurrentPage = currentPage;
        }
        public List<UserOutModel> Users { get; set; }

        public int Pages { get; set; }

        public int CurrentPage { get; set; }
    }
}
