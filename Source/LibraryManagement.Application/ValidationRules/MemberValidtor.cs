using FluentValidation;
using LibraryManagement.Domain.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.ValidationRules
{
    public  class MemberValidtor : AbstractValidator<Member>
    {
        public MemberValidtor() 
        {
            RuleFor(x=> x.Email).NotEmpty().NotNull();
            RuleFor(x => x.Id).NotEqual(Guid.Empty);
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.FullName).MaximumLength(300);

            //Add your other rules validation..
        }
    }

}
