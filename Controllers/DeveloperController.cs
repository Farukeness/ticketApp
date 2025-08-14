using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ticketApp.Data;
using ticketApp.Models;
using ticketApp.ViewModel;

namespace ticketApp.Controllers
{
    [Authorize(Roles = "Developer")]
    public class DeveloperController : Controller
    {

        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<AppUser> _userManager;
        public DeveloperController(ApplicationDbContext applicationDbContext, UserManager<AppUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? Id)
        {
            if (Id == null) { return NotFound(); }
            var _ticket = await _applicationDbContext.Tickets.FindAsync(Id);
            var _ticketComment = _applicationDbContext.TicketComments.Where(t => t.TicketId == Id);
            var _ticketAttachemnts = _applicationDbContext.TicketAttachments.Where(t => t.TicketId == Id);
            
            

            var model = new TicketDetailViewModel
            {
                ticket = _ticket,
                ticketComment = _ticketComment,
                ticketAttachments = _ticketAttachemnts,
                Usernames = _userManager.Users.ToList()

            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Detail(Tickets model)
        {
            var existingTicket = await _applicationDbContext.Tickets.FindAsync(model.Id);
            if (existingTicket == null) { return NotFound(); }
            existingTicket.ticketStatus = model.ticketStatus;
            await _applicationDbContext.SaveChangesAsync();
            TempData["statusChange"] = "Durum Değiştirildi";
            return RedirectToAction("Detail", "Developer", model.Id);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddComment(int ticketId, string comment)
        {
            var model = new TicketComments
            {
                TicketId = ticketId,
                CommentText = comment,
                CommentedAt = DateTime.Now,
                CommentedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)

            };
            _applicationDbContext.TicketComments.Add(model);
            await _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("Detail","Developer",new {Id=ticketId});
        }
    }
}