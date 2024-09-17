using LibraryManagement.Api.Contract.Members;
using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Domains;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberManagement _memberManagement;


        public MemberController(IMemberManagement memberManagement)
        {

        }



        [HttpGet("{memberName}")]
        public async Task<IActionResult> GetAllMemberByNameAsync(string memberName)
        {
            var result = await _memberManagement.SearchForMemberByName(memberName);
            if (result is null || result.Count == 0)
            {
                return NotFound();
            }


            return Ok(result);

        }


        [HttpPost]
        public async Task<IActionResult> CreateMemberAsync([FromBody] CreateMemberRequest request)
        {

            if (string.IsNullOrEmpty(request.fullName) || string.IsNullOrEmpty(request.email))
            {
                return BadRequest("Invalid data");
            }

            var member = new Member()
            {
                Id = Guid.NewGuid(),
                FullName = request.fullName,
                Email = request.email,
            };

            var result = await _memberManagement.CreateMemberAsync(member);
            if (result.Item1 is false)
            {
                return Problem("Somthing wrong");
            }

            MemberResponse<Guid> response = new(result.Item2, "Member was created successfully");

            return CreatedAtAction(nameof(CreateMemberAsync), response);


        }


        [HttpPost]
        public async Task<IActionResult> UpdateMemberAsync([FromBody] UpdateMemberRequest request)
        {
            if (string.IsNullOrEmpty(request.fullName) || string.IsNullOrEmpty(request.email))
            {
                return BadRequest("Invalid data");
            }

            var member = new Member()
            {
                Id = request.id,
                FullName = request.fullName,
                Email = request.email
            };

            var result = await _memberManagement.UpdateMemberAsync(member);
            if (result is false)
            {
                return Problem("Somthing wrong");
            }

            MemberResponse<Member> response = new(data: member, Message: "Member was successfully");

            return Ok(response);

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMemberAsync(Guid id)
        {
            var result = await _memberManagement.DeleteMemberAsync(id);
            if(result is false)
            {
                return Problem();
            }

            return Ok("Member was deleted");
        
        
        }


    }
}
