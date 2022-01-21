using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data;
using ProjectManagement.Models;
using ProjectManagement.Models.ViewModels;
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
                    .ThenInclude(a => a.Works)
                        .ThenInclude(b => b.WorkMembers)
                            .ThenInclude(b => b.Member)
                .FirstOrDefaultAsync(m => m.BoardId == id);


            if (board == null) 
            {
                return NotFound();
            }

            if (!MemberExists((int)board.Project.ProjectId, member.MemberId)) // check non-authorized access
            {
                return NotFound();
            }

            if (!board.Lists.Any()) { 
            return RedirectToAction("CreateList", new { id = id });
            }


            return View(new DashboardViewModel { Lists = board.Lists.ToList(), Work = new() });
        }

        // GET: Dashboard/CreateList/<BoardId> !
        [Authorize(Roles = "member")]
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

            if (!MemberExists((int)board.ProjectId, member.MemberId)) // check non-authorized access
            {
                return NotFound();
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
                await UpdateProjectDate((int)board.ProjectId);
                return RedirectToAction("Index", new { id = board.BoardId }); // return index
            }

            list.BoardId = board.BoardId;
            return View(list);
        }

        // POST: Dashboard/ImportTemplate/<BoardId>
        [Authorize(Roles = "member")]
        public async Task<ActionResult> ImportTemplate(int id) // id is board id
        {
            Member member = await _context.Member.FirstOrDefaultAsync(m => m.Username == User.Identity.Name); // getting member
            if (member == null)
            {
                return NotFound();
            }

            var board = await _context.Board
               .Include(m => m.Project)
               .FirstOrDefaultAsync(m => m.BoardId == id);
            if (board == null)
            {
                return NotFound();
            }

            if (!MemberExists((int)board.ProjectId, member.MemberId)) // check non-authorized access
            {
                return NotFound();
            }

            if (!(ManagerCheck(board.Project, member.MemberId)))
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "You are not project manager on this project! You can't add template. Contact your project manager.";
                return View("Failed");
            }

            ViewData["BoardId"] = id;
            IEnumerable<Template> templates =  _context.Template.Include(l => l.Lists);

            return View(templates);
        }

        [HttpPost, ActionName("ImportTemplateLists")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportTemplateLists(int BoardId, [Bind("TemplateId")] Template template)
        {
            Member member = await _context.Member.FirstOrDefaultAsync(m => m.Username == User.Identity.Name); // getting member
            if (member == null)
            {
                return NotFound();
            }

            var projectId = await GetProjectId(BoardId);
            if (projectId == null) { 
                return NotFound();
            }

            if (!MemberExists((int) projectId, member.MemberId)) // check non-authorized access
            {
                return NotFound();
            }


            template = await _context.Template
             .Include(m => m.Lists)
             .FirstOrDefaultAsync(m => m.TemplateId == template.TemplateId);

            if (template == null)
            {
                return NotFound();
            }

            foreach(List list in template.Lists)
            {
                var createList = new List
                {
                    Title = list.Title,
                    BoardId = BoardId,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                await CreateList(createList);
            }

            await _context.SaveChangesAsync();
            
            if(projectId != null)
            {
                await UpdateProjectDate((int) projectId);
            }

            return RedirectToAction("Index", new { id=BoardId }); // return index

        }

        [Authorize(Roles = "admin")]
        public ActionResult AddTemplate() // async task
        {
            return View();
        } 

        [Authorize(Roles = "member")]
        public async Task<ActionResult> AddWork([Bind("Title")] Work Work, Microsoft.AspNetCore.Http.IFormCollection fc)
        {
            Work.ListId = Int16.Parse(fc["ListId"][0]); // Parse ListId string to int

            Member member = await _context.Member.FirstOrDefaultAsync(m => m.Username == User.Identity.Name); // getting member
            if (member == null)
            {
                return NotFound();
            }

            var list = await _context.List
              .Include(m => m.Board)
              .Include(m => m.Board.Project)
              .FirstOrDefaultAsync(m => m.ListId == Work.ListId);
            if (list == null)
            {
                return NotFound();
            }

            if (!MemberExists((int)list.Board.ProjectId, member.MemberId)) // check non-authorized access
            {
                return NotFound();
            }

            if (list.Board.Project.IsCancelled)
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "The project is cancelled. You can't create list";
                ViewBag.BoardId = list.BoardId;
                return View("Failed");
            }
            if (list.Board.Project.IsDeleted)
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "The project is cancelled. You can't create list";
                ViewBag.BoardId = list.BoardId;
                return View("Failed");
            }
            else if (list.Board.Project.IsFinished)
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "The project is finished. You can't create list";
                ViewBag.BoardId = list.BoardId;
                return View("Failed");
            }

            if (ModelState.IsValid)
            {
                var dt = DateTime.Now;
                _context.Add(new Work
                {
                    Title = Work.Title,
                    CreatedDate = dt,
                    UpdatedDate = dt,
                    Priority = 0,
                    Status = 0,
                    ListId = Work.ListId,       
            }
                );
                list.Board.UpdatedDate = DateTime.Now;
                _context.Update(list);
                await _context.SaveChangesAsync();
                var work = await _context.Work.FirstOrDefaultAsync(w => w.ListId == list.ListId && w.CreatedDate == dt && w.Title == Work.Title); // Concurrency ?????
                var workmember = new WorkMember { WorkId = work.WorkId , MemberId = member.MemberId };
                _context.Add(workmember);
                await UpdateProjectDate((int)list.Board.ProjectId);
                return RedirectToAction("Index", new { id = list.BoardId }); // return index
            }

            return RedirectToAction("Index", new { id = list.Board.BoardId });
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


        private async Task<int?> GetProjectId(int boardId)
        {
            var board = await _context.Board
                       .Include(m => m.Project)
                       .FirstOrDefaultAsync(m => m.BoardId == boardId);
            if (board == null)
            {
                return null;
            }

            return board.ProjectId;
        }
    }
}

    
