
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    public class TodoItem
    {
        public int TodoItemID { get; set; }

        public string TaskName { get; set; }

        public string TaskStatus { get; set; }

        public List<TodoSubItem> TodoSubItems { get; set; }

    }
}