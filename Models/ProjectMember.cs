using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models
{
    public class ProjectMember
    {
    
        public int MemberId { get; set; }
        public Member Member { get; set; } // many to many

        public int ProjectId { get; set; }
        public Project Project { get; set; } // many to many
    }
}
