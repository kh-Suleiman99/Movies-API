namespace MoviesApi.Entities
{
    public class UserMovie
    {
        public string UserId { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public ApplicationUser User { get; set; }

    }
}
