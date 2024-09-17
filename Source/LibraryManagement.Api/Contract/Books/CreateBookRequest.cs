namespace LibraryManagement.Api.Contract.Books
{
    public record CreateBookRequest(string title , string description, string author , string ISBN );
    
}
