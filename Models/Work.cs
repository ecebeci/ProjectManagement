using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models
{
    public class Work
    {
        public int WorkId { get; set; }

        public int ListId { get; set; }
        public List List { get; set; }

        public int MemberId { get; set; }
        public Member Member { get; set; } // ICollection

        [Required]
        [StringLength(512)]
        public string Title { get; set; }

        public int Priority { get; set; }

        public int Status { get; set; } 

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
