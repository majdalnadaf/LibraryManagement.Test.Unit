namespace LibraryManagement.Api.Contract.Borrowing
{
    public record ReturnBookRequest(Guid memberId, Guid bookId ,Guid borrowingId);

}
