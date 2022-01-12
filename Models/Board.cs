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

        public int? ProjectId { get; set; } 
        public Project Project { get; set; } // every Board has a one project (one-to-many)

        public int? TemplateId { get; set; }
        public Template Template { get; set; } // every Template has boards (one-to-many)

        [Required]
        [StringLength(256)] 
        public string Title { get; set; }
        public string BoardDescription { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ICollection<List> Lists { get; set; } 
    }
}
