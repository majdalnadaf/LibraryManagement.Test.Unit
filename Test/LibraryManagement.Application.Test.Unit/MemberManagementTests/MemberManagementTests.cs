using LibraryManagement.Application.InfrastructureServices;
using LibraryManagement.Application.Services;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
#region using
using System.Text;
using System.Threading.Tasks;

using LibraryManagement.Domain.Domains;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using LibraryManagement.Application.ValidationRules;
using FluentAssertions;
using NSubstitute.ReturnsExtensions;
using Xunit.Abstractions;
using Xunit;
using NSubstitute.ExceptionExtensions;
using LibraryManagement.Application.Test.Unit.Common;
#endregion

namespace LibraryManagement.Application.Test.Unit.MemberManagementTests
{
    [Collection("My Collection Fixture Class")]
    public class MemberManagementTests : IAsyncLifetime
    {
        #region Ctor

        private readonly MemberManagement _sut;
        private readonly IValidator<Member> _validator;
        private ILoggerAdapter<MemberManagement> _logger = Substitute.For<ILoggerAdapter<MemberManagement>>();
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IMemberRepository _memberRepository = Substitute.For<IMemberRepository>();

        private readonly MyFixtureClass _fixture;
        public MemberManagementTests(ITestOutputHelper testOutputHelper, MyFixtureClass fixture)
        {
            _testOutputHelper = testOutputHelper;
            _fixture = fixture;


            // Register IValidator interface and get it 
            var services = new ServiceCollection();
            services.AddScoped<IValidator<Member>, MemberValidtor>();
            var serviceProvider = services.BuildServiceProvider();
            _validator = serviceProvider.GetRequiredService<IValidator<Member>>();

            _sut = new(_logger, _fixture._memberRepository, _validator);
        }
        #endregion

        #region LoggerAndThorwExceptionTests
        [Fact]
        public async Task CreateMemberAsync_ShouldLogMessage_WhenMemberWasCreated()
        {
            var member = new Member()
            {
                Id = Guid.NewGuid(),
                FullName = "Majd Al Nadaf",
                Email = "majdalnadaf8@gmail.com",
            };

            //Arrange 
            _fixture._memberRepository.CreateAsync(member).Returns((true, member));
            _fixture._memberRepository.GetAllAsync().Returns(new List<Member>());  // for duplication email test method


            //Act 
            await _sut.CreateMemberAsync(member);


            //Assert
            _logger.Received(1).LogInformation("Member was created");

        }


        [Fact]
        public async Task CreateMemberAsync_ShouldLogErrorMessage_WhenExceptionIsThrown()
        {
            var member = new Member()
            {
                Id = Guid.NewGuid(),
                FullName = "Majd Al Nadaf",
                Email = "majdalnadaf8@gmail.com",
            };

            var exception = new ArgumentException();

            //Arrange 
            _fixture._memberRepository.CreateAsync(member).Throws(exception);
            _fixture._memberRepository.GetAllAsync().Returns(new List<Member>());  // for duplication email test method


            //Act 
            var result = async () => await _sut.CreateMemberAsync(member);


            //Assert
            await result.Should().ThrowAsync<ArgumentException>();
            _logger.Received(1).LogError(exception, "Error Message");

        }


        [Fact]
        public async Task UpdateMemberAsync_ShouldLogMessage_WhenMemberWasUpdated()
        {
            var member = new Member()
            {
                Id = Guid.NewGuid(),
                FullName = "Majd Al Nadaf",
                Email = "majdalnadaf8@gmail.com",
            };

            //Arrange 
            _fixture._memberRepository.UpdateAsync(Arg.Any<Guid>(), member).Returns(true);


            //Act 
            await _sut.UpdateMemberAsync(member);


            //Assert
            _logger.Received(1).LogInformation("Member was updated");

        }

        [Fact]
        public async Task UpdateMemberAsync_ShouldLogErrorMessage_WhenExceptionIsThrown()
        {
            var member = new Member()
            {
                Id = Guid.NewGuid(),
                FullName = "Majd Al Nadaf",
                Email = "majdalnadaf8@gmail.com",
            };

            var exception = new NullReferenceException();

            //Arrange 
            _fixture._memberRepository.UpdateAsync(Arg.Any<Guid>(), member).Throws(exception);


            //Act 
            var result = async () => await _sut.UpdateMemberAsync(member);


            //Assert
            await result.Should().ThrowAsync<NullReferenceException>();
            _logger.Received(1).LogError(exception, "Error Message");

        }



        #endregion



        #region AddMemberAsyncTests
       
        //[Fact]
        //public async Task AddMemberAsync_ShouldRetunsTrueAndGuidOfMember_WhenTheMemberDataWereValid()
        //{
        //    var member = new Member()
        //    {
        //        Id = Guid.NewGuid(),
        //        FullName = "Majd Al Nadaf",
        //        Email = "invalid email addrees"
        //    };

        //    //Arrange

        //     _fixture._memberRepository.CreateAsync(Arg.Do<Member>(x=> member = x)).Returns((true, member));

        //    //Act
        //    var result = await _sut.CreateMemberAsync(member);


        //    //Assert
        //    result.Item1.Should().BeTrue();
        //    result.Item2.Should().Be(member.Id);
        //}

        [Fact]
        public async Task AddMemberAsync_ShouldReturnsFalseWithEmptyGuid_WhenTheMemberWasWithMissingField()
        {
            var member = new Member()
            {
                Id = Guid.NewGuid(),
                FullName = "Majd Al Nadaf",
                Email = "invalid email addrees"
            };

            //Arrange

            _fixture._memberRepository.CreateAsync(Arg.Do<Member>(x => member = x)).Returns((false, null));


            //Act
            var result = await _sut.CreateMemberAsync(member);


            //Assert
            result.Item1.Should().BeFalse();
            result.Item2.Should().Be(Guid.Empty);
        }

        [Fact]
        public async Task AddMemberAsync_ShouldReturnsFasleAndEmptyGuid_WhenTheMemberWasWithDuplicateEmail()
        {
            var member = new Member()
            {
                Id = Guid.NewGuid(),
                FullName = "Majd Al Nadaf",
                Email = "majdalnadaf8@gmial.com"
            };

            var lstMember = new List<Member>();
            lstMember.Add(member);

            //Arrange

            _fixture._memberRepository.CreateAsync(Arg.Is<Member>(x => x.Email == member.Email)).Returns((true, member));
            _fixture._memberRepository.GetAllAsync().Returns(lstMember);

            //Act
            var result = await _sut.CreateMemberAsync(member);


            //Assert
            result.Item1.Should().BeFalse();
            result.Item2.Should().Be(Guid.Empty);
        }
        #endregion

        #region UpdateMemberAsyncTests
        [Fact]
        public async Task UpdateMemberAsync_ShouldRetunsTrue_WhenTheMemberWasWithValidData()
        {
            var member = new Member()
            {
                Id = Guid.NewGuid(),
                FullName = "Majd Al Nadaf",
                Email = "majdalnadaf8@gmial.com"
            };



            //Arrange

            _fixture._memberRepository.UpdateAsync(Arg.Any<Guid>(), Arg.Any<Member>()).Returns(true);


            //Act
            var result = await _sut.UpdateMemberAsync(member);


            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateMemberAsync_ShouldRetunsFalse_WhenTheMemberWasWithInvalidMemberId()
        {
            var member = new Member()
            {
                Id = Guid.Empty,
                FullName = "Majd Al Nadaf",
                Email = "majdalnadaf8@gmial.com"
            };


            //Arrange

            // Just make the id of member equals Guid.Empty


            //Act
            var result = await _sut.UpdateMemberAsync(member);


            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateMemberAsync_ShouldRetunsFalse_WhenTheMemberWasWithMissingRequiedField()
        {
            var member = new Member()
            {
                Id = Guid.Empty,
                FullName = "Majd Al Nadaf",
            };


            //Arrange

            // Just make the email is empty 


            //Act
            var result = await _sut.UpdateMemberAsync(member);


            //Assert
            result.Should().BeFalse();
        }
        #endregion

        #region DeleteMemberAsyncTests
        [Fact]
        public async Task DeleteMemberAsync_ShouldRetuensTrue_WhenTheMemberIdIsVaild()
        {

            var member = new Member()
            {
                Id = Guid.NewGuid(),
                FullName = "Majd Al Nadaf",
                Email = "majdalnadaf8@gmail.com"
            };


            //Arrange
            _fixture._memberRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(member);
            _fixture._memberRepository.UpdateAsync(Arg.Any<Guid>(), member).Returns(true);


            //Act
            var result = await _sut.UpdateMemberAsync(member);


            //Assert
            result.Should().BeTrue();
        }
        [Fact]
        public async Task DeleteMemberAsync_ShouldRetuensFalse_WhenTheMemberIdIsInvaild()
        {
            var member = new Member()
            {
                Id = Guid.Empty,
                FullName = "Majd Al Nadaf",
                Email = "majdalnadaf8@gmail.com"
            };


            //Arrange
            _fixture._memberRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(member);
            _fixture._memberRepository.UpdateAsync(Arg.Any<Guid>(), member).Returns(true);


            //Act
            var result = await _sut.UpdateMemberAsync(member);


            //Assert
            result.Should().BeFalse();
        }
        #endregion

        #region SearchTests

        [Fact]
        public async Task SearchForMemberById_ShouldReturnsMember_WhenMemberIdIsValid()
        {
            var member = new Member()
            {
                Id = Guid.NewGuid(),
                FullName = "Majd Al Nadaf",
                Email = "majdalnadaf8@gmail.com"
            };



            //Arrange
            _fixture._memberRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(member);


            //Act
            var result = await _sut.SearchForMemberById(member.Id);


            //Assert
            result.Should().Contain(member);
        }

        [Fact]
        public async Task SearchForMemberById_ShouldReturnsEmptyList_WhenMemberIdIsInvalid()
        {
            var member = new Member()
            {
                Id = Guid.NewGuid(),
                FullName = "Majd Al Nadaf",
                Email = "majdalnadaf8@gmail.com"
            };



            //Arrange
            _fixture._memberRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();


            //Act
            var result = await _sut.SearchForMemberById(member.Id);


            //Assert
            result.Should().BeEmpty();
        }


        #endregion


        public async Task InitializeAsync()
        {
            // Configure the Initialization

            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            // Clean up
            await Task.CompletedTask;
        }
    }
}
