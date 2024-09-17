using FluentValidation;
using LibraryManagement.Domain.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.ValidationRules
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor<string>(x => x.Title).NotEmpty().NotNull(); 
        }




    }
}
