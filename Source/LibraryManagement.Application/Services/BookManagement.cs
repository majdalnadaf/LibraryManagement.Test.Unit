using FluentValidation;
using LibraryManagement.Application.Helper;
using LibraryManagement.Application.InfrastructureServices;
using LibraryManagement.Application.ValidationRules;
using LibraryManagement.Domain.Domains;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Services
{
    public class BookManagement : IBookManagement
    {
        private readonly ILoggerAdapter<BookManagement> _logger;
        private readonly IBookRepositroy _bookRepositroy;
        private readonly ClsValiationGuid _clsValiationGuid = new();
        private readonly IValidator<Book> _validator;


        public BookManagement(ILoggerAdapter<BookManagement> logger, IBookRepositroy bookRepositroy, IValidator<Book> validator)
        {



            _logger = logger;
            _bookRepositroy = bookRepositroy;
            _validator = validator;

        } 

        public async Task<(bool, Guid)> CreateBookAsync(Book book)
        {
            try
            {
                if (book is null)
                {
                    return (false, Guid.Empty);
                }

                var validationResult = _validator.Validate(book);
                if (!validationResult.IsValid)
                {
                    return (false, Guid.Empty);
                }

                if(!await CheckDuplicateISBN(book.ISBN))
                    return (false, Guid.Empty);


                var result = await _bookRepositroy.CreateAsync(book);
                if (!result.Item1)
                {
                    return (false, Guid.Empty);

                }

                _logger.LogInformation("Book was created");

                return (true, result.Item2.Id);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Message");
                throw;
            }
        }

        public async Task<bool> DeleteBookAsync(Guid bookId)
        {
            try
            {
                if (!_clsValiationGuid.CheckGuidStatus(bookId))
                {
                    return false;
                }


                var result = await _bookRepositroy.DeleteAsync(bookId);
                if (!result)
                {
                    return false;

                }

                _logger.LogInformation("Book was created");
                return result;

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Message");
                throw;
            }
        }

        public async Task<List<Book>> SearchForBooByISBNCodekAsync(string isbn)
        {
            try
            {
                if (string.IsNullOrEmpty(isbn))
                {
                    return new List<Book>();
                }


                var lstBook = await _bookRepositroy.GetAllAsync();
                lstBook = lstBook.Where(a => a.ISBN == isbn).ToList();


                if (lstBook is null || lstBook.Count == 0)
                {
                    return new List<Book>();
                }

                _logger.LogInformation("Book was created");
                return lstBook.Where(a => a.ISBN == isbn).ToList();

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Message");
                throw;
            }
        }

        public async Task<List<Book>> SearchForBookByAuthorNameAsync(string author)
        {
            try
            {
                if (string.IsNullOrEmpty(author))
                {
                    return new List<Book>();
                }


                var lstBook = await _bookRepositroy.GetAllAsync();
                lstBook = lstBook.Where(a => a.Author == author).ToList();


                if (lstBook is null || lstBook.Count == 0)
                {
                    return new List<Book>();
                }

                _logger.LogInformation("Book was created");
                return lstBook.ToList();

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Message");
                throw;
            }
        }

        public async Task<List<Book>> SearchForBookByBookNameAsync(string title)
        {
            try
            {
                if (string.IsNullOrEmpty(title))
                {
                    return new List<Book>();
                }


                var lstBook = await _bookRepositroy.GetAllAsync();
                lstBook = lstBook.Where(a => a.Title == title).ToList();
                if (lstBook is null || lstBook.Count == 0)
                {
                    return new List<Book>();
                }

                _logger.LogInformation("Book was created");
                return lstBook.ToList();

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Message");
                throw;
            }
        }

        public async Task<bool> UpdateBookAsync( Book book)
        {
            try
            {
                if (book is null)
                {
                    return false;
                }

                var validationResult = _validator.Validate(book);
                if (!validationResult.IsValid)
                {
                    return false;
                }

                var result = await _bookRepositroy.UpdateAsync( book);
                if (!result.Item1)
                {
                    return false;

                }

                _logger.LogInformation("Book was created");
                return result.Item1;

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Message");
                throw;
            }
        }
        public async Task<bool> CheckDuplicateISBN(string isbn)
        {
            var lstBook = await _bookRepositroy.GetAllAsync();
            if (lstBook is null)
                return true;

            var bookWithDuplicatedISBN = lstBook.Where(a => a.ISBN == isbn).FirstOrDefault();
            if (bookWithDuplicatedISBN is not null)
                return false;

            return true;

        }
    
    
    }
}
