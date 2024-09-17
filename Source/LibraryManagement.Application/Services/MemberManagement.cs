using LibraryManagement.Application.InfrastructureServices;
using LibraryManagement.Domain.Domains;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LibraryManagement.Application.Helper;
using FluentValidation;
using LibraryManagement.Application.ValidationRules;
namespace LibraryManagement.Application.Services
{
    public class MemberManagement : IMemberManagement
    {
        private readonly ILoggerAdapter<MemberManagement> _logger;
        private readonly IMemberRepository _memberRepository;
        private readonly IValidator<Member> _validator;

        private ClsValiationGuid _clsValiationGuid = new();

        public MemberManagement(ILoggerAdapter<MemberManagement> logger, IMemberRepository memberRepository
           , IValidator<Member> validator)
        {
            _logger = logger;
            _memberRepository = memberRepository;
            _validator = validator;

        }


        public async Task<bool> CheckIsExistsMember(Guid memberId)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if(member is null)
                return false;

            return true;
        }

        public async Task<bool> CheckDuplicatedEmail(string email)
        {
            var lstMember = await _memberRepository.GetAllAsync();
            var member = lstMember.Where(x=> x.Email == email).FirstOrDefault();
        
            return member is null
                ? true
                : false;    
        }


        public async Task<(bool, Guid)> CreateMemberAsync(Member member)
        {
            try
            {
                var vlaidationResult =  _validator.Validate(member);
                if (!vlaidationResult.IsValid)
                    return (false, Guid.Empty);


                // If email is already exists 
                if (await CheckDuplicatedEmail(member.Email) == false)
                    return (false, Guid.Empty);


                var result = await _memberRepository.CreateAsync(member);
                if (!result.Item1)
                {
                    return (false, Guid.Empty);
                }


                _logger.LogInformation("Member was created");
                return (true, result.Item2.Id);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Message");
                throw;
            }
        }

        public async Task<bool> DeleteMemberAsync(Guid memberId)
        {
            try
            {
                if (await CheckIsExistsMember(memberId) == false)
                    return false;


                var resutl = await _memberRepository.DeleteAsync(memberId);

                if (!resutl)
                    return false;


                _logger.LogInformation($"{nameof(Member)} was deleted");
                return resutl;

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Message");
                throw;
            }
        }

        public async Task<List<Member>> SearchForMemberById(Guid memberId)
        {
            try
            {

                if (!_clsValiationGuid.CheckGuidStatus(memberId))
                {
                    return new List<Member>();
                }


                Member member = await _memberRepository.GetByIdAsync(memberId);
                if (member is null )
                {
                    return new List<Member>();
                }


                _logger.LogInformation("Members were sended");
                return new List<Member>() { member };

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Message");
                throw;
            }
        }

        public async Task<List<Member>> SearchForMemberByName(string memberName)
        {
            try
            {
                if (string.IsNullOrEmpty(memberName))
                {
                    return new List<Member>();
                }

                ICollection<Member> lstAllMember = await _memberRepository.GetAllAsync();
                if (lstAllMember is null || lstAllMember.Count == 0)
                {
                    return new List<Member>();
                }


                var lstSpecificMembers = lstAllMember.Where(a => a.FullName == memberName).ToList();

                _logger.LogInformation("Members were sended");
                return lstSpecificMembers;

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Message");
                throw;
            }
        }

        public async Task<bool> UpdateMemberAsync(Member member)
        {
            try
            {
               
                var validationResult = _validator.Validate(member);
                if (!validationResult.IsValid)
                {
                    return false;
                }


                var result = await _memberRepository.UpdateAsync(member.Id,member);
                if (!result)
                {
                    return false;
                }


                _logger.LogInformation("Member was updated");
                return true;

            }
            catch (Exception e)
            {

                _logger.LogError(e, "Error Message");

                throw;
            }
        }
    }
}
