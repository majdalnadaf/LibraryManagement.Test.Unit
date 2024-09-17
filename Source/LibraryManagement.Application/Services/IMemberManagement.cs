using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LibraryManagement.Domain.Domains;

namespace LibraryManagement.Application.Services
{
    public interface IMemberManagement
    {
        Task<(bool, Guid)> CreateMemberAsync(Member member);
        Task<bool> UpdateMemberAsync( Member member);
        Task<bool> DeleteMemberAsync(Guid memberId);

        Task<List<Member>> SearchForMemberByName(string memberName);
        Task<List<Member>> SearchForMemberById(Guid memberId);
    }
}
