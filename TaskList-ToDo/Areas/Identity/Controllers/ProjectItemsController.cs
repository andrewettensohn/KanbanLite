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

        // PUT: Projects/api/ProjectItems/userId/UpdateName/5
        [HttpPut("{userId}/{updateType}/{id}")]
        public async Task<IActionResult> PutProjectItem(string updateType, int id, ProjectItem sentProjectItem)
        {
            if (id != sentProjectItem.ProjectItemID)
            {
                return BadRequest();
            }

            var projectItem = await _context.ProjectItems.FindAsync(id);

            if(updateType == "UpdateDescription")
            {
                projectItem.ProjectDescription = sentProjectItem.ProjectDescription;
            }
            else if(updateType == "UpdateName")
            {
                projectItem.ProjectName = sentProjectItem.ProjectName;
            }
            else
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
        public async Task<IActionResult> setActiveProject(string userId, int id, ProjectItem sentProjectItem)
        {
            if (id != sentProjectItem.ProjectItemID)
            {
                return BadRequest();
            }

            var activeProjectItem = await _context.ProjectItems.FindAsync(id);
            activeProjectItem.ProjectIsActive = true;

            var lastActiveProject = await _context.ProjectItems.Where(p => p.ProjectIsActive == true && p.UserId == userId).ToListAsync();

            foreach (var item in lastActiveProject)
            {
                if (item.ProjectItemID == activeProjectItem.ProjectItemID && item.ProjectIsActive == true)
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
                        { "Not Started Count", 0 },
                        { "Not Started Percent", 33 },
                        { "In-Progress Count", 0 },
                        { "In-Progress Percent", 33 },
                        { "Completed Count", 0 },
                        { "Completed Percent", 33 }
                    };

                if (taskCountTotal <= 0)
                {
                    projectItem.ProjectStatusStats = projectStatusStatsCalculated;
                    projectListWithStats.Add(projectItem);
                } 
                else
                {
                    projectStatusStatsCalculated["Not Started Count"] = notStartedCount;
                    projectStatusStatsCalculated["Not Started"] = (int)Math.Round((double)(100 * notStartedCount) / taskCountTotal);
                    projectStatusStatsCalculated["In-Progress Count"] = inProgressCount;
                    projectStatusStatsCalculated["In-Progress"] = (int)Math.Round((double)(100 * inProgressCount) / taskCountTotal);
                    projectStatusStatsCalculated["Completed Count"] = completedCount;
                    projectStatusStatsCalculated["Completed"] = (int)Math.Round((double)(100 * completedCount) / taskCountTotal);

                    projectItem.ProjectStatusStats = projectStatusStatsCalculated;

                    projectListWithStats.Add(projectItem);
                }
            }

            return projectListWithStats;
        }

    }
}
