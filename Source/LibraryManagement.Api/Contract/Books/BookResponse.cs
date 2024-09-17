
using LibraryManagement.Domain.Domains;


namespace LibraryManagement.Api.Contract.Books
{
    public record BookResponse<T>(T data, string message );
}
