using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        [Required]
        [StringLength(256)]
        public string Name { get; set; }
        public int ManagerId { get; set; }
        public List<Board> Boards { get; set; } = new List<Board>();
    }
}
