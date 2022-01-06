using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data;
using ProjectManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ProjectsContext _context;

        public DashboardController(ProjectsContext context) //UserManager<IdentityUser> userManager
        {
            _context = context; 
        }

        [Authorize(Roles = "member")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null) 
            {
                return RedirectToAction("Index", "Projects"); 
            }

            Member member = await _context.Member.FirstOrDefaultAsync(m => m.Username == User.Identity.Name); // getting member
            if (member == null)
            {
                return NotFound();
            }

            var board = await _context.Board
                 .Include(b => b.Project)
                 .FirstOrDefaultAsync(m => m.BoardId == id);
            if (board == null)
            {
                return NotFound();
            }

            if (!MemberExists(board.ProjectId, member.MemberId))
            {
                return NotFound();
            }

            return View(GetList());
        }

        private bool MemberExists(int ProjectId, int MemberId)
        {
            return _context.ProjectMember.Any(e => e.ProjectId == ProjectId && e.MemberId == MemberId);
        }

        private List<List> GetList()
        {
            return new List<List>
            {
                new List {
                        Title = "List 1",
                        Works = new List<Work>()
                },
                new List {
                        Title = "List 2",
                        Works = new List<Work>()
                 },
                new List {
                        Title = "List 3",
                        Works = new List<Work>{new Work { Title = "Work 1", WorkId= 1} , new Work { Title = "Work 2", WorkId = 2 } }
                },
                 new List {
                        Title = "List 4",
                        Works = new List<Work>{new Work { Title = "Work 1", WorkId= 1} , new Work { Title = "Work 2", WorkId = 2 } }
                 },
               new List {
                        Title = "List 5",
                        Works = new List<Work>{new Work { Title = "Work 1", WorkId= 1} , new Work { Title = "Work 2", WorkId = 2 } }
                }
            };
        }
    }

}
