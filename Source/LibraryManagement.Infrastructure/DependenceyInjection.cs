using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Infrastructure
{
    public static  class DependenceyInjection
    {
     
        public static IServiceCollection AddInfrstructure(this IServiceCollection service)
        {

            return service;
        }

    }
}
