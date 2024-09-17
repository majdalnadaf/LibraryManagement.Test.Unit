using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Domain.Domains
{
    public class Member
    {
        public Member() { }

        [Key]
        public Guid Id { get; set; }    

        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
