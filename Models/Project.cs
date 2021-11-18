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

        // TODO: Add User List on Project
        // public ICollection<TBD> Members { get; set; } ?

        public ICollection<Board> Boards { get; set; }
    }
}
