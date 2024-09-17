using FluentValidation;
using LibraryManagement.Domain.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.ValidationRules
{
    public class BorrowBookValidator : AbstractValidator<BorrowBook>
    {
        public BorrowBookValidator()
        {

            RuleFor<DateTime>(x => x.BorrowDate).NotEqual(DateTime.MinValue); 
            RuleFor<DateTime>(x => x.DueDate).NotEqual(DateTime.MinValue); 
            RuleFor<Guid>(x=> x.MemberId).NotEqual(Guid.Empty);
            RuleFor<Guid>(x=> x.BookId).NotEqual(Guid.Empty);

            
        }
    }
}
