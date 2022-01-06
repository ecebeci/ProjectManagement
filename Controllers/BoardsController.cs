using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data;
using ProjectManagement.Models;
using ProjectManagement.Models.ViewModels;

namespace ProjectManagement.Controllers
{
    public class BoardsController : Controller
    {
        private readonly ProjectsContext _context;

        public BoardsController(ProjectsContext context)
        {
            _context = context;
        }

        // GET: Boards/5
        [Authorize(Roles = "member")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null) // if there is no id on route-id
            {
                return RedirectToAction("Index", "Projects"); // returns to projects page
            }

            Member member = await _context.Member.FirstOrDefaultAsync(m => m.Username == User.Identity.Name); // getting member
            if (member == null)
            {
                return NotFound();
            }

            if (!MemberExists((int)id, member.MemberId))
            {
                return NotFound();
            }

            var memberProject = await _context.ProjectMember
             .Where(p => p.ProjectId == id) // checking id
             .Where(p => p.MemberId == member.MemberId)
             .Include(p => p.Member)
             .Include(p => p.Project.Manager) // include manager (many - (to) - many)
             .FirstOrDefaultAsync();
            
            var project = await _context.Project
              .Include(b => b.Boards) // Relation!
              .FirstOrDefaultAsync(m => m.ProjectId == id);

            // if the project not found
            if (project == null)
            {
                return RedirectToAction("Index", "Projects"); // returns to projects page
            }

            if (project.IsDeleted)
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "Project is deleted! You can't access.";
                return View("Failed"); // ./shared/failed
            }

            var boards = project.Boards;
            if (boards.Count == 0) // If the project has no boards
            {
                return View("Create", new Board { ProjectId = project.ProjectId });
            }

            return View(new BoardsProjectMember
            {
                Boards = boards,
                ProjectMember = memberProject
            });  // boards
        }

        // GET: Boards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
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

            return View(board);
        }

        // GET: Boards/Create/<ProjectId> !
        public async Task<ActionResult> Create(int? projectId)
        {

            if(projectId == null)
            {
                return NotFound();
            }

            var project = await _context.Project
               .FirstOrDefaultAsync(m => m.ProjectId == projectId);
            if (project == null)
            {
                return NotFound();
            }

            Member member = await _context.Member
                 .FirstOrDefaultAsync(m => m.Username == User.Identity.Name);
            if (member == null)
            {
                return NotFound();
            }

            if (!MemberExists((int)projectId, member.MemberId)) // check non-authorized access
            {
                return NotFound();
            }

            // TODO: Is not working!
            if (!(await ManagerCheck((int) projectId, member.MemberId)))
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "You are not Project Manager! You can't create board.";
                return View("Failed");
            }

            await UpdateProjectDate((int)projectId);

            return View(new Board { ProjectId = project.ProjectId });
        }

        // POST: Boards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectId,Title,BoardDescription")] Board board)
        {
            
            Member member = await _context.Member.FirstOrDefaultAsync(m => m.Username == User.Identity.Name); // getting member
            if (member == null)
            {
                return NotFound();
            }

            if (!MemberExists(board.ProjectId, member.MemberId)) // check non-authorized access
            {
                return NotFound();
            }

            if (! (await ManagerCheck(board.ProjectId, member.MemberId))) 
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "You are not Project Manager! You can't create board.";
                return View("Failed");
            }

            if (ModelState.IsValid)
            {
                board.CreatedDate = DateTime.Now;
                board.UpdatedDate = DateTime.Now;
                _context.Add(board);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = board.ProjectId }); // return index
            }

            await UpdateProjectDate(board.ProjectId);

            return View(board);
        }

        // GET: Boards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Member member = await _context.Member.FirstOrDefaultAsync(m => m.Username == User.Identity.Name); // getting member
            if (member == null)
            {
                return NotFound();
            }

            var board = await _context.Board.FindAsync(id);
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
                ViewBag.Message = "You are not Project Manager! You can't edit board.";
                return View("Failed");
            }    

            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "Name", board.ProjectId);
            return View(board);
        }

        // POST: Boards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BoardId,ProjectId,Title,BoardDescription,CreatedDate,UpdatedDate")] Board board)
        {
            if (id != board.BoardId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(board);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!BoardExists(board.BoardId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.Title = "Unexpected Error";
                        ViewBag.Message = ex.HResult;
                        return View("Failed");
                    }
                }
                await UpdateProjectDate(board.ProjectId);
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "Name", board.ProjectId);
            return View(board);
        }

        // GET: Boards/Delete/5
        [Authorize(Roles = "member")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Member member = await _context.Member.FirstOrDefaultAsync(m => m.Username == User.Identity.Name);
            if (member == null)
            {
                return NotFound();
            }

            var board = await _context.Board
                .Include(b => b.Project)
                .FirstOrDefaultAsync(m => m.BoardId == id);

            if (!MemberExists(board.ProjectId, member.MemberId)) // check non-authorized access
            {
                return NotFound();
            }

            if (board == null)
            {
                return NotFound();
            }

            if (!(await ManagerCheck(board.ProjectId, member.MemberId)))
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "You are not Project Manager! You can't delete board.";
                return View("Failed");
            }

            return View(board);
        }

        // POST: Boards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var board = await _context.Board.FindAsync(id);
            _context.Board.Remove(board);
            await _context.SaveChangesAsync();
            await UpdateProjectDate(board.ProjectId);
            return RedirectToAction(nameof(Index));
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
