using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute.ExceptionExtensions;
namespace LibraryManagement.Application.Test.Unit.LibraryManagementTests
{
    public class LibraryManagementTests : IDisposable
    {
        private readonly LibraryManagement _sut;
        public LibraryManagementTests()
        {
            _sut = new();
        }


        [Fact(Timeout = 3000)]   // You can put the timeout as you want
        public async Task Dosomthing()
        {
            await Task.Delay(2000);
        }




        [Theory]
        [InlineData(0, 0.0)]
        [InlineData(1, 0.5, Skip = "This test was ignored")]
        [InlineData(5, 2.5)]
        [MemberData(nameof(CalculateLateFeeTestsData))]
        public void CalculateLateFee_ShouldReturnsCorrectFee_WhenMemberIsValidAndNumberOfDaysLateGreaterThanZero(int daysLate, double expectedFee)
        {

            //Act 
            var result = _sut.CalculateLateFee(daysLate);

            //Assert
            result.Should().Be(expectedFee);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100000)]
        [ClassData(typeof(ClsCalculateLateFeeTestsData))]
        public void CalculateLateFee_ShouldThrowException_WhenDaysOfLateIsNegative(int daysLate)
        {


            //Act
            Action act = () => _sut.CalculateLateFee(daysLate);

            //Assert

            act.Should().Throw<ArgumentException>()
            .WithMessage("Days late cant be negative.");
        }

        public void Dispose()
        {
            // Clean up 

            return;
        }

        public static IEnumerable<object[]> CalculateLateFeeTestsData() => new List<object[]>
        {
            new object[] {0,0},
            new object[] {7,3.5},
            new object[] {16,8},
        };


    }


    public class ClsCalculateLateFeeTestsData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
               yield return new object[] {-9};
               yield return new object[] {-5};
               yield return new object[] {0-3};
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


}
