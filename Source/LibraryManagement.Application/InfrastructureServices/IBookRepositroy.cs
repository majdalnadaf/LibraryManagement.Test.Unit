using LibraryManagement.Domain.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.InfrastructureServices
{


    public interface IBookRepositroy
    {

        Task<ICollection<Book>> GetAllAsync();
        Task<Book> GetByIdAsync(Guid id);
        Task<(bool, Book)> CreateAsync(Book book);
        Task<(bool,Book)> UpdateAsync(Book book);
        Task<bool> DeleteAsync(Guid bookId);

    }
}
