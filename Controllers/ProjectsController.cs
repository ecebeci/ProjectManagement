using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data;
using ProjectManagement.Models;
using ProjectManagement.Models.ViewModels;

namespace ProjectManagement.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ProjectsContext _context;
       // private readonly UserManager<IdentityUser> _userManager;

        public ProjectsController(ProjectsContext context) //UserManager<IdentityUser> userManager
        {
            _context = context;
         //   _userManager = userManager;

        }

        // GET: Projects
        [Authorize(Roles = "member")]
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            Member member = await _context.Member.FirstOrDefaultAsync(m => m.Username == User.Identity.Name); // getting member
            if (member == null)
            {
                return NotFound();
            }

            var memberProjects = await _context.ProjectMember
             .Where(p => p.MemberId == member.MemberId)
             .Where(p => p.Project.IsDeleted == false) // hide logically deleted project
             .Include(p => p.Member) 
             .Include(p => p.Project.Manager) // include manager (many - (to) - many)
             .ToListAsync();

            // logically deleted project which he/she is the "project manager"
            var ProjectsManagerofDeleted = await _context.ProjectMember
                 .Where(p => p.MemberId == member.MemberId)
                 .Include(p => p.Member)
                 .Include(p => p.Project)
                 .Include(p => p.Project.Manager)
                 .Where(p => p.Project.IsDeleted == true) // select deleted projects
                 .Where(p => p.Project.ManagerId == member.MemberId) // select manager of deleted projects
                 .ToListAsync();

            memberProjects.AddRange(ProjectsManagerofDeleted); // append list to end

            if (memberProjects.Count == 0) 
            {
                return View("Create");
            }

            var memberProjectsSearched = memberProjects.Where(p => name == null || p.Project.Name.Contains(name)); // search

            var pagingInfo = new PagingInfo
            {
                CurrentPage = page,
                TotalItems = memberProjectsSearched.Count()
            };

            if (pagingInfo.CurrentPage > pagingInfo.TotalPages)
            {
                pagingInfo.CurrentPage = pagingInfo.TotalPages;
            }

            if (pagingInfo.CurrentPage < 1)
            {
                pagingInfo.CurrentPage = 1;
            }

            var memberProjectsSearchedPaginated = memberProjectsSearched
                            .Skip((pagingInfo.CurrentPage - 1) * pagingInfo.PageSize)
                            .Take(pagingInfo.PageSize);

            return View( new ProjectsListViewModel
            {
                ProjectMember = memberProjectsSearchedPaginated,
                PagingInfo = pagingInfo,
                NameSearched = name
            });
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "member")]
        public async Task<IActionResult> Details(int? id)
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

            var project = await _context.Project
                .FirstOrDefaultAsync(p => p.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            var ProjectMembers = await _context.ProjectMember
                .Where(x => x.Project == project)
                .Include(u => u.Member)
                .Include(u => u.Project.Manager)
                .ToListAsync();

            var CheckIsUserinProject = ProjectMembers.Where(p => p.MemberId == member.MemberId); // Checking Non-Authorized Access with Member id matches
            if (!CheckIsUserinProject.Any()) // Checking CheckIsUserinProject has 1 entry
            {
                return NotFound();
            }

            ProjectProjectMembers ProjectProjectMembers = new () // Setting View Model
            {
                Project = project,
                ProjectMembers = ProjectMembers
            };

            return View(ProjectProjectMembers);
        }

        // GET: Projects/Create
        [Authorize(Roles = "member")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "member")]
        public async Task<IActionResult> Create([Bind("ProjectId,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                Member member = await _context.Member.FirstOrDefaultAsync(m => m.Username == User.Identity.Name) ;
                if(member == null)
                {
                    return NotFound();
                }

                project.ManagerId = member.MemberId;
                project.CreatedDate = DateTime.Now;
                project.UpdatedDate = DateTime.Now; // creating also means the first update
                _context.Add(project);

                try
                {
                    await _context.SaveChangesAsync(); // save, before to use new id's
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ViewBag.Title = "Error";
                    ViewBag.Message = ex.HResult;
                    return View("Failed");
                }

                // find new project's id (it is important!)
                Project projectAdded = await _context.Project
                                        .OrderByDescending(p=> p.ProjectId) // Order by descending! Taking to last one!
                                        .FirstOrDefaultAsync(m => m.Name == project.Name &&  m.ManagerId == member.MemberId); // last one
                                   
                if (projectAdded == null)
                {
                    return NotFound();
                }

               
                _context.Add(new ProjectMember { 
                    MemberId = member.MemberId , 
                    ProjectId = projectAdded.ProjectId // you can't use first "project.ProjectId" because id doesnt know (before saving db) that will give error
                }); // add person to many to many
               
                try
                {
                await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex) 
                {
                    ViewBag.Title = "Error";
                    ViewBag.Message = ex.HResult;
                    return View("Failed");
                }

                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "member")]
        public async Task<IActionResult> Edit(int? id)
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

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            if(project.ManagerId != member.MemberId) // Check non-authorized access. If the user is not project member what thet selected, they cant edit.
            {
                ViewBag.Title = "Access Denied";
                ViewBag.Message = "You are not Project Manager! You can't access edit page on this project.";
                return View("Failed");
            }

            var ProjectMembers = await _context.ProjectMember
                .Where(x => x.Project == project)
                .Include(u => u.Member)
                .Include(u => u.Project.Manager).ToListAsync();

            ProjectProjectMembers ProjectProjectMembers = new () // Setting View Model
            {
                Project = project,
                ProjectMembers = ProjectMembers
            };

            return View(ProjectProjectMembers);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "member")]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,Name,IsCancelled")] Project project)
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            var projectFound = _context.Project
                              .Where(x => x.ProjectId == project.ProjectId)
                              .FirstOrDefault();

            projectFound.Name = project.Name; // Change name
            projectFound.IsCancelled = project.IsCancelled;
            projectFound.UpdatedDate = DateTime.Now;
            try
                {    
                    _context.Project.Update(projectFound);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!ProjectExists(project.ProjectId))
                    {
                    ViewBag.Title = "Error";
                    ViewBag.Message = "Project is not exist!";
                    return View("Failed"); 
                    }
                    else
                    {
                        ViewBag.Title = "Unexpected Error";
                        ViewBag.Message = ex.HResult;
                        return View("Failed");
                }
                }

                return RedirectToAction("Edit", new { id = id }); // Return edit page

        }

    
        // Adding Member to Project
        [HttpPost]
        [HttpPost, ActionName("AddMemberProject")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "member")]
        public async Task<IActionResult> AddMemberProject([Bind("Username")] Member member, [Bind("ProjectId")] Project project)
        {
            if (member.Username == null)
            {
                ViewBag.Title = "Error";
                ViewBag.Message = "You have entered invalid username. Please check the name!";
                return View("Failed");
            }

            member = await _context.Member
                         .Where(x=> x.Username == member.Username)
                         .FirstOrDefaultAsync();

            if (member == null)
            {
                ViewBag.Title = "Error";
                ViewBag.Message = "The username which you entered is not registered! Please check the name again!";
                return View("Failed");
            }

            var ProjectMemberCheck = await _context.ProjectMember
                                        .Include(x => x.Member)
                                        .Where(x => x.ProjectId == project.ProjectId)
                                        .Where(x => x.Member.Username == member.Username)
                                        .ToListAsync();

            if (ProjectMemberCheck.Any())
            {
                ViewBag.Title = "Error";
                ViewBag.Message = "You entered a person who already exists in the project. Please check the name!";
                return View("Failed");
            }


            // Find Project
            project = _context.Project
                                .Where(x => x.ProjectId == project.ProjectId)
                                .FirstOrDefault();

            if (project == null)
            {
                ViewBag.Title = "Error";
                ViewBag.Message = "The project can't found! It may have been deleted.";
                return View("Failed");
            }
            
            // Update Time
            project.UpdatedDate = DateTime.Now;

            try
            {
                    _context.ProjectMember.Add(new ProjectMember
                    {
                        ProjectId = project.ProjectId,
                        MemberId = member.MemberId
                    });

                    _context.Project.Update(project);

                    await _context.SaveChangesAsync();
                }
            catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExist(project.ProjectId, member.MemberId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
             }


            return RedirectToAction("Edit", new { id = project.ProjectId });
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            if (project.IsDeleted)
            {
                return RedirectToAction("Revive", project); // Redirects Revive View
            }

            Member member = await _context.Member
                            .FirstOrDefaultAsync(m => m.Username == User.Identity.Name);
            if (member == null)
            {
                return NotFound();
            }

            if (member.MemberId != project.ManagerId) // Checking Non-Authorized Access 
            {
                return NotFound();
            }

            var ProjectMembers = await _context.ProjectMember
                .Where(x => x.Project == project)
                .Include(u => u.Member)
                .Include(u => u.Project.Manager)
                .ToListAsync();

            ProjectProjectMembers ProjectProjectMembers = new ()
            {
                Project = project,
                ProjectMembers = ProjectMembers
            };

            return View(ProjectProjectMembers);
        }

        // POST: Projects/Delete/DeleteMember
        [HttpPost, ActionName("DeleteMember")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "member")]
        public async Task<IActionResult> DeleteMemberOnProjectConfirmed([Bind("ProjectId, MemberId")]  ProjectMember pm)
        {
            // adding Project and Member objects 
            var ProjectMember = await _context.ProjectMember 
               .Where(x => x.ProjectId == pm.ProjectId)
               .Where(x => x.MemberId == pm.MemberId)
               .Include(u => u.Member)
               .Include(u => u.Project)
               .Include(u => u.Project.Manager)    
               .FirstOrDefaultAsync();

            if (ProjectMember == null)
            {
                ViewBag.Title = "Error";
                ViewBag.Message = "Project couldn't find.";
                return View("Failed");
            }

            if(ProjectMember.Project.ManagerId == ProjectMember.Member.MemberId)
            {
                ViewBag.Title = "Error";
                ViewBag.Message = "You can't discard yourself on Project. To delete project go to Delete Project";
                return View("Failed");
            }


            // Update Time
            ProjectMember.Project.UpdatedDate = DateTime.Now;

            try
            {
                _context.ProjectMember.Remove(ProjectMember);
                _context.Project.Update(ProjectMember.Project); // Update Project.UpdateDate 
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProjectExists(ProjectMember.Project.ProjectId))
                {
                    ViewBag.Title = "Error";
                    ViewBag.Message = "Project is not exist!";
                    return View("Failed");
                }
                else
                {
                    ViewBag.Title = "Unexpected Error";
                    ViewBag.Message = ex.HResult;
                    return View("Failed");
                }
            }


                return RedirectToAction("Edit", new { id = pm.ProjectId });
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "member")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var project = await _context.Project.FindAsync(id);

            project.IsDeleted = true; // logically deleting
            project.UpdatedDate = DateTime.Now;

            try
            {
                _context.Project.Update(project);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ViewBag.Title = "Error";
                ViewBag.Message = ex.HResult;
                return View("Failed");
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Projects/Revive/5
        public async Task<IActionResult> Revive(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            if (!project.IsDeleted)
            {
                return RedirectToAction("Delete", id); // Redirects Delete View
            }

            Member member = await _context.Member
                            .FirstOrDefaultAsync(m => m.Username == User.Identity.Name);
            if (member == null)
            {
                return NotFound();
            }

            if (member.MemberId != project.ManagerId) // Checking Non-Authorized Access 
            {
                return NotFound();
            }

            var ProjectMembers = await _context.ProjectMember
                .Where(x => x.Project == project)
                .Include(u => u.Member)
                .Include(u => u.Project.Manager)
                .ToListAsync();

            ProjectProjectMembers ProjectProjectMembers = new()
            {
                Project = project,
                ProjectMembers = ProjectMembers
            };

            return View(ProjectProjectMembers);
        }

        // POST: Projects/Revive/5
        [HttpPost, ActionName("Revive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "member")]
        public async Task<IActionResult> ReviveConfirmed(int id)
        {

            var project = await _context.Project.FindAsync(id);

            project.IsDeleted = false;
            project.UpdatedDate = DateTime.Now;

            try
            {
                _context.Project.Update(project);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ViewBag.Title = "Error";
                ViewBag.Message = ex.HResult;
                return View("Failed");
            }
            return RedirectToAction(nameof(Index));
        }


        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.ProjectId == id);
        }

        private bool MemberExist(int ProjectId,int MemberId)
            {
                return _context.ProjectMember.Any(e => e.ProjectId == ProjectId && e.MemberId == MemberId);
        }
        }
}
