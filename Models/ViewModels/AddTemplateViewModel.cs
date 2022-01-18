using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models.ViewModels
{
    [Keyless]
    public class AddTemplateViewModel
    {
        public Template Template { get; set; }

        public ICollection<List> Lists { get; set; }
    }
}
