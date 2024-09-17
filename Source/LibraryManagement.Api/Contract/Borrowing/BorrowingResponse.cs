namespace LibraryManagement.Api.Contract.Borrowing
{
    public record BorrowingResponse<T>(T data , string message);

}
