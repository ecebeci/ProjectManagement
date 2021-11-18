using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models
{
    public class List
    {
        public int ID { get; set; }

        [Required]
        [StringLength(256)]
        public string Title { get; set; }
        //public List<Work> Works { get; set; } = new List<Work>();
    }

}
