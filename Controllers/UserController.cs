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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var TicketList = _applicationDbContext.Tickets.Where(t => t.CreatedByUserId == userId!.ToString()).ToList();
            return View(TicketList);
        }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(TicketComments model, IFormFile imageFile)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imageFile.FileName);
        using (var stream = new FileStream(path, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }
        

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        model.CommentedAt = DateTime.Now;
        model.UpdatedAt = DateTime.Now;
        if (userId == null) { return NotFound(); }
        model.CommentedByUserId = userId;
        model.TicketId = model.TicketId;



        var _ticketAttachments = new TicketAttachments
            {
            TicketId = model.TicketId,
            FileName = imageFile.FileName,
            FilePath = path,
            ContentType = imageFile.ContentType,
            UploadedAt = DateTime.Now,
            UploadedByUserId = userId

        };

         _applicationDbContext.TicketComments.Add(model);
        _applicationDbContext.TicketAttachments.Add(_ticketAttachments);
        await _applicationDbContext.SaveChangesAsync();
        return RedirectToAction("Index", "User");
       
        

    }
        
    }
