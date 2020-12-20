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
    public class BugsController : Controller
    {
        const int CRUDPRIVILEGE = 2;
        const string UNASSIGNED = "--UnAssigned--";
        private readonly BugTracker2Context _context;
        private string userId;

        public BugsController(BugTracker2Context context)
        {
            _context = context;

            // Get the User Id
            var httpContextAccessor = new HttpContextAccessor();
            userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        // GET: Bugs
        public async Task<IActionResult> Index()
        {
            var bugTracker2Context = _context.Bugs
                .Include(b => b.Severity)
                .Include(b => b.Status)
                .Include(b => b.UserAssigned)
                .Include(b => b.UserReported)
                .OrderBy(b => b.Severity.Priority)
                .ThenBy(b => b.ReportDate);

            return View(await bugTracker2Context.ToListAsync());
        }

        // GET: Bugs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bug = await _context.Bugs
                .Include(b => b.Severity)
                .Include(b => b.Status)
                .Include(b => b.UserAssigned)
                .Include(b => b.UserReported)
                .Include(b => b.Project)
                .FirstOrDefaultAsync(m => m.BugId == id);

            var projectId = _context.Bugs.FirstOrDefault(b => b.BugId == id).ProjectId;
            if (bug == null || !ReadEnabled(projectId))
            {
                return NotFound();
            }

            return View(bug);
        }

        // GET: Bugs/Create
        public IActionResult Create(int id)
        {
            // To Do: Need to add privilege and user validation
            var project = _context.Project.FirstOrDefault(p => p.ProjectId == id);

            ViewData["ProjectName"] = project.ProjectName;
            ViewData["SeverityId"] = new SelectList(_context.Severity.OrderByDescending(s => s.Priority), "SeverityId", "SeverityDisplay");
            ViewData["StatusId"] = new SelectList(_context.Status.OrderBy(s => s.Step), "StatusId", "StatusDisplay");
            ViewData["UserAssignedId"] = GetUserSelect(id);
            ViewData["UserReportedId"] = GetUserSelect(id);

            var bug = new Bug() { ProjectId = id };

            return View(bug);
        }

        // POST: Bugs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectId,BugId,BugName,SeverityId,Behavior,StepsDescription,UserReportedId,UserAssignedId")] Bug bug)
        {
            var status = (bug.UserAssignedId == _context.Users.FirstOrDefault(u => u.FirstName == UNASSIGNED).Id) ? "New" : "Assigned";
            bug.Status = _context.Status.FirstOrDefault(s => s.StatusName == status);
            bug.ReportDate = DateTime.Now;

            if (!CRUDEnabled(bug.ProjectId))
            {
                ModelState.AddModelError("", "You have insufficient privileges to create this item!");
            }
            else if (ModelState.IsValid)
            {
                _context.Add(bug);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ProjectsController.Details), "Projects", new { id = bug.ProjectId });
            }

            var project = _context.Project.FirstOrDefault(p => p.ProjectId == bug.ProjectId);

            ViewData["ProjectName"] = project.ProjectName;
            ViewData["SeverityId"] = new SelectList(_context.Severity.OrderByDescending(s => s.Priority), "SeverityId", "SeverityDisplay");
            ViewData["StatusId"] = new SelectList(_context.Status.OrderBy(s => s.Step), "StatusId", "StatusDisplay");
            ViewData["UserAssignedId"] = GetUserSelect(bug.ProjectId);
            ViewData["UserReportedId"] = GetUserSelect(bug.ProjectId);

            return View(bug);
        }

        // GET: Bugs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bug = await _context.Bugs.FindAsync(id);
            if (bug == null)
            {
                return NotFound();
            }

            var project = _context.Project.FirstOrDefault(p => p.ProjectId == bug.ProjectId);

            ViewData["ProjectName"] = project.ProjectName;
            ViewData["SeverityId"] = new SelectList(_context.Severity.OrderByDescending(s => s.Priority), "SeverityId", "SeverityDisplay");
            ViewData["StatusId"] = new SelectList(_context.Status.OrderBy(s => s.Step), "StatusId", "StatusDisplay");
            ViewData["UserAssignedId"] = GetUserSelect(bug.ProjectId);
            ViewData["UserReportedId"] = GetUserSelect(bug.ProjectId);

            return View(bug);
        }

        // POST: Bugs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("BugId,BugName,ReportDate,SeverityId,Behavior,StepsDescription,UserReportedId,UserAssignedId,StatusId,ProjectId")] Bug bug)
        {
            if (!CRUDEnabled(bug.ProjectId))
            {
                ModelState.AddModelError("", "You have insufficient privileges to edit this item!");
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bug);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BugExists(bug.BugId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(ProjectsController.Details), "Projects", new { id = bug.ProjectId });
            }

            var project = _context.Project.FirstOrDefault(p => p.ProjectId == bug.ProjectId);

            ViewData["ProjectName"] = project.ProjectName;
            ViewData["SeverityId"] = new SelectList(_context.Severity.OrderByDescending(s => s.Priority), "SeverityId", "SeverityDisplay");
            ViewData["StatusId"] = new SelectList(_context.Status.OrderBy(s => s.Step), "StatusId", "StatusDisplay");
            ViewData["UserAssignedId"] = GetUserSelect(bug.ProjectId);
            ViewData["UserReportedId"] = GetUserSelect(bug.ProjectId);

            return View(bug);
        }

        // GET: Bugs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bug = await _context.Bugs
                .Include(b => b.Severity)
                .Include(b => b.Status)
                .Include(b => b.UserAssigned)
                .Include(b => b.UserReported)
                .Include(b => b.Project)
                .FirstOrDefaultAsync(m => m.BugId == id);

            if (bug == null)
            {
                return NotFound();
            }

            return View(bug);
        }

        // POST: Bugs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bug = await _context.Bugs.FindAsync(id);

            if (!CRUDEnabled(bug.ProjectId))
            {
                return NotFound("Error: You have insufficient privileges to delete this item!");
            }
            else
            {
                _context.Bugs.Remove(bug);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ProjectsController.Details), "Projects", new { id = bug.ProjectId });
            }
        }

        private bool BugExists(int id)
        {
            return _context.Bugs.Any(e => e.BugId == id);
        }

        private SelectList GetUserSelect(int projectId)
        {
            var users = _context.Users
                .Where(u => u.UserProjectInfos.Any(p => p.ProjectId == projectId))
                .OrderBy(s => s.FirstName);
            var items = users.ToList();
            items.Insert(0, _context.Users.FirstOrDefault(u => u.FirstName == UNASSIGNED));
            var userSelect = new SelectList(items, "Id", "FullName");
            return userSelect;
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
