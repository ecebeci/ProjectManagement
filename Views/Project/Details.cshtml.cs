using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data;
using ProjectManagement.Models;

namespace ProjectManagement.Views.Project
{
    public class DetailsModel : PageModel
    {
        private readonly ProjectManagement.Data.ProjectsContext _context;

        public DetailsModel(ProjectManagement.Data.ProjectsContext context)
        {
            _context = context;
        }

        public Project Project { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project = await _context.Project.FirstOrDefaultAsync(m => m.ProjectId == id);

            if (Project == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
