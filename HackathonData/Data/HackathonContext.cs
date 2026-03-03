using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using HackathonData.Entities;
using Microsoft.EntityFrameworkCore;

namespace HackathonData.Data
{
    public class HackathonContext : DbContext
    {
        public HackathonContext(DbContextOptions<HackathonContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects => Set<Project>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var p = modelBuilder.Entity<Project>();

            p.HasKey(x => x.Id);
            p.Property(x => x.Id).ValueGeneratedNever();

            p.Property(x => x.TeamName)
                .IsRequired()
                .HasMaxLength(100);

            p.Property(x => x.ProjectName)
                .IsRequired()
                .HasMaxLength(120);

            p.Property(x => x.Category)
                .IsRequired()
                .HasMaxLength(50);

            p.Property(x => x.EventDate)
                .IsRequired();

            p.Property(x => x.Score)
                .IsRequired();

            p.Property(x => x.Members)
                .IsRequired();

            p.Property(x => x.Captain)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}

