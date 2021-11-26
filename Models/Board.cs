using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models
{
    public class Board
    {
        public int BoardId { get; set; }

        [Required]
        [StringLength(30)] // Think about that
        public string Title { get; set; }
        public string BoardDescription { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public Project Project { get; set; } // Every Board has a one project (one-to-many)
        public ICollection<List> Lists { get; set; } 
    }
}
