using FluentAssertions.Common;
using FluentValidation;
using LibraryManagement.Application.InfrastructureServices;
using LibraryManagement.Application.Services;
using LibraryManagement.Application.ValidationRules;
using LibraryManagement.Domain.Domains;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LibraryManagement.Application.Test.Unit.BookManagementTests
{
    public class BookManagementClassFixture : IAsyncLifetime
    {

        public BookManagement _sut { get; private set; }
        public  IBookRepositroy _bookRepository = Substitute.For<IBookRepositroy>();
        public  ILoggerAdapter<BookManagement> _logger = Substitute.For<ILoggerAdapter<BookManagement>>();
        public  IValidator<Book> _validator = Substitute.For<IValidator<Book>>();


        public BookManagementClassFixture()
        {

            var services = new ServiceCollection();
            services.AddScoped<IValidator<Book>, BookValidator>();
            var serviceProvider = services.BuildServiceProvider();
            _validator = serviceProvider.GetRequiredService<IValidator<Book>>();


            _sut = new(_logger, _bookRepository, _validator);
        }


        
        public async Task DisposeAsync()
        {

            //Setup code
            await Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            //Cleanup code
            await Task.CompletedTask;
        }

        public void Reset()
        {
            var services = new ServiceCollection();
            services.AddScoped<IValidator<Book>, BookValidator>();
            var serviceProvider = services.BuildServiceProvider();
            _validator = serviceProvider.GetRequiredService<IValidator<Book>>();

            _sut = new(_logger, _bookRepository, _validator);
        }

    }
}
