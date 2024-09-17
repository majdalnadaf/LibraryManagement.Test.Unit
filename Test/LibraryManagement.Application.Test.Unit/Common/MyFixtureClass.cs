using FluentValidation;
using LibraryManagement.Application.InfrastructureServices;
using LibraryManagement.Application.Services;
using LibraryManagement.Application.ValidationRules;
using LibraryManagement.Domain.Domains;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit.Sdk;

namespace LibraryManagement.Application.Test.Unit.Common
{

    public class MyFixtureClass
    {
        //Share the member repository test class  between the member management and borrowing management test class..... 
        public readonly IMemberRepository _memberRepository = Substitute.For<IMemberRepository>();
        public MyFixtureClass()
        {
            
        }
    }
}