using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Data
{
    public class ProjectsContext : DbContext
    {
       public ProjectsContext(DbContextOptions<ProjectsContext> options)
                : base(options)
            {
            }

      

        // Entities are mapping for ORM
        public DbSet<Project> Project { get; set; }
        public DbSet<Board> Board { get; set; }
        public DbSet<List> List { get; set; }
        public DbSet<Work> Work { get; set; }

    }
}
