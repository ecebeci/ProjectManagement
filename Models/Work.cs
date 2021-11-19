using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models
{
    public class Work
    {
        [Required]
        [StringLength(256)]
        public string Title { get; set; }

        public int Priority { get; set; }

        public int OrderNumber { get; set; }

        public bool IsDone { get; set; }
    }
}
