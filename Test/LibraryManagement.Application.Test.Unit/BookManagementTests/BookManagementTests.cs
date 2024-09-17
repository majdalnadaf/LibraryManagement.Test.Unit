#region using
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using LibraryManagement.Application.InfrastructureServices;
using LibraryManagement.Application.Services;
using LibraryManagement.Application.Test.Unit.BookManagementTests;
using LibraryManagement.Application.ValidationRules;
using LibraryManagement.Domain.Domains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit.Abstractions;
using NSubstitute.ExceptionExtensions;
#endregion

namespace LibraryManagement.Application.Test.Unit.BookManagementTests
{
    public class BookManagementTests : IClassFixture<BookManagementClassFixture>
    {
        #region Ctor

        private BookManagementClassFixture _fixture;
        private readonly ITestOutputHelper _testOutputHelper;
        public BookManagementTests(BookManagementClassFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture;
            _testOutputHelper = testOutputHelper;

        }

        #endregion


        #region LoggerAndThrowExceptionTests

        [Fact]
        public async Task DeleteBookAsync_ShouldLogErrorMessage_WhenExceptionIsThrown()
        {
            // Book with valid data
            var book = new Book()
            {
                Title = "The First Step C#",
                Author = "Majd Al Nadaf",
                Description = "Descripe C# Basic",
                Id = Guid.NewGuid(),

            };

            var exception = new StackOverflowException();

            //Arrange 
            _fixture._bookRepository.DeleteAsync(book.Id).Throws(exception);


            //Act
            var result = async () => await _fixture._sut.DeleteBookAsync(book.Id);


            //Assertion

            await result.Should().ThrowAsync<StackOverflowException>();
            _fixture._logger.Received(1).LogError(exception, "Error Message");

        }


        [Fact]
        public async Task SearchForBookByBookNameAsync_ShouldLogErrorMessage_WhenExceptionIsThrown()
        {


            var title = "The First Step C#";

            var book = new Book()
            {
                Title = title,
                Author = "Majd Al Nadaf",
                Description = "Descripe C# Basic",
                Id = Guid.NewGuid(),

            };

            var lstBooks = new List<Book>() { book };

            var exception = new NullReferenceException();

            //Arrange 
            _fixture._bookRepository.GetAllAsync().Throws(exception);


            //Act
            var result = async () => await _fixture._sut.SearchForBookByBookNameAsync(title);


            //Assert

            await result.Should().ThrowAsync<NullReferenceException>();
            _fixture._logger.Received(1).LogError(exception, "Error Message");


        }




        #endregion

        #region CreateBookAsyncTests
        [Fact]
        public async Task CreateBookAsync_ShouldReturnsTrueAndGuidIdOfBook_WhenBookWithValidData()
        {
            var book = new Book()
            {
                Title = "The First Step C#",
                Author = "Majd Al Nadaf",
                Description = "Descripe C# Basic",
                Id = Guid.NewGuid(),

            };

            //Arrange 
            _fixture = new BookManagementClassFixture();  // I used here a fresh instacne because there is a problem when i share the instance with this method test..
            _fixture._bookRepository.CreateAsync(Arg.Do<Book>(x => book = x)).Returns((true, book));

            //Act
            var result = await _fixture._sut.CreateBookAsync(book);


            //Assertion

            result.Item1.Should().BeTrue();
            result.Item2.Should().Be(book.Id);


        }

        [Fact]
        public async Task CreateBookAsync_ShouldReturnsFalse_WhenBookWithMissingRequiedField()
        {
            // Book without required field
            var book = new Book()
            {

                Author = "Majd Al Nadaf",
                Description = "Descripe C# Basic",
                Id = Guid.NewGuid(),

            };

            //Arrange 


            //Act
            var result = await _fixture._sut.CreateBookAsync(book);


            //Assertion

            result.Item1.Should().BeFalse();
            result.Item2.Should().Be(Guid.Empty);

        }

        [Fact]
        public async Task CreateBookAsync_ShouldReturnsFalse_WhenBookWithDuplicateISBN()
        {
            var book = new Book()
            {
                Title = "The First Step C#",
                Author = "Majd Al Nadaf",
                Description = "Descripe C# Basic",
                Id = Guid.NewGuid(),

            };

            var lstBook = new List<Book>() { new Book() { ISBN = book.ISBN } };

            //Arrange 
            _fixture._bookRepository.CreateAsync(book).Returns((false, book));
            _fixture._bookRepository.GetAllAsync().Returns(lstBook);


            //Act
            var result = await _fixture._sut.CreateBookAsync(book);


            //Assertion

            result.Item1.Should().BeFalse();
            result.Item2.Should().Be(Guid.Empty);

        }
        #endregion

        #region UpdateBookAsyncTests

        [Fact]
        public async Task UpdateBookAsync_ShouldReturnsTrue_WhenBookHasValidData()
        {
            // Book without required field
            var book = new Book()
            {
                Title = "The First Step C#",
                Author = "Majd Al Nadaf",
                Description = "Descripe C# Basic",
                Id = Guid.NewGuid(),

            };

            //Arrange 
            _fixture._bookRepository.UpdateAsync(Arg.Any<Book>()).Returns((true, book));


            //Act
            var result = await _fixture._sut.UpdateBookAsync(book);


            //Assertion

            result.Should().BeTrue();

        }

        [Fact]
        public async Task UpdateBookAsync_ShouldReturnsFalse_WhenBookHasInvalidBookId()
        {

            // Book with invalid book id 
            var book = new Book()
            {
                Title = "The First Step C#",
                Author = "Majd Al Nadaf",
                Description = "Descripe C# Basic",
                Id = Guid.Empty,                        // invlaid book id 

            };

            //Arrange 
            _fixture._bookRepository.UpdateAsync(Arg.Any<Book>()).Returns((false, book));


            //Act
            var result = await _fixture._sut.UpdateBookAsync(book);


            //Assertion

            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateBookAsync_ShouldReturnsFalse_WhenBookHasMissingRequiedField()
        {
            // Book with missing required field
            var book = new Book()
            {

                Author = "Majd Al Nadaf",
                Description = "Descripe C# Basic",
                Id = Guid.NewGuid(),

            };

            //Arrange 
            _fixture._bookRepository.UpdateAsync(Arg.Any<Book>()).Returns((false, book));


            //Act
            var result = await _fixture._sut.UpdateBookAsync(book);

            //Assertion

            result.Should().BeFalse();

        }
        #endregion


        #region DeleteBookAsyncTests
        [Fact(Skip = "This test was ignored")]  // You can ignore any test you want 
        public async Task DeleteBookAsync_ShouldReturnsTrue_WhenBookHasValidId()
        {
            // Book with valid data
            var book = new Book()
            {
                Title = "The First Step C#",
                Author = "Majd Al Nadaf",
                Description = "Descripe C# Basic",
                Id = Guid.NewGuid(),

            };


            //Arrange 
            _fixture._bookRepository.DeleteAsync(book.Id).Returns(true);


            //Act
            var result = await _fixture._sut.DeleteBookAsync(book.Id);


            //Assertion

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteBookAsync_ShouldReturnsFalse_WhenBookHasInvalidId()
        {
            // Book with invalid id
            var book = new Book()
            {
                Title = "The First Step C#",
                Author = "Majd Al Nadaf",
                Description = "Descripe C# Basic",
                Id = Guid.Empty, // invalid id

            };

            //Arrange 
            _fixture._bookRepository.DeleteAsync(book.Id).Returns(false);


            //Act
            var result = await _fixture._sut.DeleteBookAsync(book.Id);


            //Assertion

            result.Should().BeFalse();
        }

        #endregion


        #region SearchTests

        [Fact]
        public async Task SearchForBookByBookNameAsync_ShouldReturnsBook_WhenTheNameIsValid()
        {
            // Book without required field

            var title = "The First Step C#";

            var book = new Book()
            {
                Title = title,
                Author = "Majd Al Nadaf",
                Description = "Descripe C# Basic",
                Id = Guid.NewGuid(),

            };

            var lstBooks = new List<Book>() { book };


            //Arrange 
            _fixture._bookRepository.GetAllAsync().Returns(lstBooks);


            //Act
            var result = await _fixture._sut.SearchForBookByBookNameAsync(title);


            //Assertion

            result.Should().Contain(x => x.Title == title);
        }

        [Fact]
        public async Task SearchForBookByBookNameAsync_ShouldReturnsEmptyList_WhenTheNameIsInvalid()
        {
            var inValidTitle = "The First Step C#";
            var book = new Book()
            {
                Title = inValidTitle,
                Author = "Majd Al Nadaf",
                Description = "Descripe C# Basic",
                Id = Guid.NewGuid(),

            };



            //Arrange 
            _fixture._bookRepository.GetAllAsync().Returns(new List<Book>());


            //Act
            var result = await _fixture._sut.SearchForBookByBookNameAsync(inValidTitle);


            //Assertion

            result.Should().BeEmpty();
        }


        [Fact]
        public async Task SearchForBookByAuthorNameAsync_ShouldReturnsBook_WhenTheAuthorNameIsValid()
        {
            // Book without required field

            var author = "Majd Al Nadaf";
            var book = new Book()
            {
                Title = "The First Step C#",
                Author = author,
                Description = "Descripe C# Basic",
                Id = Guid.NewGuid(),

            };

            var lstBooks = new List<Book>() { book };


            //Arrange 

            _fixture = new BookManagementClassFixture();  // I used here a fresh instacne because there is a problem when i share the instance with this method test..
            _fixture._bookRepository.GetAllAsync().Returns(lstBooks);


            //Act
            var result = await _fixture._sut.SearchForBookByAuthorNameAsync(author);


            //Assertion

            result.Should().Contain(x => x.Author == book.Author);
        }



        [Fact]
        public async Task SearchForBooByISBNCodekAsync_ShouldReturnsBook_WhenTheISBNCodeIsValid()
        {

            var ISBN = Guid.NewGuid().ToString();
            var book = new Book()
            {
                Title = "The First Step C#",
                Author = "Majd Al Nadaf",
                Description = "Descripe C# Basic",
                Id = Guid.NewGuid(),
                IsAvailable = true,
                ISBN = ISBN,


            };

            var lstBooks = new List<Book>() { book };

            //Arrange 
            _fixture._bookRepository.GetAllAsync().Returns(lstBooks);


            //Act
            var result = await _fixture._sut.SearchForBooByISBNCodekAsync(book.ISBN);


            //Assertion

            result.Should().Contain(x => x.ISBN == book.ISBN);

        }


        [Fact]
        public async Task SearchForBookByAuthorNameAsync_ShouldReturnsEmptyList_WhenTheAuthorNameIsInvalid()
        {
            // Book without required field
            var book = new Book()
            {
                Title = "The First Step C#",
                Author = string.Empty, // invalid name 
                Description = "Descripe C# Basic",
                Id = Guid.NewGuid(),

            };



            //Arrange 


            //Act
            var result = await _fixture._sut.SearchForBookByAuthorNameAsync(book.Author);


            //Assertion

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task SearchForBooByISBNCodekAsync_ShouldReturnsEmptyList_WhenTheISBNCodeIsInvalid()
        {

            var book = new Book()
            {
                Title = "The First Step C#",
                Author = "Majd Al Nadaf",
                Description = "Descripe C# Basic",
                Id = Guid.NewGuid(),

            };


            //Arrange 

            _fixture._bookRepository.GetAllAsync().Returns(new List<Book>());

            //Act
            var result = await _fixture._sut.SearchForBooByISBNCodekAsync(book.ISBN);


            //Assertion

            result.Should().BeEmpty();
        }



        #endregion




    }
}
