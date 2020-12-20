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
    public class UserProjectInfosController : Controller
    {
        private const int USERADMIN = 3;
        private const string UNASSIGNED = "--UnAssigned--";
        private const string MASTER = "Master";
        private readonly BugTracker2Context _context;
        private string userId;

        public UserProjectInfosController(BugTracker2Context context)
        {
            _context = context;

            // Get the User Id
            var httpContextAccessor = new HttpContextAccessor();
            userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        // GET: UserProjectInfos
        public async Task<IActionResult> Index()
        {
            var bugTracker2Context = _context.UserProjectInfo
                .Include(u => u.Privilege)
                .Include(u => u.Project)
                .Include(u => u.User)
                .OrderBy(u => u.User.FirstName);

            return View(await bugTracker2Context.ToListAsync());
        }

        // GET: UserProjectInfos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProjectInfo = await _context.UserProjectInfo
                .Include(u => u.Privilege)
                .Include(u => u.Project)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserProjectInfoId == id);

            var projectId = _context.UserProjectInfo.FirstOrDefault(u => u.UserProjectInfoId == id).ProjectId;
            if (userProjectInfo == null || !ReadEnabled(projectId))
            {
                return NotFound();
            }

            return View(userProjectInfo);
        }

        // GET: UserProjectInfos/Create
        public IActionResult Create(int id)
        {
            // To Do: Need to add privilege and user validation
            var project = _context.Project.FirstOrDefault(p => p.ProjectId == id);
            ViewData["ProjectName"] = project.ProjectName;
            ViewData["PrivilegeId"] = GetPrivilegeSelectList();

            var userProjectInfo = new UserProjectInfo() { ProjectId = id };

            return View(userProjectInfo);
        }

        // POST: UserProjectInfos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserProjectInfoId,UserId,ProjectId,PrivilegeId")] UserProjectInfo userProjectInfo)
        {
            userProjectInfo.UserId = GetUserId(userProjectInfo.UserId, userProjectInfo.ProjectId);
            if (string.IsNullOrEmpty(userProjectInfo.UserId))
            {
                ModelState.AddModelError("", "Error: You must enter a unique and valid user email!");
            }
            else if (!IsUserAdmin(userProjectInfo.ProjectId))
            {
                ModelState.AddModelError("", "You have insufficient privileges to create this item!");
            }
            else if (ModelState.IsValid)
            {
                _context.Add(userProjectInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ProjectsController.Details), "Projects", new { id = userProjectInfo.ProjectId });
            }

            var project = _context.Project.FirstOrDefault(p => p.ProjectId == userProjectInfo.ProjectId);
            userProjectInfo.Project = project;
            ViewData["ProjectName"] = project.ProjectName;
            ViewData["PrivilegeId"] = GetPrivilegeSelectList();

            return View(userProjectInfo);
        }

        // GET: UserProjectInfos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProjectInfo = _context.UserProjectInfo.FirstOrDefault(u => u.UserProjectInfoId == id);
            var privilegeName = _context.Privilege.FirstOrDefault(p => p.PrivilegeId == userProjectInfo.PrivilegeId).PrivilegeName;

            if (userProjectInfo == null || privilegeName == MASTER)
            {
                return NotFound();
            }

            var project = _context.Project.FirstOrDefault(p => p.ProjectId == userProjectInfo.ProjectId);
            userProjectInfo.User = await _context.Users.FindAsync(userProjectInfo.UserId);
            userProjectInfo.UserId = userProjectInfo.User.Id;
            ViewData["ProjectName"] = project.ProjectName;
            ViewData["PrivilegeId"] = GetPrivilegeSelectList();

            return View(userProjectInfo);
        }

        // POST: UserProjectInfos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("UserProjectInfoId,UserId,ProjectId,PrivilegeId")] UserProjectInfo userProjectInfo)
        {
            var privilegeName = _context.Privilege.FirstOrDefault(p => p.PrivilegeId == userProjectInfo.PrivilegeId).PrivilegeName;
            if (!IsUserAdmin(userProjectInfo.ProjectId))
            {
                ModelState.AddModelError("", "You have insufficient privileges to edit this item!");
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userProjectInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserProjectInfoExists(userProjectInfo.UserProjectInfoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(ProjectsController.Details), "Projects", new { id = userProjectInfo.ProjectId });
            }
            else if (privilegeName == MASTER)
            {
                ModelState.AddModelError("", "You cannot edit a master privilege");
            }

            var project = _context.Project.FirstOrDefault(p => p.ProjectId == userProjectInfo.ProjectId);
            userProjectInfo.User = await _context.Users.FindAsync(userProjectInfo.UserId);
            userProjectInfo.UserId = userProjectInfo.User.Id;
            ViewData["ProjectName"] = project.ProjectName;
            ViewData["PrivilegeId"] = GetPrivilegeSelectList();

            return View(userProjectInfo);
        }

        // GET: UserProjectInfos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProjectInfo = await _context.UserProjectInfo
                .Include(u => u.Privilege)
                .Include(u => u.Project)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserProjectInfoId == id);

            if (userProjectInfo == null || userProjectInfo.Privilege.PrivilegeName == MASTER)
            {
                return NotFound();
            }

            return View(userProjectInfo);
        }

        // POST: UserProjectInfos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userProjectInfo = await _context.UserProjectInfo.FindAsync(id);
            var privilegeName = _context.Privilege.FirstOrDefault(p => p.PrivilegeId == userProjectInfo.PrivilegeId).PrivilegeName;

            if (privilegeName == MASTER)
            {
                return NotFound("You cannot delete a master privilege!");
            }
            else if (!IsUserAdmin(userProjectInfo.ProjectId))
            {
                return NotFound("You have insufficient privileges to delete this item!");
            }
            else
            {
                _context.UserProjectInfo.Remove(userProjectInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ProjectsController.Details), "Projects", new { id = userProjectInfo.ProjectId });
            }

            return View(userProjectInfo);
        }

        private bool UserProjectInfoExists(int id)
        {
            return _context.UserProjectInfo.Any(e => e.UserProjectInfoId == id);
        }

        private string GetUserId(string email, int projectId)
        {
            email = email?.ToUpper();
            // Get a user with the matching email
            var user = _context.Users.FirstOrDefault(u => u.FirstName != UNASSIGNED
                                          && !(u.UserProjectInfos.Any(p => p.ProjectId == projectId))
                                          && u.NormalizedEmail == email);
            return user?.Id;
        }

        private SelectList GetPrivilegeSelectList()
        {
            var items = _context.Privilege
                .Where(p => p.PrivilegeName != MASTER)
                .OrderBy(p => p.PrivilegeLevel);

            return new SelectList(items, "PrivilegeId", "PrivilegeDisplay");
        }

        private bool ReadEnabled(int projectId)
        {
            return _context.UserProjectInfo.Any(ug => ug.ProjectId == projectId
                                            && ug.UserId == userId);
        }

        private bool IsUserAdmin(int projectId)
        {
            return _context.UserProjectInfo.Any(p => p.ProjectId == projectId
                                        && p.UserId == userId
                                        && p.Privilege.PrivilegeLevel >= USERADMIN);
        }
    }
}
