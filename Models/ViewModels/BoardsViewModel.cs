using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models.ViewModels
{
    public class BoardsViewModel
    {
        public IEnumerable<Board> Boards { get; set; }
        public ProjectMember ProjectMember { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string NameSearched { get; set; }
    }
}
