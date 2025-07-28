using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ticketApp.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using ticketApp.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ticketApp.ViewModel;
using System.Threading.Tasks;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace ticketApp.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly UserManager<AppUser> _userManager;
    public UserController(ApplicationDbContext applicationDbContext, UserManager<AppUser> userManager)
    {
        _applicationDbContext = applicationDbContext;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(TicketComments model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        model.CommentedAt = DateTime.Now;
        model.UpdatedAt = DateTime.Now;
        if (userId == null) { return NotFound(); }
        model.CommentedByUserId = userId;
        model.TicketId = model.TicketId;
        _applicationDbContext.TicketComments.Add(model);
        await _applicationDbContext.SaveChangesAsync();
        return RedirectToAction("Detail", "User",new {Id = model.TicketId});
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCommentFile(int TicketId, IFormFile imageFile)
    {
        var extension = Path.GetExtension(imageFile.FileName);
        var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", randomFileName);
        using (var stream = new FileStream(path, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }


        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) { return NotFound(); }



        var _ticketAttachments = new TicketAttachments
        {
            TicketId = TicketId,
            FileName = randomFileName,
            FilePath = path,
            ContentType = imageFile.ContentType,
            UploadedAt = DateTime.Now,
            UploadedByUserId = userId

        };
        _applicationDbContext.TicketAttachments.Add(_ticketAttachments);
        await _applicationDbContext.SaveChangesAsync();
        return RedirectToAction("Detail", "User",new{Id =TicketId });



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


        
    }
