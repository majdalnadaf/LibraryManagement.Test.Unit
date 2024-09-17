using LibraryManagement.Api.Contract.Borrowing;
using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowingController : ControllerBase
    {

        private readonly IBorrowingManagement _borrowingManagement;
        public BorrowingController(IBorrowingManagement borrowingManagement)
        {
                _borrowingManagement = borrowingManagement;
        }



        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBookAsync([FromBody] BorrowBookRequest request)
        {



            var result = await _borrowingManagement.BorrowBook(memberId: request.memberId, bookId: request.bookId);
            if(result.Item1 is false)
            {
                return BadRequest(result.Item3); // Item3 is a message 
            }

            BorrowingResponse<DateTime> response = new(data: result.Item2 , message:  result.Item3);
            return Ok(response);

        }


        [HttpPost("return")]
        public async Task<IActionResult> ReturnBookAsync([FromBody] ReturnBookRequest request)
        {



            var result = await _borrowingManagement.ReturnBook(memberId: request.memberId, bookId: request.bookId, borrowBookId : request.borrowingId);
            if (result is false)
            {
                return BadRequest("Invalid data"); // Item3 is a message 
            }


            return Ok("Book was returnd");

        }


        [HttpGet("history/{memberId}")]
        public async Task<IActionResult> ViewBorrowingHistoryByMemberId(Guid memberId)
        {
            var result = await _borrowingManagement.ViewBorrowingHistoryForAMember(memberId);
            if (result is null || result.Count == 0)
            {
                return NotFound();
            }

            BorrowingResponse<List<BorrowBook>> response = new(result.ToList(), "Success");
            return Ok(response);
        }


        [HttpGet("availability/{bookId}")]
        public async Task<IActionResult> CheckBookavailabilityAsync(Guid bookId)
        {
            var result = await _borrowingManagement.CheckBookAvailability(bookId);
            if(result is false)
            {
                return NotFound("This book is not available");

            }

            return Ok("This book is available");

        }
    
    }
}
