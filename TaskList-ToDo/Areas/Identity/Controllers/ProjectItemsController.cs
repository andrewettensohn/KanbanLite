using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskList_ToDo.Data;
using TaskList_ToDo.Models;

namespace ToDoApi.Controllers
{
    [Area("Identity")]
    [Route("Identity/Projects/api/ProjectItems")]
    [ApiController]
    public class ProjectItemsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public ProjectItemsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Projects/api/ProjectItems/ProjectList/UserId
        [HttpGet("ProjectList/{userId}")]
        public async Task<ActionResult<IEnumerable<ProjectItem>>> GetProjectItems(string userId)
        {
            var projectsList = await _context.ProjectItems.Where(p => p.UserId == userId).ToListAsync();

            projectsList = ProjectTaskStatsCalculator(projectsList);

            return projectsList;
        }

        //GET: Projects/api/ProjectItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectItem>> GetProjectItem(int id)
        {
            var projectItem = await _context.ProjectItems.FindAsync(id);

            if(projectItem == null)
            {
                return NotFound();
            }

            return projectItem;
        }

        // PUT: Projects/api/ProjectItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectItem(int id, ProjectItem projectItem)
        {
            if (id != projectItem.ProjectItemID)
            {
                return BadRequest();
            }

            _context.Entry(projectItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

            }

            return NoContent();
        }

        // PUT: Projects/api/ProjectItems/SetActiveProject/userId/5
        [HttpPut("SetActiveProject/{userId}/{id}")]
        public async Task<IActionResult> setActiveProject(string userId, int id, ProjectItem activeProjectItem)
        {
            if (id != activeProjectItem.ProjectItemID)
            {
                return BadRequest();
            }

            var lastActiveProject = await _context.ProjectItems.Where(p => p.ProjectIsActive == true && p.UserId == userId).ToListAsync();

            foreach (var item in lastActiveProject)
            {
                if(item.ProjectItemID == activeProjectItem.ProjectItemID && item.ProjectIsActive == true)
                {
                    return NoContent();
                }

                item.ProjectIsActive = false;
            }

            await _context.SaveChangesAsync();

            _context.Entry(activeProjectItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            _context.Entry(activeProjectItem).State = EntityState.Detached;
            return NoContent();
        }

        //POST Projects/api/ProjectItems
        [HttpPost("{userId}")]
        public async Task<ActionResult<ProjectItem>> PostProjectItem(string userId, ProjectItem projectItem)
        {
            projectItem.ProjectDescription = "Enter a project description here...";
            projectItem.ProjectCreationTime = DateTime.Now.ToLongDateString();
            //projectItem.ProjectTaskStatusStats = ProjectTaskStatusStatsHandler();

            var projectItemList = await _context.ProjectItems.Where(p => p.UserId == userId).ToListAsync();

            if (projectItemList.Count() == 0)
            {
                projectItem.ProjectIsActive = true;
            }

            if (projectItem.ProjectName == null || projectItem.ProjectName == "" )
            {
                projectItem.ProjectName = "Untitled";
            }

            _context.ProjectItems.Add(projectItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProjectItem), new { id = projectItem.ProjectItemID }, projectItem);
        }

        // DELETE: Projects/api/ProjectItems/userId/5
        [HttpDelete("{userId}/{id}")]
        public async Task<ActionResult<ProjectItem>> DeleteProjectItem(int id, string userId)
        {
            var projectItem = await _context.ProjectItems.FindAsync(id);

            if (projectItem == null)
            {
                return NotFound();
            }

            _context.ProjectItems.Remove(projectItem);

            var todoItemList = await _context.TodoItems.Where(p => p.ProjectID == id && p.UserId == userId).ToListAsync();

            foreach (var todoItem in todoItemList)
            {
                _context.TodoItems.Remove(todoItem);

                var todoSubItemList = await _context.TodoSubItems.Where(p => p.TodoItemID == todoItem.TodoItemID && p.UserId == userId).ToListAsync();

                foreach (var todoSubItem in todoSubItemList)
                {
                    _context.TodoSubItems.Remove(todoSubItem);
                }
            }

            await _context.SaveChangesAsync();

            return projectItem;
        }

        private bool ProjectItemExists(int id)
        {
            return _context.ProjectItems.Any(e => e.ProjectItemID == id);
        }

        private List<ProjectItem> ProjectTaskStatsCalculator(List<ProjectItem> projectList)
        {

            var projectListWithStats = new List<ProjectItem>();

            foreach (ProjectItem projectItem in projectList)
            {

                var todoItemList = _context.TodoItems.Where(todoItem => todoItem.ProjectID == projectItem.ProjectItemID).ToList();
                var notStartedCount = 0;
                var inProgressCount = 0;
                var completedCount = 0;


                foreach (var todoItem in todoItemList)
                {
                    if (todoItem.TaskStatus == "Not Started")
                    {
                        notStartedCount++;
                    }
                    else if (todoItem.TaskStatus == "In-Progress")
                    {
                        inProgressCount++;
                    }
                    else if (todoItem.TaskStatus == "Completed")
                    {
                        completedCount++;
                    }

                }

                var taskCountTotal = notStartedCount + inProgressCount + completedCount;

                var projectStatusStatsCalculated = new Dictionary<string, int>

                    {
                        { "Not Started", 0 },
                        { "In-Progress", 0 },
                        { "Completed", 0 }
                    };

                if (taskCountTotal <= 0)
                {
                    projectItem.ProjectStatusStats = projectStatusStatsCalculated;
                    projectListWithStats.Add(projectItem);
                } 
                else
                {

                    projectStatusStatsCalculated["Not Started"] = (int)Math.Round((double)(100 * notStartedCount) / taskCountTotal);
                    projectStatusStatsCalculated["In-Progress"] = (int)Math.Round((double)(100 * inProgressCount) / taskCountTotal);
                    projectStatusStatsCalculated["Completed"] = (int)Math.Round((double)(100 * completedCount) / taskCountTotal);

                    projectItem.ProjectStatusStats = projectStatusStatsCalculated;

                    projectListWithStats.Add(projectItem);
                }
            }

            return projectListWithStats;
        }

    }
}
