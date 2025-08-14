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
using System.Globalization;
namespace ticketApp.Controllers
{

    [Authorize]
    public class GridController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

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
            var model = from ticket in TicketList
                        join project in _applicationDbContext.Projects
                        on ticket.ProjectId equals project.Id
                        select new
                        {
                            Id = ticket.Id,
                            Title = ticket.Title,
                            Description = ticket.Description,
                            ticketType = ticket.ticketType,
                            ticketPriority = ticket.ticketPriority,
                            ticketStatus = ticket.ticketStatus,
                            CreatedAt = ticket.CreatedAt,
                            ProjectId = ticket.ProjectId,
                            ProjectName = project.Name

                        };
            return Json(model.ToDataSourceResult(request));
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
            var model = _applicationDbContext.Tickets.Join(
                _applicationDbContext.Projects,
                ticket => ticket.ProjectId,
                project => project.Id,
                (ticket, project) => new
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    ticketType = ticket.ticketType,
                    ticketPriority = ticket.ticketPriority,
                    ticketStatus = ticket.ticketStatus,
                    CreatedAt = ticket.CreatedAt,
                    ProjectName = project.Name,
                    CreatedByUserId = textInfo.ToTitleCase(_applicationDbContext.Users.FirstOrDefault(t => t.Id == ticket.CreatedByUserId).UserName),
                    AssignmentControl = ticket.AssignedToUsers.Count == 0 ? "Atama Yok" : "Atanmış"
                }
            );


            return Json(model.ToDataSourceResult(request));
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
        public async Task<IActionResult> EditUserInline([DataSourceRequest] DataSourceRequest request, TicketViewModel model)
        {
            if (model != null)
            {
                var existingTicket = await _applicationDbContext.Tickets.FindAsync(model.Id);
                existingTicket.Title = model.Title;
                existingTicket.Description = model.Description;
                existingTicket.ticketPriority = model.ticketPriority;
                existingTicket.ticketType = model.ticketType;
                existingTicket.ticketStatus = model.ticketStatus;
                var proje = _applicationDbContext.Projects.Where(c => c.Name == model.ProjectName).FirstOrDefault();
                if (proje == null) { return NotFound(); }
                existingTicket.ProjectId = proje.Id;
                await _applicationDbContext.SaveChangesAsync();

            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]

        public async Task<IActionResult> AddTicket([DataSourceRequest] DataSourceRequest request, TicketViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var proje = _applicationDbContext.Projects.Where(c => c.Name == model.ProjectName).FirstOrDefault();


            var _ticket = new Tickets
            {
                Title = model.Title,
                Description = model.Description,
                ticketType = model.ticketType,
                ticketPriority = model.ticketPriority,
                ticketStatus = TicketStatus.Acik,
                CreatedAt = DateTime.Now,
                CreatedByUserId = userId,
                ProjectId = proje.Id

            };
            _applicationDbContext.Tickets.Add(_ticket);
            await _applicationDbContext.SaveChangesAsync();
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));

        }

        [HttpPost]
        public async Task<ActionResult> DestroyTicket_Admin([DataSourceRequest] DataSourceRequest request, Tickets model)
        {

            if (model != null)
            {

                _applicationDbContext.Tickets.Remove(model);

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
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Code);
                }
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));


        }

        // [HttpPost]
        // public async Task<ActionResult> EditAdminInline([DataSourceRequest] DataSourceRequest request, Tickets model)
        // {

        //     if (model != null)
        //     {
        //         var existingTicket = await _applicationDbContext.Tickets.FindAsync(model.Id);

        //         existingTicket.Title = model.Title;
        //         existingTicket.Description = model.Description;
        //         existingTicket.ticketPriority = model.ticketPriority;
        //         existingTicket.ticketType = model.ticketType;
        //         await _applicationDbContext.SaveChangesAsync();
        //     }

        //     return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        // }
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
            var model = _applicationDbContext.Projects.Join(
                _applicationDbContext.Users,
                project => project.CreatedByUserId,
                user => user.Id,
                (project, user) => new
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    CreatedAt = project.CreatedAt,
                    CreatedByUserId = user.UserName
                }
            );

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

        public ActionResult Projects_For_User([DataSourceRequest] DataSourceRequest request)
        {

            return Json(_applicationDbContext.Projects.ToDataSourceResult(request));
        }
        public ActionResult ProjectChart([DataSourceRequest] DataSourceRequest request)
        {
            var totalTicket = _applicationDbContext.Tickets.Count();

            var projectData = _applicationDbContext.Projects
                .GroupJoin(
                    _applicationDbContext.Tickets,
                    project => project.Id,
                    ticket => ticket.ProjectId,
                    (project, tickets) => new
                    {
                        ProjectName = project.Name,
                        TicketCount = tickets.Count()
                    }
                )
                .Select(p => new ProjectChartView
                {
                    ProjeTuru = p.ProjectName,
                    TicketCount = Math.Round(totalTicket > 0 ? ((double)p.TicketCount / totalTicket) * 100 : 0)
                })
                .ToList();

            return Json(projectData);
        }
        public async Task<ActionResult> Developers([DataSourceRequest] DataSourceRequest request, int Id)
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

                return Json(developersWithStatus.ToDataSourceResult(request));
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            

            //return Json(dev.ToDataSourceResult(request));
        }


    }
}


