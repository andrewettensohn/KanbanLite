using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskList_ToDo.Models;
using TodoApi.Models;

namespace TaskList_ToDo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //    => options.UseSqlite("Data Source=kanbanLiteDb.db");

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<TodoItem> TodoItems { get; set; }

        public DbSet<TodoSubItem> TodoSubItems { get; set; }

        public DbSet<ProjectItem> ProjectItems { get; set; }

    }
}
