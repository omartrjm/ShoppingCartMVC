namespace BookUI.Models.NewFolder
{
    public class BookDisplayModel
    {
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public string sTerm { get; set; } = "";
        public int genreId { get; set; } = 0;

    }
}
