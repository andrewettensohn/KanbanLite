using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskList_ToDo.Data;
using TodoApi.Models;

namespace ToDoApi.Controllers
{
    [Area("Identity")]
    [Route("Identity/Projects/api/TodoSubItems")]
    [ApiController]
    public class TodoSubItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TodoSubItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoSubItem>>> GetToDoSubItem()
        {
            return await _context.TodoSubItems.ToListAsync();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoSubItem>> GetToDoItem(int id)
        {
            var todoSubItem = await _context.TodoSubItems.FindAsync(id);

            if (todoSubItem == null)
            {
                return NotFound();
            }

            return todoSubItem;
        }

        // PUT Projects/api/TodoSubItems/5
        [HttpPut("{id}")]
        public async Task<ActionResult<TodoSubItem>> PutTodoSubItem(int id, TodoSubItem todoSubItem)
        {

            if(id != todoSubItem.TodoSubItemID)
            {
                return BadRequest();
            }

            if (todoSubItem.SubTaskName == "" || todoSubItem.SubTaskName is null)
            {
                todoSubItem.SubTaskName = "Untitled";
            }

            if (todoSubItem.SubTaskDescription is null)
            {
                todoSubItem.SubTaskDescription = "";
            }

            _context.Entry(todoSubItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoSubItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return todoSubItem;
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<ActionResult<TodoSubItem>> PostTodoSubItem(TodoSubItem todoSubItem)
        {

            if(todoSubItem.SubTaskName == "" || todoSubItem.SubTaskName is null)
            {
                todoSubItem.SubTaskName = "Untitled";
            }

            _context.TodoSubItems.Add(todoSubItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetToDoSubItem), new { id = todoSubItem.TodoSubItemID }, todoSubItem);
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoSubItem>> DeleteTodoSubItem(int id)
        {
            var todoSubItem = await _context.TodoSubItems.FindAsync(id);
            if(todoSubItem == null)
            {
                return NotFound();
            }

            _context.TodoSubItems.Remove(todoSubItem);
            await _context.SaveChangesAsync();

            return todoSubItem;

        }

        private bool TodoSubItemExists(int id)
        {
            return _context.TodoSubItems.Any(e => e.TodoSubItemID == id);
        }
    }
}
