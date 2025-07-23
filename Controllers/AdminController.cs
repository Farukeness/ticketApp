using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ticketApp.Models;
using ticketApp.ViewModel;

namespace ticketApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private RoleManager<AppRole> _roleManager;

        public AdminController(UserManager<AppUser> userManager,
         SignInManager<AppUser> signInManager,
         RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View(_userManager.Users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var userRole = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            var model = new AdminEditModel
            {
                UserId = user.Id,
                Email = user.Email,
                CurrentRole = userRole.FirstOrDefault(),
                Roles = allRoles
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdminEditModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UserId != null){var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null) { return NotFound(); }
                var currentRole = await _userManager.GetRolesAsync(user);
                if (currentRole.Any())
                    await _userManager.RemoveFromRolesAsync(user, currentRole);
                if (model.SelectedRole != null)
                await _userManager.AddToRoleAsync(user, model.SelectedRole);
                Console.WriteLine("Gelen rol: " + model.SelectedRole);
                return RedirectToAction("Index");}
                
            }
            return View(model);
            
        }
    }
}