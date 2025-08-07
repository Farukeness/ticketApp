using System.Security.Claims;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ticketApp.Data;
using ticketApp.Enums;
using ticketApp.Models;
using ticketApp.ViewModel;
using System.Linq;
namespace ticketApp.Controllers
{

    [Authorize]
    public class GridController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;


        public GridController(ApplicationDbContext applicationDbContext, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
            _roleManager = roleManager;


        }
        public ActionResult Tickets_Read([DataSourceRequest] DataSourceRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var TicketList = _applicationDbContext.Tickets.Where(t => t.CreatedByUserId == userId!);

            return Json(TicketList.ToDataSourceResult(request));
        }
        public ActionResult Tickets_Read_For_Dev([DataSourceRequest] DataSourceRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ticketList = _applicationDbContext.Tickets
                .Include(t => t.AssignedToUsers)
                .Where(t => t.AssignedToUsers.Any(u => u.Id == userId))
                .Select(t => new Tickets
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    ticketType = t.ticketType,
                    ticketPriority = t.ticketPriority,
                    ticketStatus = t.ticketStatus,
                    CreatedAt = t.CreatedAt
                })
                .AsQueryable();

            return Json(ticketList.ToDataSourceResult(request));
        }


        public ActionResult Tickets_All([DataSourceRequest] DataSourceRequest request)
        {

            return Json(_applicationDbContext.Tickets.ToDataSourceResult(request));
        }

        public async Task<ActionResult> Users([DataSourceRequest] DataSourceRequest request)
        {
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            var model = new List<UserGridModel>();
            foreach (var user in _userManager.Users.ToList())
            {
                var userRole = await _userManager.GetRolesAsync(user);
                model.Add(new UserGridModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    CurrentRole = userRole.FirstOrDefault()

                });
            }
            return Json(model.ToDataSourceResult(request));
        }

        public IActionResult GetRoles()
        {
            var allRoles = _roleManager.Roles.Select(r => new { Name = r.Name }).ToList();
            return Json(allRoles);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole([DataSourceRequest] DataSourceRequest request, UserGridModel model)
        {
            try
            {
                Console.WriteLine($"UpdateUserRole called with Id: {model.Id}, CurrentRole: {model.CurrentRole}");
                
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    var currentRole = await _userManager.GetRolesAsync(user);
                    if (currentRole.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, currentRole);
                    }
                    
                    if (!string.IsNullOrEmpty(model.CurrentRole))
                    {
                        await _userManager.AddToRoleAsync(user, model.CurrentRole);
                    }
                    
                    Console.WriteLine($"User role updated successfully for user: {user.UserName}");
                    return Json(new { success = true, message = "Rol başarıyla güncellendi" });
                }
                
                Console.WriteLine("User not found");
                return Json(new { success = false, message = "Kullanıcı bulunamadı" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateUserRole: {ex.Message}");
                return Json(new { success = false, message = "Güncelleme sırasında hata oluştu: " + ex.Message });
            }
        }
        
        
    }
}