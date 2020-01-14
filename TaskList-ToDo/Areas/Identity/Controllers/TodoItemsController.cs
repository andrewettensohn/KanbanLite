using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskList_ToDo.Data;
using TodoApi.Models;

namespace ToDoApi.Controllers
{
    [Area("Identity")]
    [Route("Identity/Projects/api/TodoItems")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        public TodoItemsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Projects/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // GET: Projects/TodoItems/Tasks/userId/7
        [HttpGet("Tasks/{userId}/{projectID}")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItemAndSubItems(string userId, int projectID)
        {

            var todoItems = await _context.TodoItems.Where(project => project.UserId == userId && project.ProjectID == projectID).ToListAsync();

            var todoSubItems = await _context.TodoSubItems.Where(todoItem => todoItem.UserId == userId).ToListAsync();

            return todoItems;

        }

        // GET: Projects/TodoItems/Filter/UserId/11/Not Started
        [HttpGet("Filter/{userId}/{projectID}/{filterStatus}")]
        public async Task<ActionResult<List<TodoItem>>> GetTodoItemsInProgress(string userId, int projectID, string filterStatus)
        {

            var todoItemsFiltered = await _context.TodoItems.Where(t => 
                t.ProjectID == projectID && 
                t.UserId == userId && 
                t.TaskStatus == filterStatus
            ).ToListAsync();

            return todoItemsFiltered;
        }


        // GET: Projects/TodoItems/1/2
        [HttpGet("{todoItemID}/{todoSubItemID}")]
        public async Task<ActionResult<TodoItem>> GetTodoItemAndSubItem(int todoItemID, int todoSubItemID)
        {
            var todoItem = await _context.TodoItems.FindAsync(todoItemID);

            var todoSubItem = await _context.TodoSubItems.FindAsync(todoSubItemID);

            if (todoItem == null || todoSubItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: Projects/TodoItems/1/2
        //[HttpPut("{todoItemID}/{todoSubItemID}")]
        //public async Task<IActionResult> PutTodoItemAndSubItem(int todoItemID, int todoSubItemID, TodoItem todoItem)
        //{

        //    if (todoItemID != todoItem.TodoItemID)
        //    {
        //        return BadRequest();
        //    }

        //    if (todoSubItemID != todoItem.TodoSubItems[0].TodoSubItemID)
        //    {
        //        return BadRequest();
        //    }

        //    if (todoItem.TodoSubItems[0].SubTaskStatus == "In-Progress")
        //    {
        //        todoItem.TaskStatus = todoItem.TodoSubItems[0].SubTaskStatus;
        //    }

        //    _context.Entry(todoItem).State = EntityState.Modified;

        //    _context.Entry(todoItem.TodoSubItems[0]).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TodoItemExists(todoItemID))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // PUT: Projects/TodoItems/UpdateName/5
        [HttpPut("{updateType}/{id}")]
        public async Task<IActionResult> PutTodoItem(string updateType, int id, TodoItem sentTodoItem)
        {
            if (id != sentTodoItem.TodoItemID)
            {
                return BadRequest();
            }

            var todoItem = _context.TodoItems.Where(t => t.TodoItemID == id).First();

            if(updateType == "UpdateName")
            {
                todoItem.TaskName = sentTodoItem.TaskName;
            }
            else if(updateType == "UpdateStatus")
            {
                todoItem.TaskStatus = sentTodoItem.TaskStatus;

                if(todoItem.TaskStatus == "In-Progress")
                {
                    todoItem.TaskInProgressTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                }
                else if(todoItem.TaskStatus == "Completed")
                {
                    todoItem.TaskCompletionTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                }
            }
            else
            {
                return BadRequest();
            }

            if (todoItem.TaskName == "" || todoItem.TaskName is null)
            {
                todoItem.TaskName = "Untitled";
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
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

        // POST: Projects/TodoItems
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {

            if (todoItem.TaskName == "" || todoItem.TaskName is null)
            {
                todoItem.TaskName = "Untitled";
            }

            todoItem.TaskCreationTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            var projectItem = await _context.ProjectItems.FindAsync(todoItem.ProjectID);
            projectItem.ProjectTotalTasks++;

            _context.Entry(projectItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.TodoItemID }, todoItem);
        }

        // DELETE: Projects/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            var subItems = await _context.TodoSubItems.ToListAsync();

            foreach(var subItem in subItems)
            {
                if(subItem.TodoItemID == id)
                {
                    _context.TodoSubItems.Remove(subItem);
                    await _context.SaveChangesAsync();
                }
            }

            var projectItem = await _context.ProjectItems.FindAsync(todoItem.ProjectID);
            projectItem.ProjectTotalTasks--;

            _context.Entry(projectItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return todoItem;
        }

        private bool TodoItemExists(int id)
        {
            return _context.TodoItems.Any(e => e.TodoItemID == id);
        }
    }
}
