using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortfolioWeb.Domain;

namespace PortfolioWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<PortfolioUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Status>().HasData(
                new Status
                {
                    Id = 1,
                    Description = "Planning",
                },
                new Status
                {
                    Id = 2,
                    Description = "Finished",
                },
                new Status
                {
                    Id = 3,
                    Description = "Currently working on",
                },
                new Status
                {
                    Id = 4,
                    Description = "Delayed",
                });

            modelBuilder.Entity<ProjectTag>()
                .HasKey(et => new { et.ProjectId, et.TagId });

            modelBuilder.Entity<ProjectTag>()
                .HasOne(et => et.Project)
                .WithMany(e => e.ProjectTags)
                .HasForeignKey(et => et.ProjectId);

            modelBuilder.Entity<ProjectTag>()
                .HasOne(et => et.Tag)
                .WithMany(e => e.ProjectTags)
                .HasForeignKey(et => et.TagId);

            modelBuilder.Entity<Tag>()
                .HasData(
                new Tag { Id = 1, Name = "C#" },
                new Tag { Id = 2, Name = "ASP.NET Core" },
                new Tag { Id = 3, Name = "Winforms" },
                new Tag { Id = 4, Name = "html" },
                new Tag { Id = 5, Name = "css" },
                new Tag { Id = 6, Name = "javascript" }
                );
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProjectTag> ProjectTags { get; set; }
    }
}
