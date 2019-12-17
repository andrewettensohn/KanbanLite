
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class TodoItem
    {
        public string UserID { get; set; }
        public int TodoItemID { get; set; }

        public string TaskName { get; set; }

        public string TaskStatus { get; set; }

        public List<TodoSubItem> TodoSubItems { get; set; }

    }
}