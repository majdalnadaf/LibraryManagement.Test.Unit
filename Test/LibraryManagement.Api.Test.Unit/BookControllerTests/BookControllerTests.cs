using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using LibraryManagement.Api.AutoMapper;
using LibraryManagement.Api.Contract.Books;
using LibraryManagement.Api.Contract.ValidationRules;
using LibraryManagement.Api.Controllers;
using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.ReturnsExtensions;

namespace LibraryManagement.Api.Test.Unit.BookControllerTests 
{
    public class BookControllerTests
    {
        private readonly BookController _sut;
        private readonly IBookManagement _bookManagement = Substitute.For<IBookManagement>();
        private readonly IValidator<CreateBookRequest> _createBookRequestValidator;
        private readonly IValidator<UpdateBookRequest> _updateBookRequestValidtor;
        private readonly IMapper _mapper;
        public BookControllerTests()
        {



            var service = new ServiceCollection();
            service.AddScoped<IValidator<CreateBookRequest>, CreateBookRequestValidator>();
            service.AddScoped<IValidator<UpdateBookRequest>, UpdateBookRequestValidator>();
            service.AddAutoMapper(typeof(MappingProfile));

            var serviceProvider = service.BuildServiceProvider();
            _createBookRequestValidator = serviceProvider.GetRequiredService<IValidator<CreateBookRequest>>();
            _updateBookRequestValidtor = serviceProvider.GetRequiredService<IValidator<UpdateBookRequest>>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();


            _sut = new(_bookManagement, _createBookRequestValidator, _updateBookRequestValidtor);
        }



        [Fact]
        public async Task CreateBookAsync_ShouldReturnsStatusCode201_WhenBookHasValidData()
        {
            CreateBookRequest request = new(
                title: "C#",
                description: "Basic of C#",
                author: "Majd Al Nadaf",
                ISBN: "459sdf-998ddfd"

                );

            Book book = _mapper.Map<Book>(request);
            book.Id = Guid.NewGuid();

            BookResponse<Guid> excpectedResponse = new(data: book.Id, "Book was created successfully");

            // Arrange 
            _bookManagement.CreateBookAsync(Arg.Do<Book>(x => book = x)).Returns((true, book.Id));

            // Act 

            var result = (CreatedAtActionResult)await _sut.CreateBookAsync(request);


            // Assert
            result.StatusCode.Should().Be(201);
            result.Value.Should().BeEquivalentTo(excpectedResponse);

        }



        [Fact]
        public async Task CreateBookAsync_ShouldReturnsBadRequest_WhenBookHasInvalidData()
        {
            CreateBookRequest request = new(
                title: string.Empty,  // Invalid data
                description: "Basic of C#",
                author: "Majd Al Nadaf",
                ISBN: "459sdf-998ddfd"

                );

            Book book = _mapper.Map<Book>(request);
            book.Id = Guid.NewGuid();


            // Arrange 
            _bookManagement.CreateBookAsync(Arg.Do<Book>(x => book = x)).Returns((true, book.Id));

            // Act 

            var result = (BadRequestObjectResult)await _sut.CreateBookAsync(request);


            // Asserts
            result.StatusCode.Should().Be(400);


        }


        [Fact]
        public async Task CreateBookAsync_ShouldRetunsBadRequest_WhenThereIsDulicpationISBN()
        {
            CreateBookRequest request = new(
               title: "C#",
               description: "Basic of C#",
               author: "Majd Al Nadaf",
               ISBN: "459sdf-998ddfd"

               );

            Book book = _mapper.Map<Book>(request);
            book.Id = Guid.NewGuid();

            var excpectedMessage = "The book wan not created";


            // Arrange 
            _bookManagement.CreateBookAsync(Arg.Do<Book>(x => book = x)).Returns((false, Guid.Empty));

            // Act 

            var result = (BadRequestObjectResult)await _sut.CreateBookAsync(request);


            // Asserts
            result.StatusCode.Should().Be(400);
            result.Value.Should().Be(excpectedMessage);

        }


        [Fact]
        public async Task UpdateBookAsync_ShouldReturnsStaturCode200_WhenBookHasValidData()
        {
            UpdateBookRequest request = new(
               id: Guid.NewGuid(),
              title: "C#",
              description: "Basic of C#",
              author: "Majd Al Nadaf",
              ISBN: "459sdf-998ddfd",
              isAvailable: true
              );

            Book book = _mapper.Map<Book>(request);

            BookResponse<Book> excpectedResponse = new(data: book, "Book was updated");

            // Arrange 
            _bookManagement.UpdateBookAsync(Arg.Do<Book>(x => book = x)).Returns(true);

            // Act 

            var result = (OkObjectResult)await _sut.UpdateBookAsync(request);


            // Asserts
            result.StatusCode.Should().Be(200);
            result.Value.Should().BeEquivalentTo(excpectedResponse);
        }


        [Fact]
        public async Task UpdateBookAsync_ShouldReturnsBadRequest_WhenBookHasInvalidId()
        {
            UpdateBookRequest request = new(
               id: Guid.Empty,
              title: "C#",
              description: "Basic of C#",
              author: "Majd Al Nadaf",
              ISBN: "459sdf-998ddfd",
              isAvailable: true
              );

            Book book = _mapper.Map<Book>(request);



            // Arrange 
            _bookManagement.UpdateBookAsync(Arg.Do<Book>(x => book = x)).Returns(true);

            // Act 

            var result = (BadRequestObjectResult)await _sut.UpdateBookAsync(request);


            // Asserts
            result.StatusCode.Should().Be(400);

        }

        [Fact]
        public async Task UpdateBookAsync_ShouldReturnsBadRequest_WhenBookWithMissingRequiredData()
        {
            UpdateBookRequest request = new(
               id: Guid.NewGuid(),
              title: string.Empty, // missing required data
              description: "Basic of C#",
              author: "Majd Al Nadaf",
              ISBN: "459sdf-998ddfd",
              isAvailable: true
              );

            Book book = _mapper.Map<Book>(request);



            // Arrange 
            _bookManagement.UpdateBookAsync(Arg.Do<Book>(x => book = x)).Returns(true);

            // Act 

            var result = (BadRequestObjectResult)await _sut.UpdateBookAsync(request);


            // Asserts
            result.StatusCode.Should().Be(400);

        }



        [Fact]
        public async Task DeleteBookAsync_ShouldRetrunsStatusCode200_WhenBookIdIsExists()
        {

            var bookId = Guid.NewGuid();
            var excpectedMessage = "Book was deleted";
            //Arrange 

            _bookManagement.DeleteBookAsync(bookId).Returns(true);

            // Act 

            var result = (OkObjectResult) await _sut.DeleteBookAsync(bookId);


            // Assert

            result.StatusCode.Should().Be(200);
            result.Value.Should().Be(excpectedMessage);

        }

        [Fact]
        public async Task DeleteBookAsync_ShouldRetrunsProblem_WhenBookIdIsNotExists()
        {

            var bookId = Guid.NewGuid();
            //Arrange 

            _bookManagement.DeleteBookAsync(bookId).Returns(false);

            // Act 

            var result = (ObjectResult)await _sut.DeleteBookAsync(bookId);


            // Assert
             result.StatusCode.Should().Be(500);
        }


        [Fact]
        public async Task GetAllBookByNameAsync_ShouldReturnsListOfBook_WhenBookTitleIsExists()
        {
            var bookTitle = "C#";
            var lstBook = new List<Book>() 
            {
                new Book { Title = bookTitle } 
            };



            // Arrange 
            _bookManagement.SearchForBookByBookNameAsync(Arg.Any<string>()).Returns(lstBook);

            // Act 
            var result =  (OkObjectResult)await _sut.GetAllBookByNameAsync(bookTitle);


            // Assert

            BookResponse<List<Book>> excpectedResponse = new(data: lstBook , message:"Success");


            result.StatusCode.Should().Be(200);
            result.Value.Should().BeEquivalentTo(excpectedResponse);
        }



        [Fact]
        public async Task GetAllBookByNameAsync_ShouldReturnsBadRequest_WhenBookTitleIsNotExists()
        {
            var bookTitle = "C#";




            // Arrange 
            _bookManagement.SearchForBookByBookNameAsync(Arg.Any<string>()).ReturnsNull();

            // Act 
            var result = (BadRequestObjectResult)await _sut.GetAllBookByNameAsync(bookTitle);


            // Assert

            BookResponse<List<Book>> excpectedResponse = new(data: null!, message: "Not found");


            result.StatusCode.Should().Be(400);
            result.Value.Should().BeEquivalentTo(excpectedResponse);
        }

    }
}
