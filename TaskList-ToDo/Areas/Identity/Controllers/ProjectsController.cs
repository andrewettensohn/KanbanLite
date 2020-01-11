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
        private readonly ILogger<ProjectsController> _logger;

        private readonly ApplicationDbContext _context;

        public IActionResult Board()
        {

            return View();
        }

        public IActionResult Project()
        {

            return View();
        }

        public IActionResult Tags()
        {

            return View();

        }

        public IActionResult TagList()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tagList = _context.Tag.Where(t => t.UserId == userId).ToList();

            return PartialView("_ViewTags", tagList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
