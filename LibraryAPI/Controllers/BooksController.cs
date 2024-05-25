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

        /// <summary>
        /// Gets the list of all books
        /// </summary>
        /// <returns>The list of books</returns>
        /// /// <response code="200">Returns the list of books.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
        {
            var books = await _context.Books.ToListAsync();

            return Ok(books);
        }

        /// <summary>
        /// Gets a specific book by ID.
        /// </summary>
        /// <param name="id">The ID of the book to retrieve.</param>
        /// <returns>The requested book.</returns>
        /// <response code="200">Returns the requested book.</response>
        /// <response code="404">If the book is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book is null)
                return NotFound("Book not found");
            return Ok(book);
        }

        /// <summary>
        /// Creates a book.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Post api/Books
        ///     {
        ///         "Title": "My 60 Memorable Games",
        ///         "Author": "Bobby Fischer",
        ///         "ISBN": "978-1906388300",
        ///         "PublishedDate": "1969-01-01T00:00:00"
        ///     }
        /// </remarks>
        /// <param name="book"></param>
        /// <returns>A newly created book</returns>
        /// <response code="201">Returns the newly created book</response>
        /// <response code="400">If the book is null</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Produces("application/json")]
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

        /// <summary>
        /// Updates an existing book.
        /// </summary>
        /// /// <remarks>
        /// Sample request:
        /// 
        ///     Put /api/Books
        ///     {
        ///         "Title": "Introduction to Algorithms",
        ///         "Author": "Thomas H. Cormen, Charles E. Leiserson, Ronald L. Rivest, Clifford Stein",
        ///         "ISBN": "978-0262033848",
        ///         "PublishedDate": "2009-07-31T00:00:00"
        ///     }
        /// </remarks>
        /// <param name="id">The ID of the book to update.</param>
        /// <param name="updatedBook">The updated book details.</param>
        /// <returns>No content</returns>
        /// <response code="204">Book updated successfully</response>
        /// <response code="400">If the book details are invalid.</response>
        /// <response code="404">If the book is not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Book>> UpdateBook(int id, Book updatedBook)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbBook = await _context.Books.FindAsync(id);
            if (dbBook is null)
                return NotFound("Book not found.");

            dbBook.Title = updatedBook.Title;
            dbBook.Author = updatedBook.Author;
            dbBook.ISBN = updatedBook.ISBN;
            dbBook.PublishedDate = updatedBook.PublishedDate;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific book by ID.
        /// </summary>
        /// <param name="id">The ID of the book to delete.</param>
        /// <returns>The deleted book.</returns>
        /// <response code="200">Returns the deleted book.</response>
        /// <response code="404">If the book is not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Book>> DeleteBook(int id)
        {
            var dbBook = await _context.Books.FindAsync(id);
            if (dbBook is null)
                return NotFound("Book not found.");

           _context.Books.Remove(dbBook);
            await _context.SaveChangesAsync();

            return Ok(dbBook);
        }
    }
}
