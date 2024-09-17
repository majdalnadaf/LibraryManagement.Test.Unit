using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagement.Domain.Domains;

namespace LibraryManagement.Application.Services
{
    public interface IBookManagement
    {
        Task<(bool, Guid)> CreateBookAsync(Book book);
        Task<bool> UpdateBookAsync( Book book);
        Task<bool> DeleteBookAsync(Guid bookId);

        Task<List<Book>> SearchForBookByBookNameAsync(string title);
        Task<List<Book>> SearchForBookByAuthorNameAsync(string author);
        Task<List<Book>> SearchForBooByISBNCodekAsync(string isbn);
    }
}
