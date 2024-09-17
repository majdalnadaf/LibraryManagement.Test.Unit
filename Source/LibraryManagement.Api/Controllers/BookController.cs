using LibraryManagement.Api.Contract.Books;
using LibraryManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Domain.Domains;
using FluentValidation;
using LibraryManagement.Api.Contract.ValidationRules;
using System.Data;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {

        private readonly IBookManagement _bookManagement;
        private readonly IValidator<CreateBookRequest> _createBookRequestValidator;
        private readonly IValidator<UpdateBookRequest> _updateBookRequestValidtor;


        public BookController(IBookManagement bookManagement, IValidator<CreateBookRequest> createBookRequestValidator,
             IValidator<UpdateBookRequest> updateBookRequestValidtor)
        {
            _bookManagement = bookManagement;
            _createBookRequestValidator = createBookRequestValidator;
            _updateBookRequestValidtor = updateBookRequestValidtor;
        }

        // GET: api/<BookController>
        [HttpGet("{name}")]
        public async Task<IActionResult> GetAllBookByNameAsync(string name)
        {

            var lstBook = await _bookManagement.SearchForBookByBookNameAsync(name);
            BookResponse<List<Book>> response;

            if (lstBook is null)
            {
                response = new(lstBook!, "Not found");
                return BadRequest(response);
            }


            response = new(lstBook, "Success");
            return Ok(response);
        }



        // POST api/<BookController>
        [HttpPost]
        public async Task<IActionResult> CreateBookAsync([FromBody] CreateBookRequest request)
        {
            var validaionResult = _createBookRequestValidator.Validate(request);
            if (!validaionResult.IsValid)
            {
                return BadRequest(validaionResult);
            }

            var book = new Book()
            {
                Id = Guid.NewGuid(),
                Title = request.title,
                Author = request.author,
                ISBN = request.ISBN,
                Description = request.description,
                IsAvailable = true // by default

            };


            var result = await _bookManagement.CreateBookAsync(book);
            if (result.Item1 is false)
            {
                return BadRequest("The book wan not created");
            }


            BookResponse<Guid> response = new(data: result.Item2, "Book was created successfully");

            return CreatedAtAction(nameof(CreateBookAsync), response);

        }


        [HttpPost]
        public async Task<IActionResult> UpdateBookAsync([FromBody] UpdateBookRequest request)
        {
            var validationResult = _updateBookRequestValidtor.Validate(request);
            if (validationResult.IsValid is false)
            {
                return BadRequest(validationResult);
            }

            var book = new Book()
            {
                Id = request.id,
                Title = request.title,
                Author = request.author,
                ISBN = request.ISBN,
                Description = request.description,
                IsAvailable = request.isAvailable
            };

            var result = await _bookManagement.UpdateBookAsync(book);
            if (result is false)
            {
                return Problem("Book wan not updated");
            }

            BookResponse<Book> response = new(data:book , message:"Book was updated");

            return Ok(response);
        }


        // DELETE api/<BookController>/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteBookAsync(Guid id)
        {

            var result = await _bookManagement.DeleteBookAsync(id);
            if (result is false)
            {
                return Problem("Book not found");
            }

            return Ok("Book was deleted");

        }
    }
}
