using FluentValidation;
using LibraryManagement.Application.Helper;
using LibraryManagement.Application.InfrastructureServices;
using LibraryManagement.Domain.Domains;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LibraryManagement.Application.Services
{
    public class BorrowingManagement : IBorrowingManagement
    {


        private readonly IBorrowingRepository _borrowingRepository;
        private readonly IBookRepositroy _bookRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly ILoggerAdapter<BorrowingManagement> _logger;
        private readonly ClsValiationGuid _clsValiationGuid = new();

        public BorrowingManagement(IBorrowingRepository borrowingRepository, ILoggerAdapter<BorrowingManagement> logger
            , IBookRepositroy bookRepository, IMemberRepository memberRepository)
        {
            _borrowingRepository = borrowingRepository;
            _logger = logger;
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
        }




        public async Task<(bool, DateTime, string)> BorrowBook(Guid memberId, Guid bookId)
        {
            try
            {

                if (!_clsValiationGuid.CheckGuidStatus(memberId))
                {
                    return (false, DateTime.MinValue, "Guid is not valid");
                }

                if (!await CheckBookAvailability(bookId))
                {
                    return (false, DateTime.MinValue, "Book is not available");
                }


                if (!await CheckMemberIsValid(memberId))
                {
                    return (false, DateTime.MinValue, "Member is not valid");
                }



                var borrowBook = new BorrowBook()
                {
                    Id = Guid.NewGuid(),
                    BookId = bookId,
                    MemberId = memberId,
                    BorrowDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(15)
                };



                var result = await _borrowingRepository.CreateAsync(borrowBook);
                if (!result.Item1)
                {
                    return (false, DateTime.MinValue, "Try again");
                }


                _logger.LogInformation("Book was borrowed");
                return (true, result.Item2.DueDate, "Success");



            }
            catch (Exception e)
            {

                _logger.LogError(e, "Error Message");
                throw;
            }
        }

        public async Task<bool> CheckBookAvailability(Guid bookId)
        {
            try
            {
                if (!_clsValiationGuid.CheckGuidStatus(bookId))
                {
                    return false;
                }

                var book = await _bookRepository.GetByIdAsync(bookId);

                if (book is null || !book.IsAvailable)
                    return false;

                return true;

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Message");
                throw;
            }
        }


        public async Task<bool> CheckMemberIsValid(Guid memberId)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member is null)
                return false;

            return true;

        }

        public async Task<bool> ReturnBook(Guid memberId, Guid bookId, Guid borrowBookId)
        {
            try
            {

                if (!_clsValiationGuid.CheckGuidStatus(memberId))
                {
                    return false;
                }

                if (!await CheckMemberIsValid(memberId))
                {
                    return false;
                }

                if (await CheckBookAvailability(bookId))
                {
                    return false; 
                }


                var book = await _bookRepository.GetByIdAsync(bookId);
                if (book is null || book.IsAvailable)
                {
                    return false;
                }


                book.IsAvailable = true;


                //Optional , you can store them in any place do you want as history of borrowing or delete them like i did
                await _bookRepository.DeleteAsync(borrowBookId);



                _logger.LogInformation("Book was returned");
                return book.IsAvailable;



            }
            catch (Exception e)
            {

                _logger.LogError(e, "Error Message");
                throw;
            }
        }

        public async Task<ICollection<BorrowBook>> ViewBorrowingHistoryForAMember(Guid memberId)
        {
            try
            {


                if (!_clsValiationGuid.CheckGuidStatus(memberId))
                {
                    return null;
                }


                var lstBorrowBook = await _borrowingRepository.GetAllBorrowingBookByMemberIdAsync(memberId);
                if (lstBorrowBook is null)
                {
                    return new List<BorrowBook>();
                }



                _logger.LogInformation("Book was returned");
                return lstBorrowBook;



            }
            catch (Exception e)
            {

                _logger.LogError(e, "Error Message");
                throw;
            }
        }


    }
}
