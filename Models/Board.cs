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

        public int ProjectId { get; set; } // daha sonra kullanilir
        public Project Project { get; set; } // Every Board has a one project (one-to-many)

        [Required]
        [StringLength(256)] // Think about that
        public string Title { get; set; }
        public string BoardDescription { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ICollection<List> Lists { get; set; } 
    }
}
