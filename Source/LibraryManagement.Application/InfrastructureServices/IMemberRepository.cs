using LibraryManagement.Domain.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.InfrastructureServices
{
    public interface IMemberRepository
    {


        Task<ICollection<Member>> GetAllAsync();
        Task<(bool, Member)> CreateAsync(Member member);
        Task<bool> UpdateAsync(Guid memberId, Member member);
        Task<bool> DeleteAsync(Guid memberId);
        Task<Member> GetByIdAsync(Guid memberId);
    
    }
}
