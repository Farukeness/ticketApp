using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ticketApp.Data;

namespace ticketApp.Controllers
{
    [Authorize(Roles ="Developer")]
    public class DeveloperController : Controller
    {

        private readonly ApplicationDbContext _applicationDbContext;
        public DeveloperController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }


        public IActionResult Index()
        {
            var devId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tickets = _applicationDbContext.Tickets.Where(t => t.CreatedByUserId == devId);

            return View(tickets);
        }
    }
}