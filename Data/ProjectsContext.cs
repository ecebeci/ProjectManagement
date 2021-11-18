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

        public DbSet<Project> Project { get; set; }
        public DbSet<Board> Boards { get; set; }
        // TODO
        //public DbSet<List> Lists { get; set; }
        //public DbSet<Work> Works { get; set; }

    }
}
