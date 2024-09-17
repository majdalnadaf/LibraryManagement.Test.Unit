using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Domain.Domains
{
    public class Book
    {

         
        public Book() { }

        [Key]
        public Guid Id { get; set; }
        [StringLength(250)]
        public bool IsAvailable { get; set; } = true;

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
        public string Author { get; set; }

        public string ISBN { get; set; }



    }
}
