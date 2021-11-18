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
        [StringLength(256)]
        public string Name { get; set; }

        // TODO: Add Selected template.
        // public string SelectedTemplate { get; set; }

        // TODO : List Board ekle
        //public ICollection<List> Lists { get; set; }S

    }
}
