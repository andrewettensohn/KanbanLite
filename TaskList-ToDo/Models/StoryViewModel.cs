using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TaskList_ToDo.Models
{
    public class StoryViewModel
    {
        public List<ProjectItem> ProjectItems { get; set; }

        public List<TodoItem> TodoItems { get; set; }

    }
}
