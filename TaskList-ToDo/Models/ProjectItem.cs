﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskList_ToDo.Models
{
    public class ProjectItem
    {
        public int ProjectItemID { get; set; }

        public string UserId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectDescription { get; set; }

        public bool? ProjectIsActive { get; set; }

        public bool ProjectIsArchived { get; set; }

        public DateTime ProjectCreationTime { get; set; }

        [NotMapped]
        public string ProjectCreationTimeString { get; set; }

        public DateTime ProjectCompletionTime { get; set; }

        [NotMapped]
        public string ProjectCompletionTimeString { get; set; }

        public int ProjectTotalTasks { get; set; }

        [NotMapped]
        public Dictionary<string,int> ProjectStatusStats { get; set; }

    }

}
