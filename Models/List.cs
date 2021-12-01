using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models
{
    public class List
    {
        public int ListId { get; set; }

        public int BoardId { get; set; } 
        public Board Boards { get; set; }

        [Required]
        [StringLength(512)] 
        public string Title { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
       
        public ICollection<Work> Works { get; set; } 
    }

}
