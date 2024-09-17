namespace LibraryManagement.Api.Contract.Books
{
    public record UpdateBookRequest(Guid id , string author , string title , string description , bool isAvailable , string ISBN);
    
}
