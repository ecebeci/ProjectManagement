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
        [StringLength(30)]
        public string Name { get; set; }
        public List<List> Lists { get; set; } = new List<List>();
    }
}
