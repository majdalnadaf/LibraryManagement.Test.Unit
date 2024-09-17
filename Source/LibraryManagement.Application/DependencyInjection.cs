using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using LibraryManagement.Domain.Domains;
using FluentValidation.AspNetCore;
using LibraryManagement.Application.ValidationRules;
using LibraryManagement.Application.Services;

namespace LibraryManagement.Application
{



    public static class DependencyInjection
    {
       
        public static  IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddScoped<IBookManagement, BookManagement>();
            services.AddScoped<IMemberManagement, MemberManagement>();
            services.AddScoped<IBorrowingManagement, BorrowingManagement>();
            services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<BookValidator>());
            return services;
        }

    }
}
