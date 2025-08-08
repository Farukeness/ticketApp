using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ticketApp.Data;
using ticketApp.Models;
using ticketApp.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ticketApp.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {

        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<AppUser> _userManager;
        public TicketController(ApplicationDbContext applicationDbContext,
        UserManager<AppUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;

        }
        public IActionResult Create()
        {
            ViewBag.Projects = _applicationDbContext.Projects.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Tickets model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                var _ticket = new Tickets
                {
                    Title = model.Title,
                    Description = model.Description,
                    ticketType = model.ticketType,
                    ticketPriority = model.ticketPriority,
                    ticketStatus = TicketStatus.Acik,
                    CreatedAt = DateTime.Now,
                    CreatedByUserId = userId,
                    ProjectId = model.ProjectId

                };
                _applicationDbContext.Tickets.Add(_ticket);
                await _applicationDbContext.SaveChangesAsync();
                TempData["success"] = "Başarıyla eklendi.";
                return RedirectToAction("Index", "User");
            }
            return View(model);
        }
    }
}