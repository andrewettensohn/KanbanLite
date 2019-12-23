using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskList_ToDo.Data;
using TaskList_ToDo.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaskList_ToDo.Areas.Identity.Controllers
{
    [Route("Projects/api/ProjectItems")]
    public class ProjectItemsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public ProjectItemsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Projects/api/ProjectItems/UserId
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<ProjectItem>>> GetProjectItems(string userId)
        {
            var projectItemList = await _context.ProjectItems.ToListAsync();

            var queryUserProjectItems = from ProjectItem projectItem in projectItemList
                                        where projectItem.UserId == userId
                                        select projectItem;

            var userProjectItems = new List<ProjectItem>();

            foreach (ProjectItem t in queryUserProjectItems)
            {
                userProjectItems.Add(t);
            }

            return userProjectItems;
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


        //POST Projects/api/ProjectItems
       [HttpPost]
        public async Task<ActionResult<ProjectItem>> PostProjectItem(ProjectItem projectItem)
        {
            _context.ProjectItems.Add(projectItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProjectItem), new { id = projectItem.ProjectItemID }, projectItem);
        }

    }
}
