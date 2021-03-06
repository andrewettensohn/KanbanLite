﻿
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TaskList_ToDo.Models;

namespace TodoApi.Models
{
    public class TodoItem
    {
        public int TodoItemID { get; set; }

        public string UserId { get; set; }

        public int ProjectID { get; set; }

        public string TaskName { get; set; }

        public string TaskStatus { get; set; }

        public DateTime TaskCreationTime { get; set; }

        public DateTime TaskInProgressTime { get; set; }

        public DateTime TaskCompletionTime { get; set; }

        public List<TodoSubItem> TodoSubItems { get; set; }

    }
}