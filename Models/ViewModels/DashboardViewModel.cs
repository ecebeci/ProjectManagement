using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<ProjectManagement.Models.List> Lists { get; set; }
        public Work Work { get; set; } // for submit created work
    }
}
