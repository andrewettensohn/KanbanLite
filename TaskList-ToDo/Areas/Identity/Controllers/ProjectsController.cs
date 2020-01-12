using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskList_ToDo.Models;
using TaskList_ToDo.Data;


namespace TaskList_ToDo.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class ProjectsController : Controller
    {
        //private readonly ILogger<ProjectsController> _logger;

        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Board()
        {

            return View();
        }

        public IActionResult Project()
        {

            return View();
        }

        public IActionResult ArchivedProject()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = _context.ProjectItems.Where(p => p.UserId == userId && p.ProjectIsArchived == true).ToList();

            return View(model);
        }

        public IActionResult Tags()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = _context.Tag.Where(t => t.UserId == userId).ToList();

            return View(model);

        }


        public IActionResult Story()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
