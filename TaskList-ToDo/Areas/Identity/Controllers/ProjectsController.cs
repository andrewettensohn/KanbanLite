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
using TodoApi.Models;

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

            foreach(var project in model)
            {
                project.ProjectCompletionTimeString = project.ProjectCompletionTime.ToString("MM/dd/yyyy hh:mm tt");
                project.ProjectCreationTimeString = project.ProjectCreationTime.ToString("MM/dd/yyyy hh:mm tt");
            }

            return View(model);
        }


        public IActionResult Story()
        {
            var model = new StoryViewModel();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            model.ActiveProjectItem = _context.ProjectItems.Where(p => p.UserId == userId && p.ProjectIsActive == true).First();
            model.TodoItems = _context.TodoItems.Where(t => t.UserId == userId && t.ProjectID == model.ActiveProjectItem.ProjectItemID).ToList();
            model.ProjectItems = _context.ProjectItems.Where(p => p.UserId == userId).ToList();

            model.TaskStoryList = new List<TaskStory>();

            foreach(TodoItem todoitem in model.TodoItems)
            {
                TaskStory taskStory = new TaskStory
                {
                    ActionTime = todoitem.TaskCreationTime,
                    ActionType = "Created",
                    TaskName = todoitem.TaskName
                };
                model.TaskStoryList.Add(taskStory);

                if(todoitem.TaskInProgressTime > DateTime.MinValue)
                {
                    taskStory = new TaskStory
                    {
                        ActionTime = todoitem.TaskInProgressTime,
                        ActionType = "Moved to In-Progress",
                        TaskName = todoitem.TaskName
                    };
                    model.TaskStoryList.Add(taskStory);
                }

                if(todoitem.TaskCompletionTime > DateTime.MinValue)
                {
                    taskStory = new TaskStory
                    {
                        ActionTime = todoitem.TaskCompletionTime,
                        ActionType = "Moved to Completed",
                        TaskName = todoitem.TaskName
                    };
                    model.TaskStoryList.Add(taskStory);
                }
            }

            model.TaskStoryList = model.TaskStoryList.OrderBy(x => x.ActionTime).ToList();


            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

