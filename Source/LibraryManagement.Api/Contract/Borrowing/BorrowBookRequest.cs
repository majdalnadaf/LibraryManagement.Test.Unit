namespace LibraryManagement.Api.Contract.Borrowing
{
    public record BorrowBookRequest(Guid memberId , Guid bookId);

}
