using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_CouponAPI.Repository
{
    public class CouponRepository:ICouponRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public CouponRepository(ApplicationDBContext dBContext)
        {
            _dbContext= dBContext;
        }

        public async Task CreateAsync(Coupon coupon)
        {
            _dbContext.Coupones.Add(coupon);
            
        }

        public async Task DeleteAsync(Coupon coupon)
        {
            _dbContext.Coupones.Remove(coupon);
        }

        public async Task<ICollection<Coupon>> GetAllAsync()
        {
            return await _dbContext.Coupones.ToListAsync();
        }

        public async Task<Coupon> GetAsync(int id)
        {
            return await _dbContext.Coupones.FirstOrDefaultAsync(u=>u.Id==id);
        }

        public async Task<Coupon> GetAsync(string CouponName)
        {
            return await _dbContext.Coupones.FirstOrDefaultAsync(u=>u.Name.ToLower()==CouponName.ToLower());
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Coupon coupon)
        {
           // _dbContext.Coupones.Update(coupon);
           var editCoupon=_dbContext.Coupones.FirstOrDefault(u=>u.Id== coupon.Id);
            if (editCoupon!=null)
            {
                editCoupon.Percent = coupon.Percent;
                editCoupon.Name= coupon.Name;
                editCoupon.IsActive= coupon.IsActive;
                editCoupon.Created=coupon.Created;
                editCoupon.LastUpdated=DateTime.Now;
            }
        }
    }
}
