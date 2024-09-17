using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Domain.Domains
{
    public class BorrowBook  
    {

        public Guid Id { get; set; }
        public Guid BookId { get; set; } 
        public  DateTime BorrowDate { get; set; }

        public DateTime DueDate { get; set; } 
        public Guid MemberId { get; set; }

        public virtual Member Member { get; set; } = null!;
        public virtual Book Book { get; set; } = null!;
    }
}
