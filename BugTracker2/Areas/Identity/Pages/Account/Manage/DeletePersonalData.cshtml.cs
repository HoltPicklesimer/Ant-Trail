using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BugTracker2.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using BugTracker2.Models;
using BugTracker2.Areas.Identity.Data;
using System.Linq;

namespace BugTracker2.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;
        private BugTracker2Context _context;

        public DeletePersonalDataModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<DeletePersonalDataModel> logger,
            BugTracker2Context context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Incorrect password.");
                    return Page();
                }
            }

            // Remove the user from all dependent projects before deleting
            var userProjects = _context.UserProjectInfo.Where(p => p.UserId == user.Id);
            _context.RemoveRange(userProjects);

            // Remove the user from all bugs where they are referenced
            var assignedBugs = _context.Bugs.Where(b => b.UserAssignedId == user.Id);
            foreach (var bug in assignedBugs)
                bug.UserAssignedId = _context.Users.FirstOrDefault(u => u.FirstName == "--UnAssigned--").Id;

            var reportedBugs = _context.Bugs.Where(b => b.UserAssignedId == user.Id);
            foreach (var bug in reportedBugs)
                bug.UserReportedId = _context.Users.FirstOrDefault(u => u.FirstName == "--UnAssigned--").Id;

            _context.SaveChanges();

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }
    }
}
