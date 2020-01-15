using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TaskList_ToDo.Models
{
    public class StoryViewModel
    {
        public ProjectItem ActiveProjectItem { get; set; }

        public List<ProjectItem> ProjectItems { get; set; }

        public List<TodoItem> TodoItems { get; set; }

        public List<TaskStory> TaskStoryList { get; set; }

    }

    public class TaskStory
    {
        public DateTime ActionTime { get; set; }

        public string ActionType { get; set; }

        public string TaskName { get; set; }

    }

}
