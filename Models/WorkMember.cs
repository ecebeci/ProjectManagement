using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models
{
    public class WorkMember
    {
        public int MemberId { get; set; }
        public Member Member { get; set; } // many to many

        public int WorkId { get; set; }
        public Work Work { get; set; } // many to many
    }
}
