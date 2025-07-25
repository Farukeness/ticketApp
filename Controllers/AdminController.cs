using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ticketApp.Data;
using ticketApp.Enums;
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

        private readonly ApplicationDbContext _applicationDbContext;


        public AdminController(UserManager<AppUser> userManager,
         SignInManager<AppUser> signInManager,
         RoleManager<AppRole> roleManager,
         ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View(_userManager.Users);
        }
        public async Task<IActionResult> Tickets()
        {
            var devName = "Developer";
            var devList = await _userManager.GetUsersInRoleAsync(devName);
            var model = new TicketListViewModel
            {
                Tickets = _applicationDbContext.Tickets.ToList(),
                Developers = devList.ToList()
            };

            return View(model);
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
                if (model.UserId != null)
                {
                    var user = await _userManager.FindByIdAsync(model.UserId);
                    if (user == null) { return NotFound(); }
                    var currentRole = await _userManager.GetRolesAsync(user);
                    if (currentRole.Any())
                        await _userManager.RemoveFromRolesAsync(user, currentRole);
                    if (model.SelectedRole != null)
                        await _userManager.AddToRoleAsync(user, model.SelectedRole);
                    return RedirectToAction("Index");
                }

            }
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> EditAssigned(Tickets tickets)
        
        {
            var existingTicket = await _applicationDbContext.Tickets.FindAsync(tickets.Id);
            if(existingTicket == null){ return NotFound(); }
            existingTicket.AssignedToUserId = tickets.AssignedToUserId;
            existingTicket.ticketStatus = tickets.ticketStatus;
            //_applicationDbContext.Tickets.Update(existingTicket);
            if (_applicationDbContext == null){ return NotFound(); }
            await   _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("Tickets");
        }
    }
}