using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
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
            
            return View();
        }
        public async Task<IActionResult> TicketEdit(int Id)
        {
            try
            {
                var devName = "Developer";
                var devList = await _userManager.GetUsersInRoleAsync(devName);
                var ticket = await _applicationDbContext.Tickets
                    .Include(t => t.AssignedToUsers)
                    .FirstOrDefaultAsync(t => t.Id == Id);

                if (ticket == null)
                {
                    return Json(new { success = false, message = "Ticket bulunamadı" });
                }

                var developersWithStatus = devList.Select(dev => new
                {
                    id = dev.Id,
                    UserName = dev.UserName,
                    isAssigned = ticket.AssignedToUsers.Any(assigned => assigned.Id == dev.Id)
                }).ToList();

                return Json(new { 
                    success = true, 
                    developers = developersWithStatus 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Statistic()
        {

            var _total = _applicationDbContext.Tickets.Count();
            float _open= _applicationDbContext.Tickets.Where(t => t.ticketStatus == TicketStatus.Acik).Count();
            float _close = _applicationDbContext.Tickets.Where(t => t.ticketStatus == TicketStatus.Kapatildi).Count();
            var model = new StatusChartView
            {
                TotalTicket = _total,
                OpenTicket = _open,
                CloseTicket = _close
            };



            return View(model);
        }
        
        public IActionResult Projects()
        {
            return View();
        }

        public IActionResult Tickets()
        {
            return View();
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
        public async Task<IActionResult> EditAssigned(Tickets tickets, 
        [FromForm(Name = "AssignedToUsers")] List<string> selectedDeveloperIds)
        
        {
            var existingTicket =  _applicationDbContext.Tickets.Include(t => t.AssignedToUsers)
                                                                .FirstOrDefault(t => t.Id == tickets.Id);
            if(existingTicket == null){ return NotFound(); }
            existingTicket.AssignedToUsers.Clear();
            foreach (var devId in selectedDeveloperIds)
            {
                var devToAdd = await _userManager.FindByIdAsync(devId);
                if (devToAdd != null)
                {
                    existingTicket.AssignedToUsers.Add(devToAdd);
                }
                
            }
            if (_applicationDbContext == null) { return NotFound(); }
            await   _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("Tickets");
        }
    }
}