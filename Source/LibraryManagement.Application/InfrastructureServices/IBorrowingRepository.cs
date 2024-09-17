using LibraryManagement.Domain.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.InfrastructureServices
{
    public interface IBorrowingRepository
    {
        Task<ICollection<BorrowBook>> GetAllBorrowingBookByMemberIdAsync(Guid memberId);
        Task<BorrowBook> GetByIdAsync(Guid id);
        Task <(bool,BorrowBook)> CreateAsync(BorrowBook borrowBook);
        Task<(bool, BorrowBook)> UpdateAsync(BorrowBook borrowBook);
        Task<bool> DeleteAsync(Guid id);

    }
}
