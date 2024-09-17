#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using LibraryManagement.Application.InfrastructureServices;
using LibraryManagement.Application.Services;
using LibraryManagement.Application.Test.Unit.Common;
using LibraryManagement.Application.ValidationRules;
using LibraryManagement.Domain.Domains;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
#endregion

namespace LibraryManagement.Application.Test.Unit.BorrowingManagementTests
{
    [Collection("My Collection Fixture Class")]

    public class BorrowingManagementTests : IAsyncLifetime
    {
        #region Ctor
        private readonly BorrowingManagement _sut;
        private readonly ILoggerAdapter<BorrowingManagement> _logger = Substitute.For<ILoggerAdapter<BorrowingManagement>>();
        private readonly IBorrowingRepository _borrowingRepository = Substitute.For<IBorrowingRepository>();
        private readonly IBookRepositroy _bookRepository = Substitute.For<IBookRepositroy>();
        private readonly IBorrowingManagement _borrowingManagement = Substitute.For<IBorrowingManagement>();

        private readonly MyFixtureClass _fixture;

        public BorrowingManagementTests(MyFixtureClass fixture)
        {
            _fixture = fixture;
            _sut = new(_borrowingRepository, _logger, _bookRepository, _fixture._memberRepository);
        }

        #endregion

        #region BorrowBookTests
        [Fact]
        public async Task BorrowBook_ShouldTrueAndDate_WhenBookIdAndMemberIdAreValid()
        {

            var borrowBook = new BorrowBook()
            {
                Id = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid(),
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(15),
            };

            //Arrange
            _borrowingRepository.CreateAsync(Arg.Any<BorrowBook>()).Returns((true, borrowBook));
            _bookRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(new Book { Id = borrowBook.BookId });
            _fixture._memberRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(new Member { Id = borrowBook.MemberId });

            //Act

            var result = await _sut.BorrowBook(borrowBook.MemberId, borrowBook.BookId);

            //Assert

            result.Item1.Should().BeTrue();
            result.Item2.Should().BeSameDateAs(borrowBook.DueDate);
        }

        [Fact]
        public async Task BorrowBook_ShouldFalse_WhenBookIdIsNotValid()
        {

            var borrowBook = new BorrowBook()
            {
                Id = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid(),
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(15),
            };

            //Arrange
            _bookRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

            //Act

            var result = await _sut.BorrowBook(borrowBook.MemberId, borrowBook.BookId);

            //Assert
            result.Item1.Should().BeFalse();
            result.Item2.Should().BeSameDateAs(DateTime.MinValue);
            result.Item3.Should().Be("Book is not available");

        }


        [Fact]
        public async Task BorrowBook_ShouldFalse_WhenMemberIdIsNotValid()
        {
            var borrowBook = new BorrowBook()
            {
                Id = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid(),
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(15),
            };

            //Arrange
            _borrowingRepository.CreateAsync(Arg.Any<BorrowBook>()).Returns((true, borrowBook));
            _bookRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(new Book() { IsAvailable = true });
            _fixture._memberRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();
            //Act

            var result = await _sut.BorrowBook(borrowBook.MemberId, borrowBook.BookId);

            //Assert

            result.Item1.Should().BeFalse();
            result.Item2.Should().BeSameDateAs(DateTime.MinValue);
        }


        [Fact]
        public async Task BorrowBook_ShouldFalse_WhenBookIsAlreadyBorrowed()
        {
            var borrowBook = new BorrowBook()
            {
                Id = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid(),
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(15),
                Book = new() { IsAvailable = false, Title = "C#" }
            };

            var book = new Book()
            {
                IsAvailable = false
            };

            //Arrange
            _bookRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(book);


            //Act

            var result = await _sut.BorrowBook(borrowBook.MemberId, borrowBook.BookId);

            //Assert

            result.Item1.Should().BeFalse();
            result.Item2.Should().BeSameDateAs(DateTime.MinValue);
        }
        #endregion


        #region ReturnBookTests
        [Fact]
        public async Task ReturnBook_ShouldRuturnsTrue_WhenMemberIdAndBookIdAreValid()
        {

            var borrowBook = new BorrowBook()
            {
                Id = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid(),
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(15),
                Book = new() { IsAvailable = false, Title = "C#" }
            };

            var book = new Book() { IsAvailable = false };
            var member = new Member() { };
            //Arrange
            _bookRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(book);
            _fixture._memberRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(member);

            //Act
            var result = await _sut.ReturnBook(borrowBook.MemberId, borrowBook.BookId, borrowBook.Id);

            //Assert
            result.Should().Be(true);

        }


        [Fact]
        public async Task ReturnBook_ShouldReturnsFalse_WhenTheMemberIdIsNotValid()
        {
            var borrowBook = new BorrowBook()
            {
                Id = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid(),
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(15),
                Book = new() { IsAvailable = false, Title = "C#" }
            };

            var book = new Book() { IsAvailable = false };

            //Arrange
            _fixture._memberRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

            //Act
            var result = await _sut.ReturnBook(borrowBook.MemberId, borrowBook.BookId, borrowBook.Id);

            //Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task ReturnBook_ShouldReturnsFalse_WhenTheBookIdIsNotValid()
        {
            var borrowBook = new BorrowBook()
            {
                Id = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid(),
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(15),
                Book = new() { IsAvailable = false, Title = "C#" }
            };

            var book = new Book() { IsAvailable = false };

            //Arrange
            _bookRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

            //Act
            var result = await _sut.ReturnBook(borrowBook.MemberId, borrowBook.BookId, borrowBook.Id);

            //Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task ReturnBook_ShouldReturnsFalse_WhenTheBookWanNotBorrorwed()
        {
            var borrowBook = new BorrowBook()
            {
                Id = Guid.NewGuid(),
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid(),
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(15),
                Book = new() { IsAvailable = false, Title = "C#" }
            };

            var book = new Book() { IsAvailable = true };

            //Arrange
            _bookRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(book);

            //Act
            var result = await _sut.ReturnBook(borrowBook.MemberId, borrowBook.BookId, borrowBook.Id);

            //Assert
            result.Should().Be(false);
        }
        #endregion



        #region ViewBorrowingHistoryTests
        [Fact]
        public async Task ViewBorrowingHistoryForAMember_ShouldReturnsListOfBrorrowBook_WhenMemberIdIsValid()
        {

            var lstBorrowingBooks = new List<BorrowBook>()
            {
                new BorrowBook(){Id = Guid.NewGuid(), BookId = Guid.NewGuid() , MemberId = Guid.NewGuid()},
                new BorrowBook(){Id = Guid.NewGuid(), BookId = Guid.NewGuid() , MemberId = Guid.NewGuid()},
            };


            //Arrange 
            _borrowingRepository.GetAllBorrowingBookByMemberIdAsync(Arg.Any<Guid>()).Returns(lstBorrowingBooks);

            //Act
            var result = await _sut.ViewBorrowingHistoryForAMember(Guid.NewGuid());

            //Assert

            result.Should().BeEquivalentTo(lstBorrowingBooks);
        }

        [Fact]
        public async Task ViewBorrowingHistoryForAMember_ShouldReturnsNull_WhenMemberIdIsNotValid()
        {
            var lstBorrowingBooks = new List<BorrowBook>()
            {
                new BorrowBook(){Id = Guid.NewGuid(), BookId = Guid.NewGuid() , MemberId = Guid.NewGuid()},
                new BorrowBook(){Id = Guid.NewGuid(), BookId = Guid.NewGuid() , MemberId = Guid.NewGuid()},
            };


            //Arrange 
            _borrowingRepository.GetAllBorrowingBookByMemberIdAsync(Arg.Any<Guid>()).Returns(lstBorrowingBooks);

            //Act
            var result = await _sut.ViewBorrowingHistoryForAMember(Guid.Empty);

            //Assert

            result.Should().BeNull();
        }

        [Fact]
        public async Task ViewBorrowingHistoryForAMember_ShouldReturnsEmptyList_WhenMemberWithNotBrorrowingRecords()
        {
            var lstBorrowingBooks = new List<BorrowBook>();


            //Arrange 
            _borrowingRepository.GetAllBorrowingBookByMemberIdAsync(Arg.Any<Guid>()).Returns(lstBorrowingBooks);

            //Act
            var result = await _sut.ViewBorrowingHistoryForAMember(Guid.NewGuid());

            //Assert

            result.Should().BeEquivalentTo(new List<BorrowBook>());
        }
        #endregion



        #region CheckBookAvailabilityTests

        [Fact]
        public async Task CheckBookAvailability_ShouldReturnsTrue_WhenBookIsAvailableAndValidBookId()
        {
            var bookId = Guid.NewGuid();
            var book = new Book()
            {
                Id = bookId,
                IsAvailable = true
            };

            //Arrange 
            _bookRepository.GetByIdAsync(bookId).Returns(book);

            //Act 
            var result = await _sut.CheckBookAvailability(bookId);

            //Assert

            result.Should().BeTrue();
        }

        [Fact]
        public async Task CheckBookAvailability_ShouldReturnsFalse_WhenBookIdIsNotValid()
        {
            var invlaidBookId = Guid.Empty;
            var book = new Book()
            {
                Id = invlaidBookId,
                IsAvailable = true
            };

            //Arrange 

            //Act 
            var result = await _sut.CheckBookAvailability(invlaidBookId);

            //Assert

            result.Should().BeFalse();
        }


        [Fact]
        public async Task CheckBookAvailability_ShouldReturnsFalse_WhenBookIsNotAvailable()
        {
            var bookId = Guid.NewGuid();
            var book = new Book()
            {
                Id = bookId,
                IsAvailable = false // not available
            };

            //Arrange 
            _bookRepository.GetByIdAsync(bookId).Returns(book);

            //Act 
            var result = await _sut.CheckBookAvailability(bookId);

            //Assert

            result.Should().BeFalse();
        }


        #endregion


        public async Task InitializeAsync()
        {
            // Initialization
            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            // Clean up
            await Task.CompletedTask;
        }
    }
}
