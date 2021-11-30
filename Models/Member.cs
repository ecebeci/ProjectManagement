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
        [StringLength(15)]
        public string Username { get; set; }

        [Required]
        [StringLength(512)]
        public string Name { get; set; }
    
        /*[Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; } */

        [Required]
        public int TitleId { get; set; }

        public ICollection<ProjectMember> ProjectMembers { get; set; }

        public ICollection<Work> Works { get; set; } // Every member can have more works
    }

}
