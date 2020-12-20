using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker2.Areas.Identity.Data;
using BugTracker2.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BugTracker2.Controllers
{
    public class ProjectsController : Controller
    {
        private const int CRUDPRIVILEGE = 2;
        private readonly BugTracker2Context _context;
        private string userId;

        public ProjectsController(BugTracker2Context context)
        {
            _context = context;

            // Get the User Id
            var httpContextAccessor = new HttpContextAccessor();
            userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        // GET: Projects
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentSort, string currentFilter)
        {
            // Get the projects a user is a part of
            var projects = _context.Project
                .Include(p => p.UserProjectInfos)
                .ThenInclude(up => up.User)
                .Where(g => g.UserProjectInfos.Any(u => u.UserId == userId));

            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(p => p.ProjectName.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(sortOrder))
            {
                if (sortOrder == "asc")
                    projects = projects.OrderBy(p => p.ProjectName);
                else
                    projects = projects.OrderByDescending(p => p.ProjectName);
            }

            return View(await projects.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the project, user, admin, and privilege info
            var project = await _context.Project
                .Include(p => p.UserProjectInfos)
                .ThenInclude(up => up.User)
                .Include(p => p.UserProjectInfos)
                .ThenInclude(up => up.Privilege)
                .Include(p => p.Bugs)
                .ThenInclude(b => b.Severity)
                .Include(p => p.Bugs)
                .ThenInclude(b => b.Status)
                .Include(p => p.Bugs)
                .ThenInclude(b => b.UserAssigned)
                .Include(p => p.Bugs)
                .ThenInclude(b => b.UserReported)
                .FirstOrDefaultAsync(m => m.ProjectId == id);

            // Check to make sure the user has read privileges
            if (project == null || !ReadEnabled(project.ProjectId))
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectId,ProjectName,ProjectDescription")] Project project)
        {
            if (ModelState.IsValid)
            {
                // Add the project
                _context.Add(project);

                await _context.SaveChangesAsync();

                // Add the user to the project with the master privilege level
                var userProjectInfo = new UserProjectInfo()
                {
                    ProjectId = project.ProjectId,
                    UserId = userId,
                    PrivilegeId = _context.Privilege.Where(p => p.PrivilegeLevel == 4)
                        .FirstOrDefault()?.PrivilegeId ?? 0
                };

                _context.Add(userProjectInfo);

                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.UserProjectInfos)
                .FirstOrDefaultAsync(m => m.ProjectId == id);

            // Check to make sure the user has CRUD privileges
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectId,ProjectName,ProjectDescription")] Project project)
        {
            // Verify the user has CRUD privileges
            if (!CRUDEnabled(id))
            {
                ModelState.AddModelError("", "You do not have sufficient privileges to edit this project!");
                return View(project);
            }
            else if (id != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ProjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.UserProjectInfos)
                .FirstOrDefaultAsync(m => m.ProjectId == id);

            // Verify the user has CRUD privileges
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Project.FindAsync(id);

            // Check to make sure the user has CRUD privileges
            if (!CRUDEnabled(id))
            {
                ModelState.AddModelError("", "You do not have sufficient privileges to edit this project!");
                return View(project);
            }
            else
            {
                _context.Project.Remove(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.ProjectId == id);
        }

        private bool ReadEnabled(int projectId)
        {
            return _context.UserProjectInfo.Any(ug => ug.ProjectId == projectId
                                            && ug.UserId == userId);
        }

        private bool CRUDEnabled(int projectId)
        {
            return _context.UserProjectInfo.Any(p => p.ProjectId == projectId
                                        && p.UserId == userId
                                        && p.Privilege.PrivilegeLevel >= CRUDPRIVILEGE);
        }
    }
}
