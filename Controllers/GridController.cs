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

        [HttpPost]
        public async Task<IActionResult> EditUserInline([DataSourceRequest] DataSourceRequest request, Tickets model)
        {
            if (model != null)
            {
                var existingTicket = await _applicationDbContext.Tickets.FindAsync(model.Id);
                existingTicket.Title = model.Title;
                existingTicket.Description = model.Description;
                existingTicket.ticketPriority = model.ticketPriority;
                existingTicket.ticketType = model.ticketType;
                await _applicationDbContext.SaveChangesAsync();

            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public async Task<ActionResult> DeleteAdminInline([DataSourceRequest] DataSourceRequest request, UserGridModel model)
        {

            if (model != null)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null) { return NotFound(); }
                await _userManager.DeleteAsync(user);


            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));


        }
        [HttpPost]
        public async Task<ActionResult> UserCreateFromAdmin([DataSourceRequest] DataSourceRequest request, UserGridModel model)
        {

            if (model != null)
            {
                var user = new AppUser { Email = model.Email, UserName = model.UserName };
                IdentityResult result = await _userManager.CreateAsync(user, "useR123.");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.CurrentRole);


                }
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));


        }

        [HttpPost]
        public async Task<ActionResult> EditAdminInline([DataSourceRequest] DataSourceRequest request, Tickets model)
        {

            if (model != null)
            {
                var existingTicket = await _applicationDbContext.Tickets.FindAsync(model.Id);

                existingTicket.Title = model.Title;
                existingTicket.Description = model.Description;
                existingTicket.ticketPriority = model.ticketPriority;
                existingTicket.ticketType = model.ticketType;
                await _applicationDbContext.SaveChangesAsync();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public async Task<ActionResult> UpdateUserRole([DataSourceRequest] DataSourceRequest request, UserGridModel model)
        {

            if (model != null && ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null) { return NotFound(); }
                var currentRole = await _userManager.GetRolesAsync(user);
                if (currentRole.Any())
                    await _userManager.RemoveFromRolesAsync(user, currentRole);
                if (model.CurrentRole != null)
                    await _userManager.AddToRoleAsync(user, model.CurrentRole);
                user.UserName = model.UserName;
                user.Email = model.Email;
                await _applicationDbContext.SaveChangesAsync();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }



        public ActionResult Projects([DataSourceRequest] DataSourceRequest request)
        {
            var model = from project in _applicationDbContext.Projects
                        join user in _applicationDbContext.Users on project.CreatedByUserId equals user.Id
                        select new
                        {
                            Id = project.Id,
                            Name = project.Name,
                            Description = project.Description,
                            CreatedAt = project.CreatedAt,
                            CreatedByUserId = user.UserName
                        };
            return Json(model.ToDataSourceResult(request));
        }

        [HttpPost]
        public async Task<ActionResult> AddProject([DataSourceRequest] DataSourceRequest request, Projects model)
        {

            if (model != null)
            {
                model.CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.CreatedAt = DateTime.Now;
                _applicationDbContext.Projects.Add(model);
                await _applicationDbContext.SaveChangesAsync();

            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));


        }
        [HttpPost]
        public async Task<ActionResult> UpdateProject([DataSourceRequest] DataSourceRequest request, Projects model)
        {

            if (model != null)
            {
                var existingProject = await _applicationDbContext.Projects.FindAsync(model.Id);
                if (existingProject == null) { return NotFound(); }
                existingProject.Name = model.Name;
                existingProject.Description = model.Description;


                await _applicationDbContext.SaveChangesAsync();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        
        [HttpPost]
        public async Task<ActionResult> DeleteProject([DataSourceRequest] DataSourceRequest request, Projects model)
        {

            if (model != null)
            {
                
                _applicationDbContext.Projects.Remove(model);
                
                await _applicationDbContext.SaveChangesAsync();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

    }
}


