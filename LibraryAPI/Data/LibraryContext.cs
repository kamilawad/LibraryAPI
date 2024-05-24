using LibraryAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) 
        {
        }

        public DbSet<Book> Books { get; set; }
    }
}
