using DomainLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Contexts
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stadium>().HasOne(x => x.AppUser).WithOne(x => x.Stadium).HasForeignKey<Stadium>(x => x.AppUserId);

            modelBuilder.Entity<Reservation>().HasOne(x => x.Area).WithMany(x => x.Reservations).HasForeignKey(x => x.AreaId);

            modelBuilder.Entity<StadiumImage>().HasOne(x => x.Stadium).WithMany(x => x.StadiumImages).HasForeignKey(x => x.StadiumId);

            modelBuilder.Entity<StadiumDiscount>().HasOne(x => x.Stadium).WithMany(x => x.StadiumDiscounts).HasForeignKey(x => x.StadiumId);

            modelBuilder.Entity<Area>().HasOne(x => x.Stadium).WithMany(x => x.Areas).HasForeignKey(x => x.StadiumId);
            
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Stadium> Stadiums { get; set; }
        public DbSet<StadiumImage> StadiumImages { get; set; }
        public DbSet<StadiumDiscount> StadiumDiscounts { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
    }
}
