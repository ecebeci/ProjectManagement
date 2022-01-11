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
                 .Include(b => b.Lists)
                 .FirstOrDefaultAsync(m => m.BoardId == id);
            if (board == null)
            {
                return NotFound();
            }

            if (!MemberExists(board.ProjectId, member.MemberId))
            {
                return NotFound();
            }

            var list = board.Lists;
            if (list == null || list.Count == 0)
            {
                return RedirectToAction("CreateList", new { id = board.BoardId });
            }

            return View(list);
        }

        // GET: Dashboard/CreateList/<BoardId> !
        public async Task<ActionResult> CreateList(int id)
        {
            var board = await _context.Board
               .Include(m => m.Project)
               .FirstOrDefaultAsync(m => m.BoardId == id);

            if (board == null)
            {
                return NotFound();
            }

            Member member = await _context.Member
                 .FirstOrDefaultAsync(m => m.Username == User.Identity.Name);
            if (member == null)
            {
                return NotFound();
            }

            if (!MemberExists((int)board.Project.ProjectId, member.MemberId)) // check non-authorized access
            {
                return NotFound();
            }

            // TODO: Is not working!
            if (!(await ManagerCheck((int)board.Project.ProjectId, member.MemberId)))
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "You are not project manager on this project! You can't create board.";
                return View("Failed");
            }

            // TODO : Is Cancelled or Deleted!!!!!!!!!!!!
            if (board.Project.IsCancelled)
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "The project is cancelled. You can't create list";
                ViewBag.BoardId = board.BoardId;
                return View("Failed");
            }
            if (board.Project.IsDeleted)
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "The project is cancelled. You can't create list";
                ViewBag.BoardId = board.BoardId;
                return View("Failed");
            }
            else if (board.Project.IsFinished)
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "The project is finished. You can't create list";
                ViewBag.BoardId = board.BoardId;
                return View("Failed");
            }

            return View(new List { BoardId = board.BoardId });
        }

        // POST: Boards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateList([Bind("BoardId,Title")] List list)
        {

            Member member = await _context.Member.FirstOrDefaultAsync(m => m.Username == User.Identity.Name); // getting member
            if (member == null)
            {
                return NotFound();
            }

            var board = await _context.Board
               .Include(m => m.Project)
               .FirstOrDefaultAsync(m => m.BoardId == list.BoardId);
            if (board == null)
            {
                return NotFound();
            }

            if (!MemberExists(board.ProjectId, member.MemberId)) // check non-authorized access
            {
                return NotFound();
            }

            if (!(await ManagerCheck(board.ProjectId, member.MemberId)))
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "You are not Project Manager! You can't create board.";
                return View("Failed");
            }

            if (ModelState.IsValid)
            {
                _context.Add(new List { 
                    Title = list.Title , 
                    BoardId = list.BoardId, 
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now  }
                );
                board.UpdatedDate = DateTime.Now;
                _context.Update(board);
                await _context.SaveChangesAsync();
                await UpdateProjectDate(board.ProjectId);
                return RedirectToAction("Index", new { id = board.BoardId }); // return index
            }

            return View(list);
        }

        private bool BoardExists(int id)
        {
            return _context.Board.Any(e => e.BoardId == id);
        }

        private bool MemberExists(int ProjectId, int MemberId)
        {
            return _context.ProjectMember.Any(e => e.ProjectId == ProjectId && e.MemberId == MemberId);
        }

        private bool ManagerCheck(Project project, int MemberId)
        {
            if (project.ManagerId != MemberId) // Check non-authorized access. If the user is not project member what thet selected, they cant delete.
                return false;

            return true;
        }

        private async Task<bool> ManagerCheck(int projectId, int MemberId)
        {
            var project = await _context.Project
           .FirstOrDefaultAsync(m => m.ProjectId == projectId);
            if (project == null)
            {
                return false;
            }

            if (project.ManagerId != MemberId) // Check non-authorized access. If the user is not project member what thet selected, they cant delete.
                return false;

            return true;
        }

        private async Task<bool> UpdateProjectDate(int projectId)
        {
            var project = await _context.Project
            .FirstOrDefaultAsync(m => m.ProjectId == projectId);
            if (project == null)
            {
                return false;
            }

            try
            {
                project.UpdatedDate = DateTime.Now;
                _context.Update(project);
                await _context.SaveChangesAsync(); // save, before to use new id's
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }
    }
}

    
