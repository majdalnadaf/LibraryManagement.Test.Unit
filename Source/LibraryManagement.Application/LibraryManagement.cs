using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


// This line of code for i can access the internal member
[assembly: InternalsVisibleTo("LibraryManagement.Application.Test.Unit")]


namespace LibraryManagement.Application
{

    // Make this class internal to try to test it from the test projct

    internal class LibraryManagement 
    {
        private const double DailyLateFee = 0.5;
        public double CalculateLateFee(int daysLate)
        {
            if (daysLate < 0)
            {
                throw new ArgumentException("Days late cant be negative.");
            }

            double lateFree = DailyLateFee * daysLate;
            return lateFree;
         

        }
    }
}
