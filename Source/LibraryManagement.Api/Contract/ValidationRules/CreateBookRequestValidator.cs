using FluentValidation;
using LibraryManagement.Api.Contract.Books;

namespace LibraryManagement.Api.Contract.ValidationRules
{
    public class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
    {
        public CreateBookRequestValidator()
        {
            RuleFor(x => x.title).NotEmpty();
            RuleFor(x => x.ISBN).NotEmpty();
            RuleFor(x => x.author).NotEmpty();
            RuleFor(x => x.description).NotEmpty().MaximumLength(1000);
            
            
        }
    }
}
