using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ticketApp.Data;

namespace ticketApp.Controllers
{
    public class FilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FilesController(ApplicationDbContext applicationDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _context = applicationDbContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Download(int id)
        {
            var attachment = await _context.TicketAttachments.FindAsync(id);
            if(attachment == null){ return NotFound(); }

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", attachment.FileName);
            
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            
            return File(fileBytes, attachment.ContentType, attachment.FileName);
        }
    }
}