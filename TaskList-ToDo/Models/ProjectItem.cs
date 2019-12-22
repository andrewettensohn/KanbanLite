using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskList_ToDo.Models
{
    public class ProjectItem
    {
        public int ProjectID { get; set; }

        public string UserId { get; set; }
        public string ProjectName { get; set; }

        public string ProjectDescription { get; set; }

        public bool ProjectIsActive { get; set; }

        //public string ProjectTemplate { get; set; }

        //public bool ProjectHasStories { get; set; }

    }
}
