using LibraryManagement.Application.InfrastructureServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagement.Domain.Domains;

namespace LibraryManagement.Infrastructure
{
    internal class MemberRepository : IMemberRepository
    {

        private readonly List<Member> _members = new();

        public async Task<(bool, Member)> CreateAsync(Domain.Domains.Member member)
        {
            if (member is null)
                return  (false, null);
            
            _members.Add(member);
            return (true, member);            
        }

        public Task<bool> DeleteAsync(Guid memberId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Domain.Domains.Member>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Domain.Domains.Member> GetByIdAsync(Guid memberId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Guid memberId, Domain.Domains.Member member)
        {
            throw new NotImplementedException();
        }
    }
}
