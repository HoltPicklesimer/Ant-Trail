﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker2.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BugTracker2.Models;

namespace BugTracker2.Areas.Identity.Data
{
    public class BugTracker2Context : IdentityDbContext<User>
    {
        public BugTracker2Context(DbContextOptions<BugTracker2Context> options)
            : base(options)
        {
        }

        public DbSet<Bug> Bugs { get; set; }
        public DbSet<Privilege> Privilege { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<Severity> Severity { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<UserProjectInfo> UserProjectInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<Bug>().ToTable("Bug");
            builder.Entity<Privilege>().ToTable("Privilege");
            builder.Entity<Project>().ToTable("Project");
            builder.Entity<Severity>().ToTable("Severity");
            builder.Entity<Status>().ToTable("Status");
            builder.Entity<UserProjectInfo>().ToTable("UserProject");
        }
    }
}
