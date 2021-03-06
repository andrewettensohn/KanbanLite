﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        // GET: api/TodoSubItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoSubItem>>> GetToDoSubItem()
        {
            return await _context.TodoSubItems.ToListAsync();
        }

        // GET api/TodoSubItems/5
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

        // PUT api/TodoSubItems/5
        [HttpPut("{updateType}/{id}")]
        public async Task<ActionResult<TodoSubItem>> PutTodoSubItem(string updateType, int id, TodoSubItem sentTodoSubItem)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            TodoSubItem todoSubItem = _context.TodoSubItems.Find(id);

            if (id != sentTodoSubItem.TodoSubItemID || todoSubItem.UserId != userId)
            {
                return BadRequest();
            }

            if(updateType == "UpdateName")
            {
                if (todoSubItem.SubTaskName == "" || todoSubItem.SubTaskName is null)
                {
                    todoSubItem.SubTaskName = "Untitled";
                }
                else
                {
                    todoSubItem.SubTaskName = sentTodoSubItem.SubTaskName;
                }

            } 
            else if (updateType == "UpdateDescription")
            {
                if (todoSubItem.SubTaskDescription is null)
                {
                    todoSubItem.SubTaskDescription = "";
                }
                else
                {
                    todoSubItem.SubTaskDescription = sentTodoSubItem.SubTaskDescription;
                }
            }
            else if(updateType == "UpdateStatus")
            {
                if(sentTodoSubItem.SubTaskStatus == "In-Progress")
                {
                    TodoItem todoItem = _context.TodoItems.Find(todoSubItem.TodoItemID);

                    todoItem.TaskStatus = "In-Progress";

                    _context.Entry(todoItem).State = EntityState.Modified;

                    _context.SaveChanges();

                    todoSubItem.SubTaskStatus = sentTodoSubItem.SubTaskStatus;

                }
                else
                {
                    todoSubItem.SubTaskStatus = sentTodoSubItem.SubTaskStatus;
                }
            }
            else
            {
                return BadRequest();
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

        // POST api/TodoSubItems
        [HttpPost]
        public async Task<ActionResult<TodoSubItem>> PostTodoSubItem(TodoSubItem todoSubItem)
        {

            todoSubItem.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (todoSubItem.SubTaskName == "" || todoSubItem.SubTaskName is null)
            {
                todoSubItem.SubTaskName = "Untitled";
            }

            _context.TodoSubItems.Add(todoSubItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetToDoSubItem), new { id = todoSubItem.TodoSubItemID }, todoSubItem);
        }

        // DELETE api/TodoSubItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoSubItem>> DeleteTodoSubItem(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var todoSubItem = await _context.TodoSubItems.FindAsync(id);

            if(todoSubItem == null || todoSubItem.UserId != userId)
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
