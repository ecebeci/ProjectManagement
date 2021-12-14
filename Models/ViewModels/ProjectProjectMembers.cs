using ProjectManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For the Details and Edit pages
namespace ProjectManagement.Models.ViewModels
{
    public class ProjectProjectMembers
    {
        public Project Project { get; set; }
        public List<ProjectMember> ProjectMembers { get; set; }
    }
}
