using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models
{
    public class Template
    {
        public int TemplateId { get; set; }
        public int Name { get; set; }

        public ICollection<Board> Boards { get; set; }
    }
}
