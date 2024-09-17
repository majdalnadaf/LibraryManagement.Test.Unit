using LibraryManagement.Domain.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Services
{
    public interface IBorrowingManagement
    {
        Task<(bool, DateTime, string)> BorrowBook(Guid memberId, Guid bookId);
        Task<bool> ReturnBook(Guid memberId , Guid bookId,Guid borrowBookId);
        Task<ICollection<BorrowBook>> ViewBorrowingHistoryForAMember(Guid memberId);

        Task<bool> CheckBookAvailability(Guid bookId);
        Task<bool> CheckMemberIsValid(Guid memberId);

    }
}
