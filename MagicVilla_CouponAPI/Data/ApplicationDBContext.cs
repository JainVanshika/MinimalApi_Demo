using MagicVilla_CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_CouponAPI.Data
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext>options):base(options)
        {

        }
        public DbSet<Coupon> Coupones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coupon>().HasData(
            new Coupon()
            {
                Id = 1,
                Name = "10OFF",
                Percent = 10,
                IsActive = true,
            },
            new Coupon()
            {
                Id = 2,
                Name = "20OFF",
                Percent = 20,
                IsActive = false,
            });
        }
    }
}
