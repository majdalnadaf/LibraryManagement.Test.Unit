
using AutoMapper;
using LibraryManagement.Api.Contract.Books;
using LibraryManagement.Api.Contract.Members;
using LibraryManagement.Domain.Domains;
namespace LibraryManagement.Api.AutoMapper
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateBookRequest, Book>();
            CreateMap<UpdateBookRequest, Book>();

            CreateMap<CreateMemberRequest, Member>();
        }
    }
}
