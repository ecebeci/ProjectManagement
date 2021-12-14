using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models.ViewModels
{
    public class BoardsProjectMember
    {
        public ICollection<Board> Boards { get; set; }
        public ProjectMember ProjectMember { get; set; }
    }
}
