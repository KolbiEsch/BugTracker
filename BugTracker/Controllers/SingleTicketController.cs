using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.ProjectServices;
using Microsoft.AspNetCore.Authorization;

namespace BugTracker.Controllers
{
    public class SingleTicketController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;

        public SingleTicketController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _context = context;
            _authorizationService = authorizationService;
        }
       
        [HttpGet]
        public async Task<IActionResult> Index(string id, string uploadMessage = null)
        {
            var model = new SingleTicketModel();
            Ticket ticket = getTicketByID(id);

            var result = await _authorizationService.AuthorizeAsync(User, ticket, "assignedTicketPolicy");

            if (!result.Succeeded)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return new ForbidResult();
                }
                else
                {
                    return new ChallengeResult();
                }
            }

            model.ticket = ticket;
            model.UploadMessage = uploadMessage;
            var user = await _userManager.FindByIdAsync(ticket.AssignedByID);
            var userFullName = user.FullName;
            ViewData["FullName"] = userFullName;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, string ticketID)
        {
            if (file == null || file.Length == 0)
            {
                return Content("file not selected");
            }
            Ticket ticket = getTicketByID(ticketID);
            var types = GetMimeTypes();
            var path = Path.Combine("wwwroot", file.FileName);
            if (!types.ContainsKey(Path.GetExtension(path).ToLowerInvariant()))
            {
                return RedirectToAction("Index", new { id = ticket.TicketID, uploadMessage = "Incorrect ext"});
            }
            var filePathRandomName = Path.Combine("wwwroot", Path.GetRandomFileName());
            FilePath paths = new FilePath
            {
                ID = Guid.NewGuid().ToString("N"),
                HiddenPath = filePathRandomName,
                UnhiddenPath = path
                
            };
            ticket.FilePaths.Add(paths);
            _context.SaveChanges();
            
            using (var stream = new FileStream(filePathRandomName, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return RedirectToAction("Index", new { id = ticket.TicketID});
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTicket(string ticketID)
        {
            Ticket ticket = getTicketByID(ticketID);
            foreach(var path in ticket.FilePaths)
            {
                FileInfo file = new FileInfo(path.HiddenPath);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            _context.Tickets.Remove(ticket);
            _context.SaveChanges();
            return RedirectToAction("Index", "MyTickets");
        }

        public async Task<FileContentResult> GetContent(string fileID)
        {
            var file = getFileByID(fileID);
            byte[] document = System.IO.File.ReadAllBytes(file.HiddenPath);
            return File(document, GetContentType(file.UnhiddenPath));
        }

        private string GetContentType(string unhiddenPath)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(unhiddenPath).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/msword"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},  
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".css", "text/css"},
                {".html", "text/html"},
                {".json", "application/json"}
            };
        }

        private Ticket getTicketByID(string id)
        {
            foreach(var ticket in _context.Tickets)
            {
                if (ticket.TicketID == id)
                {
                    return ticket;
                }
            }
            return null;
        }

        private FilePath getFileByID(string id)
        {
            foreach(var FilePath in _context.FilePaths)
            {
                if (FilePath.ID == id)
                {
                    return FilePath;
                }
            }
            return null;
        }
    }
}
