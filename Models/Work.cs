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
        [Required]
        [StringLength(512)]
        public string Title { get; set; }
        public int ListId { get; set; }
        public int Priority { get; set; }
        public int OrderNumber { get; set; }
        public bool IsDone { get; set; }
    }
}
