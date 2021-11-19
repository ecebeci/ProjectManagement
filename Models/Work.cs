using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models
{
    public class Work
    {
        public string Title { get; set; }

        [Required]
        [StringLength(256)]
        public int Priority { get; set; }

        public int OrderNumber { get; set; }

        public bool IsDone { get; set; }
    }
}
