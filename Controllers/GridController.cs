using System.Security.Claims;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ticketApp.Data;
using ticketApp.Models;

namespace ticketApp.Controllers
{

    [Authorize]
    public class GridController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<AppUser> _userManager;

        public GridController(ApplicationDbContext applicationDbContext,UserManager<AppUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;

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
            var TicketList = _applicationDbContext.Tickets.Where(t => t.AssignedToUserId == userId!);

            return Json(TicketList.ToDataSourceResult(request));
        }

        public ActionResult Tickets_All([DataSourceRequest] DataSourceRequest request)
        {

            return Json(_applicationDbContext.Tickets.ToDataSourceResult(request));
        }
        
        public ActionResult Users([DataSourceRequest] DataSourceRequest request)
        {
            
            return Json(_userManager.Users.ToDataSourceResult(request));
        }
        
        
    }
}