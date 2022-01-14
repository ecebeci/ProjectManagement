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
        public async Task<IActionResult> Index(string name,int id = -1, int page = 1)
        {
            Member member = await _context.Member.FirstOrDefaultAsync(m => m.Username == User.Identity.Name); // getting member
            if (member == null)
            {
                return NotFound();
            }

            if (!MemberExists((int)id, member.MemberId))
            {
                return RedirectToAction("Index", "Projects"); // returns to projects page
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
                // return View("Create", new Board { ProjectId = project.ProjectId }); direkt view a gonderirsen, board islenemeyecegi hata verir action olmali
                return RedirectToAction("Create",  new { projectId = id });
            }

            var boardsSearched = boards.Where(p => name == null || p.Title.Contains(name));

            var pagingInfo = new PagingInfo
            {
                CurrentPage = page,
                TotalItems = boardsSearched.Count()
            };

            if (pagingInfo.CurrentPage > pagingInfo.TotalPages)
            {
                pagingInfo.CurrentPage = pagingInfo.TotalPages;
            }

            if (pagingInfo.CurrentPage < 1)
            {
                pagingInfo.CurrentPage = 1;
            }

            var boardsSearchedPaginated = boardsSearched
                            .Skip((pagingInfo.CurrentPage - 1) * pagingInfo.PageSize)
                            .Take(pagingInfo.PageSize);


            return View(new BoardsViewModel
            {
                Boards = boardsSearchedPaginated,
                ProjectMember = memberProject,
                PagingInfo = pagingInfo,
                NameSearched = name  
            });  // boards
        }

        // GET: Boards/Details/5
        [Authorize(Roles = "member")]
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
        [Authorize(Roles = "member")]
        public async Task<ActionResult> Create(int? projectId)
        {

            if(projectId == null)
            {
                return NotFound();
            }

            var project = await _context.Project
               .FirstOrDefaultAsync(m => m.ProjectId == projectId)
               ;
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
                ViewBag.Message = "You are not project manager on this project! You can't create board.";
                return View("Failed");
            }
  
            return View(new Board { ProjectId = (int)projectId});
        }

        // POST: Boards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectId,Title,BoardDescription")] Board board, string submit)
        {
            
            Member member = await _context.Member.FirstOrDefaultAsync(m => m.Username == User.Identity.Name); // getting member
            if (member == null)
            {
                return NotFound();
            }

            if (!MemberExists((int)board.ProjectId, member.MemberId)) // check non-authorized access
            {
                return NotFound();
            }

            if (! (await ManagerCheck((int)board.ProjectId, member.MemberId))) 
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "You are not Project Manager! You can't create board.";
                return View("Failed");
            }

            if (ModelState.IsValid)
            {
                var datetime = DateTime.Now;
                board.CreatedDate = datetime;
                board.UpdatedDate = datetime;
                _context.Add(board);
                await _context.SaveChangesAsync();
                await UpdateProjectDate((int)board.ProjectId);
                switch (submit)
                {
                    case "Create a Board":
                        return RedirectToAction("Index", new { id = board.ProjectId }); // return index
                    case "Create with a Template":
                        var addedBoard = await _context.Board.FirstOrDefaultAsync(b => b.Title == board.Title && b.BoardDescription == board.BoardDescription && b.CreatedDate == datetime);
                        if(addedBoard == null)
                        {
                            return NotFound();
                        }
                        return RedirectToAction("TemplateSelector", new { id = addedBoard.BoardId }); // return template selector
                }
               
            }

            return View(board);
        }

        // GET: Boards/Edit/5
        [Authorize(Roles = "member")]
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

            var board = await _context.Board.Include(b => b.Project)
                            .FirstOrDefaultAsync(b => b.BoardId == id);
            if (board == null)
            {
                return NotFound();
            }

            if (!MemberExists((int)board.ProjectId, member.MemberId)) // check non-authorized access
            {
                return NotFound();
            }

            if (!(await ManagerCheck((int)board.ProjectId, member.MemberId)))
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "You are not project manager on this project! You can't edit board.";
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
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,BoardId,Title,BoardDescription")] Board board)
        {
            if (id != board.BoardId)
            {
                return NotFound();
            }

            Member member = await _context.Member.FirstOrDefaultAsync(m => m.Username == User.Identity.Name);
            if (member == null)
            {
                return NotFound();
            }

            if (!(await ManagerCheck((int)board.ProjectId, member.MemberId)))
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "You are not project manager on this project! You can't edit board.";
                return View("Failed");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    board.UpdatedDate = DateTime.Now;
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
                await UpdateProjectDate((int)board.ProjectId);
                return RedirectToAction("Index", new { id = board.ProjectId }); // return index
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

            if (!MemberExists((int)board.ProjectId, member.MemberId)) // check non-authorized access
            {
                return NotFound();
            }

            if (board == null)
            {
                return NotFound();
            }

            if (!(await ManagerCheck((int)board.ProjectId, member.MemberId)))
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "You are not project manager on this project! You can't delete board.";
                return View("Failed");
            }

            return View(board);
        }

        // POST: Boards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // TODO: Delete board with LISTS and WORKS
            var board = await _context.Board.FindAsync(id);
            _context.Board.Remove(board);
            await _context.SaveChangesAsync();
            await UpdateProjectDate((int)board.ProjectId);
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
