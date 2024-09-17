using FluentAssertions;
using LibraryManagement.Api.Contract.Borrowing;
using LibraryManagement.Api.Controllers;
using LibraryManagement.Application.Services;
using LibraryManagement.Domain.Domains;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Api.Test.Unit.BorrowingControllerTests
{
    public class BorrowingControllerTests
    {
        private readonly BorrowingController _sut;
        private readonly IBorrowingManagement _borrowingManagement = Substitute.For<IBorrowingManagement>();

        public BorrowingControllerTests()
        {
            _sut = new(_borrowingManagement);
        }



        [Fact]
        public async Task BorrowBook_ShouldReturnsStatusCode200_WhenBookWithValidData()
        {
            var request = new BorrowBookRequest(memberId: Guid.NewGuid(), bookId: Guid.NewGuid());

            var dateTime = DateTime.Now;
            var message = "Success";

            //Arrange 
            _borrowingManagement.BorrowBook(memberId: request.memberId, bookId: request.bookId).Returns((true, dateTime, message));

            // Act 
            var result = (OkObjectResult)await _sut.BorrowBookAsync(request);

            // Assert

            var excpectedResponse = new BorrowingResponse<DateTime>(data: dateTime, message: message);

            result.StatusCode.Should().Be(200);
            result.Value.Should().BeEquivalentTo(excpectedResponse);

        }

        [Fact]
        public async Task BorrowBookAsync_ShouldReturnsStatusCode400_WhenBookWithInvalidData()
        {
            var request = new BorrowBookRequest(memberId: Guid.Empty, bookId: Guid.Empty);

            var dateTime = DateTime.MinValue;
            var message = "Invalid book/member id";

            //Arrange 
            _borrowingManagement.BorrowBook(memberId: request.memberId, bookId: request.bookId).Returns((false, dateTime, message));

            // Act 
            var result = (BadRequestObjectResult)await _sut.BorrowBookAsync(request);

            // Assert



            result.StatusCode.Should().Be(400);
            result.Value.Should().Be(message);

        }



        [Fact]
        public async Task ReturnBookAsync_ShouldReturnStatusCode200_WhenIdsAreValidAndBookIsAvailable()
        {
            var request = new ReturnBookRequest(memberId: Guid.NewGuid(), bookId: Guid.NewGuid(), borrowingId: Guid.NewGuid());


            //Arrange 

            _borrowingManagement.ReturnBook(memberId: Arg.Any<Guid>(), bookId: Arg.Any<Guid>(), borrowBookId: Arg.Any<Guid>()).Returns(true);

            // Act 
            var result = (OkObjectResult) await _sut.ReturnBookAsync(request);


            //Assert
            var excepctedMessage = "Book was returnd";

            result.StatusCode.Should().Be(200);
            result.Value.Should().Be(excepctedMessage);
        }



        [Fact]
        public async Task ReturnBookAsync_ShouldReturnStatusCode400_WhenIdsAreInvalidOrBookNotAvailable()
        {
            var request = new ReturnBookRequest(memberId: Guid.Empty, bookId: Guid.Empty, borrowingId: Guid.Empty);


            //Arrange 

            _borrowingManagement.ReturnBook(memberId: Arg.Any<Guid>(), bookId: Arg.Any<Guid>(), borrowBookId: Arg.Any<Guid>()).Returns(false);

            // Act 
            var result = (BadRequestObjectResult)await _sut.ReturnBookAsync(request);


            //Assert
            var excepctedMessage = "Invalid data";

            result.StatusCode.Should().Be(400);
            result.Value.Should().Be(excepctedMessage);
        }

        [Fact]
        public async Task ViewBorrowingHistoryByMemberId_ShouldReturnsStatusCode200_WhenMemberIdIsValid()
        {
            var memberId = Guid.NewGuid();

            var lstBook = new List<BorrowBook>()
            {
                new BorrowBook() {MemberId = memberId},
            };

            //Arrnage
            _borrowingManagement.ViewBorrowingHistoryForAMember(Arg.Any<Guid>()).Returns(lstBook);

            //Act
            var result =  (OkObjectResult) await _sut.ViewBorrowingHistoryByMemberId(memberId);

            //Assert

            var excepctedResponse = new BorrowingResponse<List<BorrowBook>>(data:lstBook, message:"Success");


            result.StatusCode.Should().Be(200);

            // You can exclude any property you want....
            result.Value.Should().BeEquivalentTo(excepctedResponse, options => options.Excluding(x=> x.data));

        }


        [Fact]
        public async Task ViewBorrowingHistoryByMemberId_ShouldReturnsStatusCode404_WhenMemberIdIsNotValid()
        {
            var memberId = Guid.NewGuid();


            //Arrnage
            _borrowingManagement.ViewBorrowingHistoryForAMember(Arg.Any<Guid>()).ReturnsNull();

            //Act
            var result = (NotFoundResult)await _sut.ViewBorrowingHistoryByMemberId(memberId);

            //Assert

            result.StatusCode.Should().Be(404);


        }

        [Fact]
        public async Task CheckBookavailabilityAsync_ShouldReturnsStatusCode200_WhenBookIsAvailable()
        {
            var bookId = Guid.NewGuid();


            //Arrnage
            _borrowingManagement.CheckBookAvailability(Arg.Any<Guid>()).Returns(true);

            //Act
            var result = (OkObjectResult)await _sut.CheckBookavailabilityAsync(bookId);

            //Assert

            var excepectedMessage = "This book is available";

            result.StatusCode.Should().Be(200);
            result.Value.Should().Be(excepectedMessage);
        }


        [Fact]
        public async Task CheckBookavailabilityAsync_ShouldReturnsStatusCode404_WhenBookIsNotAvailable()
        {
            var bookId = Guid.NewGuid();


            //Arrnage
            _borrowingManagement.CheckBookAvailability(Arg.Any<Guid>()).Returns(false);

            //Act
            var result = (NotFoundObjectResult)await _sut.CheckBookavailabilityAsync(bookId);

            //Assert

            var excepectedMessage = "This book is not available";

            result.StatusCode.Should().Be(404);
            result.Value.Should().Be(excepectedMessage);
        }


    }
}
