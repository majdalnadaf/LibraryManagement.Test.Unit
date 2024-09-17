namespace LibraryManagement.Api.Contract.Members
{
    public record MemberResponse<T>(T data, string Message);
    
}
