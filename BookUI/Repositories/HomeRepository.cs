using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookUI.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            this._db = db; // connected to database
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _db.Genres.ToListAsync();
        }
            
        public async Task<IEnumerable<Book>> GetBooks(string sTerm = "", int genreId=0) // method to get data from database.
        {
            sTerm = sTerm.ToLower();
             IEnumerable<Book> books = await(from book in _db.Bookks
                         join genre in _db.Genres // joining to tables Book and genre.
                         on book.GenreId equals genre.Id
                         where string.IsNullOrWhiteSpace(sTerm) || (book != null && book.BookName.ToLower().StartsWith(sTerm)) // search by letters dont need to write the whole book name.
                         select new Book 
                         { 
                            Id= book.Id,
                            Image= book.Image,
                            AuthorName= book.AuthorName,
                            BookName= book.BookName,
                            GenreId= book.GenreId,
                            Price= book.Price,
                            GenreName= genre.GenreName,
                         }).ToListAsync();
            if (genreId > 0)
            { 
                books =  books.Where(a => a.GenreId == genreId).ToList();
            }
            return books;
        }
    }
}
