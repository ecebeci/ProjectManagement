using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models
{
    public class Member
    {
        public int MemberId { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Username { get; set; }

        [Required]
        [StringLength(512)]
        public string Name { get; set; }

        public ICollection<ProjectMember> ProjectMembers { get; set; }

        public ICollection<Project> Projects { get; set; } // for Project ManagerId

        public ICollection<Work> Works { get; set; } // Every member can have more works
    }

}
