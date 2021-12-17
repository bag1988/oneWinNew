using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using oneWin.Models.generalModel;
using oneWin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace oneWin.Data
{
    public class AppDbContext : IdentityDbContext<userModel>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
          
        }

       public DbSet<controlerModel> controller { get; set; }
        public DbSet<actionModel> action { get; set; }
        public DbSet<roleForActionModel> roleAction { get; set; }
        public DbSet<roleForControllerModel> roleController { get; set; }

        public DbSet<orgIssueModel> OrgIssue { get; set; }

        public DbSet<calendarModel> Calendar { get; set; }

         public DbSet<fullAdresModel> fullAdres { get; set; }

        //public DbSet<logerModel> log { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<orgIssueModel>().Property(b => b.Id).HasDefaultValueSql("newid()");
        }
    }
}
