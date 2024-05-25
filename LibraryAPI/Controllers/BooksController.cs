using LibraryAPI.Data;
using LibraryAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.Swagger.Annotations;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
        {
            var books = await _context.Books.ToListAsync();

            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book is null)
                return BadRequest("Book not found.");

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> AddBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Book>> UpdateBook(int id, Book updatedBook)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbBook = await _context.Books.FindAsync(id);
            if (dbBook is null)
                return BadRequest("Book not found.");

            dbBook.Title = updatedBook.Title;
            dbBook.Author = updatedBook.Author;
            dbBook.ISBN = updatedBook.ISBN;
            dbBook.PublishedDate = updatedBook.PublishedDate;

            await _context.SaveChangesAsync();

            return Ok(dbBook);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Book>> DeleteBook(int id)
        {
            var dbBook = await _context.Books.FindAsync(id);
            if (dbBook is null)
                return BadRequest("Book not found.");

           _context.Books.Remove(dbBook);
            await _context.SaveChangesAsync();

            return Ok(dbBook);
        }
    }
}
