﻿using System;
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

        // GET: Projects/TodoItems/Tasks/userId
        [HttpGet("Tasks/{userId}")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItemAndSubItems(string userId)
        {
            var todoItemList = new List<TodoItem>();

            while (todoItemList.Count() <=  0)
            {

                todoItemList = await _context.TodoItems.ToListAsync();

                await _context.TodoSubItems.ToListAsync();

                if(todoItemList.Count() <= 0)
                {
                    System.Threading.Thread.Sleep(10000); 
                }
            }

            var queryUserToDoItems = from TodoItem todoItem in todoItemList
                                     where todoItem.UserId == userId
                                     select todoItem;

            var userTodoItems = new List<TodoItem>();

            foreach (TodoItem t in queryUserToDoItems)
            {
                userTodoItems.Add(t);
            }

            return userTodoItems;

        }

        // GET: Projects/TodoItems/Filter/UserId/Not Started
        [HttpGet("Filter/{userId}/{filterStatus}")]
        public async Task<ActionResult<List<TodoItem>>> GetTodoItemsInProgress(string userId, string filterStatus)
        {
            var todoItems = await _context.TodoItems.ToListAsync();

            await _context.TodoSubItems.ToListAsync();


            var todoItemsFiltered = new List<TodoItem>();

            foreach(var item in todoItems)
            {
                if(item.TaskStatus == filterStatus)
                {
                    todoItemsFiltered.Add(item);
                }
            }

            var queryUserToDoItemsFiltered = from TodoItem todoItem in todoItemsFiltered
                                             where todoItem.UserId == userId
                                             select todoItem;

            var userTodoItemsFiltered = new List<TodoItem>();

            foreach (TodoItem t in queryUserToDoItemsFiltered)
            {
                userTodoItemsFiltered.Add(t);
            }



            return userTodoItemsFiltered;
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
        [HttpPut("{todoItemID}/{todoSubItemID}")]
        public async Task<IActionResult> PutTodoItemAndSubItem(int todoItemID, int todoSubItemID, TodoItem todoItem)
        {

            if (todoItemID != todoItem.TodoItemID)
            {
                return BadRequest();
            }

            if (todoSubItemID != todoItem.TodoSubItems[0].TodoSubItemID)
            {
                return BadRequest();
            }

            if (todoItem.TodoSubItems[0].SubTaskStatus == "In-Progress")
            {
                todoItem.TaskStatus = todoItem.TodoSubItems[0].SubTaskStatus;
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            _context.Entry(todoItem.TodoSubItems[0]).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(todoItemID))
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

        // PUT: Projects/TodoItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItem todoItem)
        {
            if (id != todoItem.TodoItemID)
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

            _context.TodoItems.Add(todoItem);
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

            return todoItem;
        }

        private bool TodoItemExists(int id)
        {
            return _context.TodoItems.Any(e => e.TodoItemID == id);
        }
    }
}
