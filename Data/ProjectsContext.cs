using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectManagement.Models.ViewModels;

namespace ProjectManagement.Data
{
    public class ProjectsContext : DbContext
    {
       public ProjectsContext(DbContextOptions<ProjectsContext> options)
                : base(options)
            {
            }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // one-to-one for each project
             modelBuilder.Entity<Project>()
                 .HasOne(p => p.Manager) // entity framework helps on this lambda variables
                 .WithMany(p => p.Projects)
                 .HasForeignKey(p => p.ManagerId);

            // setting up many to many relationship
            // ProjectMember
            modelBuilder.Entity<ProjectMember>()
                .HasKey(pm => new { pm.ProjectId, pm.MemberId }); // two primary keys

            modelBuilder.Entity<ProjectMember>()
               .HasOne(p => p.Project)
               .WithMany(p => p.ProjectMembers)
               .HasForeignKey(p => p.ProjectId);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(p => p.Member)
                .WithMany(p => p.ProjectMembers)
                .HasForeignKey(p => p.MemberId);

            //WorkMember
            modelBuilder.Entity<WorkMember>()
            .HasKey(wm => new { wm.WorkId, wm.MemberId }); // two primary keys

            modelBuilder.Entity<WorkMember>()
               .HasOne(wm => wm.Work)
               .WithMany(wm => wm.WorkMembers)
               .HasForeignKey(wm => wm.WorkId);

            modelBuilder.Entity<WorkMember>()
                .HasOne(wm => wm.Member)
                .WithMany(wm => wm.WorkMembers)
                .HasForeignKey(wm => wm.MemberId);

            var foreignKeysWithCascadeDelete = modelBuilder.Model.GetEntityTypes()
             .SelectMany(t => t.GetForeignKeys())
             .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in foreignKeysWithCascadeDelete)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

        }

        // Entities are mapping for ORM
        public DbSet<Project> Project { get; set; }
        public DbSet<Board> Board { get; set; }
        public DbSet<List> List { get; set; }
        public DbSet<Work> Work { get; set; }
        public DbSet<Member> Member { get; set; }
        public DbSet<ProjectMember> ProjectMember { get; set; }
        public DbSet<WorkMember> WorkMember { get; set; }
        public DbSet<ProjectManagement.Models.Template> Template { get; set; }
        public DbSet<ProjectManagement.Models.ViewModels.AddTemplateViewModel> AddTemplateViewModel { get; set; }
    }
}
