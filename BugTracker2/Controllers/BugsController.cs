using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker2.Areas.Identity.Data;
using BugTracker2.Models;

namespace BugTracker2.Controllers
{
    public class BugsController : Controller
    {
        private readonly BugTracker2Context _context;

        public BugsController(BugTracker2Context context)
        {
            _context = context;
        }

        // GET: Bugs
        public async Task<IActionResult> Index()
        {
            var bugTracker2Context = _context.Bugs.Include(b => b.Severity).Include(b => b.Status).Include(b => b.UserAssigned).Include(b => b.UserReported);
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
                .FirstOrDefaultAsync(m => m.BugId == id);
            if (bug == null)
            {
                return NotFound();
            }

            return View(bug);
        }

        // GET: Bugs/Create
        public IActionResult Create()
        {
            ViewData["SeverityId"] = new SelectList(_context.Severity, "SeverityId", "SeverityId");
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "StatusId");
            ViewData["UserAssignedId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["UserReportedId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Bugs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BugId,BugName,ReportDate,SeverityId,Behavior,StepsDescription,UserReportedId,UserAssignedId,StatusId")] Bug bug)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bug);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SeverityId"] = new SelectList(_context.Severity, "SeverityId", "SeverityId", bug.SeverityId);
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "StatusId", bug.StatusId);
            ViewData["UserAssignedId"] = new SelectList(_context.Users, "Id", "Id", bug.UserAssignedId);
            ViewData["UserReportedId"] = new SelectList(_context.Users, "Id", "Id", bug.UserReportedId);
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
            ViewData["SeverityId"] = new SelectList(_context.Severity, "SeverityId", "SeverityId", bug.SeverityId);
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "StatusId", bug.StatusId);
            ViewData["UserAssignedId"] = new SelectList(_context.Users, "Id", "Id", bug.UserAssignedId);
            ViewData["UserReportedId"] = new SelectList(_context.Users, "Id", "Id", bug.UserReportedId);
            return View(bug);
        }

        // POST: Bugs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BugId,BugName,ReportDate,SeverityId,Behavior,StepsDescription,UserReportedId,UserAssignedId,StatusId")] Bug bug)
        {
            if (id != bug.BugId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["SeverityId"] = new SelectList(_context.Severity, "SeverityId", "SeverityId", bug.SeverityId);
            ViewData["StatusId"] = new SelectList(_context.Status, "StatusId", "StatusId", bug.StatusId);
            ViewData["UserAssignedId"] = new SelectList(_context.Users, "Id", "Id", bug.UserAssignedId);
            ViewData["UserReportedId"] = new SelectList(_context.Users, "Id", "Id", bug.UserReportedId);
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
            _context.Bugs.Remove(bug);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BugExists(int id)
        {
            return _context.Bugs.Any(e => e.BugId == id);
        }
    }
}
